using System;
using System.Net.Sockets;
using System.IO;
using Objects;
using System.Collections.Generic;
using PaperSoccerAI;
using System.Threading;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            int[,] pitch = new int[10,8];
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
            m._type = messageType.playerLogin;
            m._playerLogin = new playerLogin();
            m._playerLogin._gameType = "paper_football";
            string nick = new Random().Next().ToString();
            m._playerLogin._nick = nick;
            Console.WriteLine("My nick is " + nick);
            string text = XmlParser.Deparse(m);
            Console.WriteLine("Sending login request...");
            writer.WriteLine(text);
            message msg = null;
            GameBoard board=null;
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
                            board = new GameBoard();
                            break;
                        case messageType.gameState:
                            Console.WriteLine("Obtained a new game state");
                                if (msg._gameState!= null && msg._gameState._point != null)
                                {
                                    Console.WriteLine("Opponent moved");
                                    Console.Clear();
                                    Console.WriteLine(board.ToString());
                                    board.AddMove(msg._gameState._point);
                                }
                                if (msg._gameOver.Count != 0)
                                {
                                    foreach(player p in msg._gameOver)
                                        if(p._nick == nick)
                                            if (p._result == result.looser)
                                            {
                                                Console.WriteLine("Game ended and I lost");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Game ended and I won");
                                            }
                                    board = new GameBoard();
                                    break;
                                }
                                if (msg._nextPlayer!=null && msg._nextPlayer._nick == nick)
                                {
                                    Thread.Sleep(100);
                                    AIResult result;
                                    message nm = new message();
                                    nm._type = messageType.move;
                                    nm._point = new List<point>();
                                    result = AIEngine.GenerateMove(board);
                                    nm._point = result.Moves;
                                    Console.WriteLine("Sending my move");
                                    writer.WriteLine(XmlParser.Deparse(nm));
                                    break;
                                }
                                break;
                        case messageType.thankYou:
                            Console.WriteLine("Thanks for playing");
                            Console.ReadKey();
                            return;
                        case messageType.championsList:
                            foreach(player p in msg._players)
                                Console.WriteLine(p._nick + " has won: " + p._won + " and lost:" + p._lost);
                            Console.WriteLine("Press a key to exit");
                            Console.ReadKey();
                            return;
                        default:
                            Console.WriteLine("Obtained: " + msg._type + " and i don't know what to do with it.");
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
                if (msg != null)
                    Console.WriteLine(XmlParser.Deparse(msg));
                Console.WriteLine(e.ToString());
                Console.Read();
            }

        }
    }
}
