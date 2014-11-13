using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System;
using System.Net.Sockets;
using System.IO;
using Objects;
using PaperSoccerAI;
namespace GameMaster
{
    public class GameMaster
    {


        static void Main(string[] args)
        {
            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();

            NetworkStream serverStream;

            Console.WriteLine("Client Started");

            //clientSocket.Connect("194.29.178.156",8888);
            //clientSocket.Connect("194.29.178.162",8888);
            //clientSocket.Connect("194.29.178.49",5555);
            clientSocket.Connect("127.0.0.1", 8888);

            Console.WriteLine("Client Socket Program - Server Connected ...");

            serverStream = clientSocket.GetStream();

            StreamWriter writer = new StreamWriter(serverStream);
            StreamReader reader = new StreamReader(serverStream);

            message m = new message();
            m._type = messageType.gameMasterLogin;
            m._gameMasterLogin = new gameMasterLogin();
            m._gameMasterLogin._gameType = "paper_football";
            string id = new Random().Next().ToString();
            m._gameMasterLogin._id = id;
            m._gameMasterLogin._playersMin = 2;
            m._gameMasterLogin._playersMax = 2;
            string text = XmlParser.Deparse(m);
            Console.WriteLine("Sending login request...");
            writer.WriteLine(text);
            message msg=null;
            string next=null;
            GameBoard board = new GameBoard();
            List<string> players = new List<string>();
            try
            {
                while (true)
                {
                    writer.Flush();
                    msg = XmlParser.listen(reader);
                    switch (msg._type)
                    {
                        case messageType.error:
                            Console.WriteLine(msg._text);
                            break;
                        case messageType.loginResponse:
                            if (msg._loginResponse._accept == accept.no)
                            {
                                switch (msg._loginResponse._error._id)
                                {
                                    case 1: Console.WriteLine("wrong nick"); break;
                                    case 2: Console.WriteLine("improper game type"); break;
                                    case 3: Console.WriteLine("players pool overflow"); break;
                                    case 4: Console.WriteLine("master for this game already registered"); break;
                                    case 5: Console.WriteLine("wrong game type description data"); break;
                                }
                                Console.ReadKey();
                                return;
                            }
                            else Console.WriteLine("Logged in succesfully");
                            break;
                        case messageType.beginGame:
                            Console.WriteLine("Starting the game...");
                            message nmsg = new message();
                            nmsg._type = messageType.gameState;
                            nmsg._nextPlayer = new nextPlayer();
                            players.Add(msg._players[0]._nick);
                            players.Add(msg._players[1]._nick);
                            Console.WriteLine("My players:");
                            foreach (string n in players)
                                Console.WriteLine(n);
                            if (new Random().Next() % 2 == 1)
                            {
                                nmsg._nextPlayer._nick = msg._players[1]._nick;
                                next = msg._players[1]._nick;
                            }
                            else
                            {
                                nmsg._nextPlayer._nick = msg._players[0]._nick;
                                next = msg._players[0]._nick;
                            }
                            Console.WriteLine(nmsg._nextPlayer._nick + " will move first");
                            writer.WriteLine(XmlParser.Deparse(nmsg));
                            writer.Flush();
                            break;
                        case messageType.gameState:
                            if (msg._gameState != null && msg._gameState._point != null)
                            {
                                board.AddMove(msg._gameState._point);
                            }
                            if (msg._gameOver.Count!=0)
                            {
                                message nm2 = new message();
                                nm2._type = messageType.thankYou;
                                writer.WriteLine(XmlParser.Deparse(nm2));
                                Console.WriteLine("Game finished");
                                board = new GameBoard();
                                players = new List<string>();
                            }
                            break;
                        case messageType.move:
                            message nm;
                            nm = new message();
                            switch (board.IsMoveValid(msg._point))
                            {
                                case GameEvent.EndOfTurn:
                                    nm._type = messageType.gameState;
                                    nm._gameState = new gameState();
                                    nm._gameState._point = msg._point;
                                    nm._nextPlayer = new nextPlayer();
                                    if (next == players[0])
                                    {
                                        nm._nextPlayer._nick = players[1];
                                        next = players[1];
                                    }
                                    else
                                    {
                                        nm._nextPlayer._nick = players[0];
                                        next = players[0];
                                    }

                                    writer.WriteLine(XmlParser.Deparse(nm));
                                    break;
                                case GameEvent.ExtraTurn:
                                    nm._type = messageType.gameState;
                                    nm._gameState = new gameState();
                                    nm._gameState._point = msg._point;
                                    nm._nextPlayer = new nextPlayer();
                                    if (next != players[0])
                                    {
                                        nm._nextPlayer._nick = players[1];
                                        next = players[1];
                                    }
                                    else
                                    {
                                        nm._nextPlayer._nick = players[0];
                                        next = players[0];
                                    }

                                    writer.WriteLine(XmlParser.Deparse(nm));
                                    break;
                                case GameEvent.PlayerOneWon:
                                    nm = new message();
                                    nm._type = messageType.gameState;
                                    nm._gameOver = new List<player>();
                                    foreach (string n in players)
                                    {
                                        player p = new player();
                                        p._nick = n;
                                        if (next == n)
                                        {
                                            Console.WriteLine("Player: " + n + " has won");
                                            p._result = result.winner;
                                        }
                                        else p._result = result.looser;
                                        nm._gameOver.Add(p);
                                    }
                                    writer.WriteLine(XmlParser.Deparse(nm));
                                    break;
                                case GameEvent.PlayerTwoWon: 
                                    nm = new message();
                                    nm._type = messageType.gameState;
                                    nm._gameOver = new List<player>();
                                    foreach (string n in players)
                                    {
                                        player p = new player();
                                        p._nick = n;
                                        if (msg._players[0]._nick != n)
                                            p._result = result.winner;
                                        else p._result = result.looser;
                                        nm._gameOver.Add(p);
                                    }
                                    writer.WriteLine(XmlParser.Deparse(nm));
                                    break;
                            }
                            break;
                        case messageType.thankYou:
                            Console.WriteLine("Thanks for playing");
                            Console.ReadKey();
                            return;
                        case messageType.championsList:
                            foreach (player p in msg._players)
                                Console.WriteLine(p._nick + " has won: " + p._won + " and lost:" + p._lost);
                            Console.WriteLine("Press a key to exit");
                            Console.ReadKey();
                            return;
                        default:
                            Console.WriteLine("Obtained: " + msg._type);
                            break;
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Client disconnected from the server");
            }
            catch(Exception e)
            {
                if(msg!=null)
                Console.WriteLine(XmlParser.Deparse(msg));
                Console.WriteLine(e.ToString());
                Console.Read();
            }
        }
    }
}
