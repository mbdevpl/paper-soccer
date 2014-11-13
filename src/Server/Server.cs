using System;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Objects;
using System.IO;


namespace Server
{
    public class Server
    {
        static ArrayList clients = new ArrayList();
        static ArrayList games = new ArrayList();
        static ArrayList types = new ArrayList();
        static ArrayList masters = new ArrayList();
        volatile static Boolean cham;
        static void Main(string[] args)
        {
            cham = false;
            types.Add("paper_football");
            TcpListener serverSocket = new TcpListener(8888);
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();

            Console.WriteLine("Press 1 for championship mode");
            if (Console.ReadLine() == "1")
            {
                cham = true;
                Console.WriteLine(" >> Server Started in Championship mode");
            }
            else
                Console.WriteLine(" >> " + "Server Started");
            while (true)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                StreamReader reader = new StreamReader(clientSocket.GetStream());
                StreamWriter writer = new StreamWriter(clientSocket.GetStream());
                message msg;
                Console.WriteLine("Approving connection...");
                try
                {
                    msg = XmlParser.listen(reader);
                    if (msg._type == messageType.playerLogin)
                    {
                        int i;
                        if ((i = validate(msg)) != 0)
                        {
                            message m = new message();
                            m._type = messageType.loginResponse;
                            m._loginResponse = new loginResponse();
                            m._loginResponse._accept = accept.no;
                            m._loginResponse._error = new error();
                            m._loginResponse._error._id = i;
                            writer.WriteLine(XmlParser.Deparse(m));
                            writer.Flush();
                            continue;
                        }
                        else
                        {
                            message m = new message();
                            m._type = messageType.loginResponse;
                            m._loginResponse = new loginResponse();
                            m._loginResponse._accept = accept.yes;
                            writer.WriteLine(XmlParser.Deparse(m));
                            writer.Flush();
                        }
                        Console.WriteLine("Client: " + msg._playerLogin._nick + " connected");
                        handleClient c = new handleClient(clientSocket, msg._playerLogin._gameType, msg._playerLogin._nick);
                        bool flag = true;
                        if (!cham)
                            foreach (game g in games)
                            {
                                if (!g.full && g._type == msg._playerLogin._gameType)
                                {
                                    g.addplayer(c);
                                    flag = false;
                                    break;
                                }
                            }
                        if (flag)
                            clients.Add(c);
                    }
                    else if (msg._type == messageType.gameMasterLogin)
                    {
                        int i;
                        if ((i = validate(msg)) != 0)
                        {
                            message m = new message();
                            m._type = messageType.loginResponse;
                            m._loginResponse = new loginResponse();
                            m._loginResponse._accept = accept.no;
                            m._loginResponse._error = new error();
                            m._loginResponse._error._id = i;
                            writer.WriteLine(XmlParser.Deparse(m));
                            writer.Flush();
                            continue;
                        }
                        else
                        {
                            message m = new message();
                            m._type = messageType.loginResponse;
                            m._loginResponse = new loginResponse();
                            m._loginResponse._accept = accept.yes;
                            writer.WriteLine(XmlParser.Deparse(m));
                            writer.Flush();
                        }
                        Console.WriteLine("GameMaster: " + msg._gameMasterLogin._id + " connected");
                        handleMaster master = new handleMaster(clientSocket, msg._gameMasterLogin._id);
                        masters.Add(master);
                        if (!cham)
                        {
                            game g = new game(master, msg._gameMasterLogin._playersMin,
                                msg._gameMasterLogin._playersMax, new Random().Next().ToString(), msg._gameMasterLogin._gameType);
                            games.Add(g);
                            foreach (handleClient c in new ArrayList(clients))
                            {
                                if (!g.full && c._type == g._type)
                                {
                                    g.addplayer(c);
                                    clients.Remove(c);
                                }
                            }
                        }
                    }
                    else
                        Console.WriteLine("Invalid initial message type");
                }
                catch (IOException) { continue; }
                if (cham && clients.Count > 1 && masters.Count != 0)
                {
                    handleMaster master = (handleMaster)masters[0];
                    Console.WriteLine("Press 1 to start a championship, or other key to wait for more players");
                    if (Console.ReadLine() == "1")
                    {
                        Console.WriteLine("Championship started");
                        for (int i = 0; i < clients.Count - 1; i++)
                        {
                            for (int j = i + 1; j < clients.Count; j++)
                            {
                                game g = new game(master, 2, 2, new Random().Next().ToString(), (string)types[0]);
                                g.addplayer((handleClient)clients[i]);
                                g.addplayer((handleClient)clients[j]);
                                g.waitforscore();
                                Console.WriteLine("Actual scores:");
                                foreach (handleClient client in clients)
                                    Console.WriteLine(client._nick + " : " + client.won);
                            }
                        }
                    
                    message cm = new message();
                    cm._type = messageType.championsList;
                    cm._players = new List<player>();
                    foreach(handleClient hc in clients)
                    {
                        player p = new player();
                        p._nick = hc._nick;
                        p._won = hc.won;
                        p._lost = hc.lost;
                        cm._players.Add(p);
                    }
                    string txt = XmlParser.Deparse(cm);

                    foreach (handleClient hc in clients)
                        hc.sendmsg(txt);

                    foreach (handleMaster hm in masters)
                        hm.sendmsg(txt);

                    Console.WriteLine("Press a key to exit");
                    Console.ReadKey();

                    return;
                    }
                }
            }
        }
        /// <summary>
        /// The class that handles all communication with a client
        /// </summary>
        public class handleClient
        {
            TcpClient clientSocket;
            public game _game { get; set; }
            public int won { get; set; }
            public int lost { get; set; }
            NetworkStream networkStream;
            StreamWriter writer;
            StreamReader reader;
            public string _type { get; set; }
            public string _nick { get; set; }
            public handleClient(TcpClient inClientSocket, string type, string nick)
            {
                this.clientSocket = inClientSocket;
                networkStream = clientSocket.GetStream();
                writer = new StreamWriter(networkStream);
                reader = new StreamReader(networkStream);
                _type = type;
                _nick = nick;
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }
            public void sendmsg(string msg)
            {
                writer.WriteLine(msg);
                writer.Flush();
            }
            private void doChat()
            {
                try
                {
                    message msg;
                    while (true)
                    {
                        msg = XmlParser.listen(reader);
                        switch (msg._type)
                        {
                            case messageType.error:
                                Console.WriteLine(msg._text);
                                break;
                            case messageType.move:
                                _game.tell_master(msg);
                                break;
                            default:
                                Console.WriteLine("Obtained " + msg._type + " at client handler, but i don't know what to do with it");
                                break;
                        }
                    }
                }
                catch (IOException)
                {
                    if (_game != null)
                    {
                        message msg = new message();
                        msg._type = messageType.leaveGame;
                        _game.tell_master(msg);
                    }
                    Console.WriteLine("Client disconnected from the server");
                }
            }
        }
        /// <summary>
        /// The class that handles all communication with a gamemaster
        /// </summary>
        public class handleMaster
        {
            public game _game { get; set; }
            TcpClient clientSocket;
            NetworkStream networkStream;
            StreamWriter writer;
            StreamReader reader;
            public string _nick;
            public handleMaster(TcpClient client, string nick)
            {
                _nick = nick;
                networkStream = client.GetStream();
                writer = new StreamWriter(networkStream);
                reader = new StreamReader(networkStream);
                Thread ctThread = new Thread(doChat);
                ctThread.Start();
            }
            private void doChat()
            {
                try
                {
                    message msg;
                    while (true)
                    {
                        msg = XmlParser.listen(reader);
                        switch (msg._type)
                        {
                            case messageType.error:
                                Console.WriteLine(msg._text);
                                break;
                            case messageType.gameState:
                                _game.pass(msg);
                                if (msg._gameOver.Count != 0)
                                {
                                    foreach (player p in msg._gameOver)
                                        foreach (handleClient client in _game._clients)
                                            if (client._nick == p._nick)
                                                if (p._result == result.winner)
                                                {
                                                    client.won++;
                                                }
                                                else client.lost++;
                                }
                                break;
                            case messageType.move:
                                _game.pass(msg);
                                break;
                            case messageType.thankYou:
                                if (cham)
                                {
                                    _game.playing = false;
                                    break;
                                }
                                _game.pass(msg);
                                break;
                            default:
                                Console.WriteLine("Obtained " + msg._type + " at master handler, but i don't know what to do with it");
                                break;
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Client disconnected from the server");
                }
            }
            public void sendmsg(string msg)
            {
                try
                {
                    writer.WriteLine(msg);
                    writer.Flush();
                }
                catch (IOException)
                {

                }
            }

        }
        /// <summary>
        /// A game class constituting of a gamemaster and players, can be
        /// locked or not, depanding whether it waits for additional players
        /// </summary>
        public class game
        {
            int _min, _max;
            public string _type { get; set; }
            public string _id { get; set; }
            public Boolean full { get; set; }
            public volatile Boolean playing;
            public ArrayList _clients;
            handleMaster _master;
            public game(handleMaster master, int min, int max, string id, string type)
            {
                _clients = new ArrayList();
                _master = master;
                _min = min;
                _max = max;
                _id = id;
                _type = type;
                playing = true;
            }
            public void tell_master(message msg)
            {
                _master.sendmsg(XmlParser.Deparse(msg));
            }
            public void pass(message msg)
            {
                string text = XmlParser.Deparse(msg);
                _master.sendmsg(text);
                foreach (handleClient client in _clients)
                    client.sendmsg(text);
            }
            public void addplayer(handleClient client)
            {
                _clients.Add(client);
                Console.WriteLine("Player: " + client._nick + " joined game: " + _id + "!");
                if (_clients.Count < _min)
                {
                    Console.WriteLine("Game: " + _id +
                    " still waiting for " + (_min - _clients.Count) + "player(s)");
                    return;
                }
                full = true;
                Console.WriteLine("Game: " + _id + " has started");
                foreach (handleClient c in _clients)
                    c._game = this;
                _master._game = this;
                message msg = new message();
                msg._type = messageType.beginGame;
                msg._gameId = new gameId();
                msg._gameId._id = _id;
                msg._players = new List<player>();
                foreach (handleClient c in _clients)
                {
                    player p = new player();
                    p._nick = c._nick;
                    msg._players.Add(p);
                }
                _master.sendmsg(XmlParser.Deparse(msg));
            }
            public void waitforscore()
            {
                while (playing)
                    Thread.Sleep(100);
                return;
            }
        }
        /// <summary>
        /// A validation method for messages
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        static public int validate(message msg)
        {
            if (msg._type == messageType.gameMasterLogin)
            {
                if (!(types.Contains(msg._gameMasterLogin._gameType)))
                    return 2;
                foreach (game g in games)
                    if (g._id == msg._gameMasterLogin._id)
                        return 4;
                return 0;
            }
            else
            {
                if (!(types.Contains(msg._playerLogin._gameType)))
                    return 2;
                foreach (handleClient c in clients)
                    if (c._nick == msg._playerLogin._nick)
                        return 1;
                return 0;
            }
        }
    }
}
