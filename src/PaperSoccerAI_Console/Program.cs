using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PaperSoccerAI;
using System.Threading;

namespace PaperSoccerAI_Console {

	internal class Program {

		static void Main(string[] args) {
			//AddMove_Testing();

			AI_Testing();

			Console.ReadKey();
		}

		private static void AddMove_Testing() {
			Console.Out.WriteLine("Adding a move...");

			GameBoard gb = new GameBoard();
			Console.Out.WriteLine("1)");
			Console.Out.WriteLine(gb.ToString());

			gb.AddMove(Direction.Up);
			gb.AddMove(Direction.Left);
			gb.AddMove(Direction.Down);
			gb.AddMove(Direction.DownLeft);
			Console.Out.WriteLine("2)");
			Console.Out.WriteLine(gb.ToString());

			gb.AddMove(Direction.DownLeft);
			Console.Out.WriteLine("3)");
			Console.Out.WriteLine(gb.ToString());

			gb.AddMove(Direction.Up);
			gb.AddMove(Direction.Up);
			gb.AddMove(Direction.Up);
			gb.AddMove(Direction.Up);
			gb.AddMove(Direction.UpRight);
			Console.Out.WriteLine("4)");
			Console.Out.WriteLine(gb.ToString());
		}

		private static void AI_Testing() {
			GameBoard gb = new GameBoard();
			Console.Out.WriteLine(gb);
			//AIEngine ai = new AIEngine(gb);


			try {
				for (int i = 0; ; i++) {
					Console.Out.Write(" generating move... ");
					AIResult aiMove = AIEngine.GenerateMove(gb);
					//gb = aiMove.Board;
					gb.IsMoveValid(aiMove.Moves);
					gb.AddMove(aiMove.Moves);
					Console.Out.WriteLine("done.");

					if (aiMove.Event != GameEvent.ExtraTurn) {
						Console.Out.WriteLine(String.Format("Move no.{0}: ", i));
						Console.Out.WriteLine("Ball is at " + gb.PointToOuter(gb.BallPos).ToString());
						Console.Out.WriteLine("Event = " + aiMove.Event);
						Console.Out.WriteLine("Moves[0] (i.e. start) = " + aiMove.Moves[0]._x + ','
							+ aiMove.Moves[0]._y);
						Console.Out.WriteLine("Moves[1] (i.e. end) = " + aiMove.Moves[1]._x + ','
							+ aiMove.Moves[1]._y);

						Console.Out.WriteLine(gb);

						Thread.Sleep(100);
						//Console.ReadKey();
					}

					if (aiMove.Event == GameEvent.BallBlocked)
						throw new AIException("No move was made.");

					if (aiMove.Event > GameEvent.SomeoneWon) {
						Console.Out.WriteLine("GAME OVER");
						break;
					}
				}
			} catch (AIException ex) {
				Console.Out.WriteLine("AI failed: ");
				Console.Out.WriteLine(ex);
			}
		}

	}

}
