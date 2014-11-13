using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaperSoccerAI {

	public class AIEngine {

		public static AIResult GenerateMove(GameBoard gameBoard) {
			GameBoard copy = new GameBoard(gameBoard);
			Point start = copy.PointToOuter(copy.BallPos);
			Point end = Tools.GetPoint(0, 0);
			GameEvent currentEvent = GameEvent.EndOfTurn;
			try {
				var dir = GetNextMove(copy);
				copy.AddMove(dir);
				var state = copy.GetGameState();

				if (state == null) {
					//must check on the original due to the modification of the board
					GamePoint ballPoint = gameBoard.GetPointOfBoard(copy.BallPos);
					if (ballPoint.IsEmpty()) {
						currentEvent = GameEvent.EndOfTurn;
					} else
						currentEvent = GameEvent.ExtraTurn;
				} else if (state.Equals(PlayerNumber.One))
					currentEvent = GameEvent.PlayerOneWon;
				else if (state.Equals(PlayerNumber.Two))
					currentEvent = GameEvent.PlayerTwoWon;
				else
					currentEvent = GameEvent.InternalError;

			} catch (BallBlockedException) {
				// game will soon end with the defeat of current player, 
				// but, if move can stiil be made, it has to be made
				GamePoint dis = new GamePoint();
				try {
					var dir = GeneratePartialMove(copy, dis);
					copy.AddMove(dir);
					var state = copy.GetGameState();

					if (state == null) {
						//must check on the original due to the modification of the board
						if (gameBoard.GetPointOfBoard(copy.BallPos).IsEmpty()) {
							copy.EndTurn();
							currentEvent = GameEvent.EndOfTurn;
						} else
							currentEvent = GameEvent.ExtraTurn;
					} else if (state.Equals(PlayerNumber.One))
						currentEvent = GameEvent.PlayerOneWon;
					else if (state.Equals(PlayerNumber.Two))
						currentEvent = GameEvent.PlayerTwoWon;
					else
						currentEvent = GameEvent.InternalError;

				} catch (BallBlockedException) {
					//after all game ends now, no move can be made anymore
					if (copy.CurrPlayer == PlayerNumber.One)
						currentEvent = GameEvent.PlayerTwoWon;
					else
						currentEvent = GameEvent.PlayerOneWon;
				}
			} catch (AIException) {
				currentEvent = GameEvent.InternalError;
			}

			end = copy.PointToOuter(copy.BallPos);

			return new AIResult(start, end, copy, currentEvent);
		}

		private static Direction GetNextMove(GameBoard board) {
			List<GameBoard> boards = new List<GameBoard>();
			boards.Add(board);
			List<Direction> generated = GenerateCompleteMove(new List<Direction>(),
				new List<GamePoint>(), boards);

			if (generated == null || generated.Count == 0)
				throw new BallBlockedException("empty move was generated");

			return generated[0];
		}

		internal static List<Direction> GenerateCompleteMove(List<Direction> path,
			List<GamePoint> disallowed, List<GameBoard> boards) {

			//first move
			bool firstMove = false;
			if (disallowed.Count == 0) {
				disallowed.Add(new GamePoint());
				firstMove = true;
			}

			if (boards.Count == 0)
				throw new AIException("list of boards should not be empty here!!!");


			Direction? dir = null;
			try {

				dir = GeneratePartialMove(boards[boards.Count - 1],
					disallowed[disallowed.Count - 1]);

			} catch (BallBlockedException) {
				if (path.Count == 0)
					return null;
				if (disallowed.Count == 0)
					throw new AIException("list of disallowed moves should not be empty here!!!");

				//marking the last move as forbidden in the future
				disallowed[disallowed.Count - 1].SetValue(path[path.Count - 1], true);
				//the last move was wrong, and lead to the dead-end, 
				// therefore it is subtracted from the path
				path.RemoveAt(path.Count - 1);
				boards.RemoveAt(boards.Count - 1);

				return GenerateCompleteMove(path, disallowed, boards);
			}

			if (dir == null)
				return null;

			boards.Add(new GameBoard(boards[boards.Count - 1]));

			path.Add((Direction)dir); //this direction is good, adding it to the path

			GameBoard lastBoard = boards[boards.Count - 1];
			lastBoard.AddMove((Direction)dir); //making a move 

			//not the first move
			if (!firstMove)
				disallowed.Add(new GamePoint());

			//C/onsole.Out.WriteLine(copy);

			GamePoint ballCopy = lastBoard.GetPointOfBoard(lastBoard.BallPos);

			ballCopy.SetValue(Tools.GetOpposite((Direction)dir), false);

			//end calculation because the winning move or any valid move was found
			if (ballCopy.IsEmpty() || lastBoard.GetGameState() != null) {
				return path;
			}

			return GenerateCompleteMove(path, disallowed, boards);
		}

		internal static Direction GeneratePartialMove(GameBoard board,
			GamePoint disallowed) {
			if (board.CurrPlayer.Equals(PlayerNumber.One)) {

				if (board.BallPos.X > board.GoalEndX)
					if (disallowed.GetValue(Direction.DownLeft) == false
						 && board.IsMovePossible(Direction.DownLeft))
						return Direction.DownLeft;

				if (board.BallPos.X < board.GoalStartX)
					if (disallowed.GetValue(Direction.DownRight) == false
						 && board.IsMovePossible(Direction.DownRight))
						return Direction.DownRight;

				if (disallowed.GetValue(Direction.Down) == false
					 && board.IsMovePossible(Direction.Down))
					return Direction.Down;

				if (disallowed.GetValue(Direction.DownLeft) == false
					 && board.IsMovePossible(Direction.DownLeft))
					return Direction.DownLeft;
				if (disallowed.GetValue(Direction.DownRight) == false
					 && board.IsMovePossible(Direction.DownRight))
					return Direction.DownRight;

				if (disallowed.GetValue(Direction.Left) == false
					 && board.IsMovePossible(Direction.Left))
					return Direction.Left;
				if (disallowed.GetValue(Direction.Right) == false
					 && board.IsMovePossible(Direction.Right))
					return Direction.Right;

				if (disallowed.GetValue(Direction.UpLeft) == false
					 && board.IsMovePossible(Direction.UpLeft))
					return Direction.UpLeft;
				if (disallowed.GetValue(Direction.UpRight) == false
					 && board.IsMovePossible(Direction.UpRight))
					return Direction.UpRight;

				if (disallowed.GetValue(Direction.Up) == false
					 && board.IsMovePossible(Direction.Up))
					return Direction.Up;
			} else if (board.CurrPlayer.Equals(PlayerNumber.Two)) {
				if (disallowed.GetValue(Direction.Up) == false
					 && board.IsMovePossible(Direction.Up))
					return Direction.Up;

				//randomize left/right choice
				if (disallowed.GetValue(Direction.UpRight) == false
					 && board.IsMovePossible(Direction.UpRight))
					return Direction.UpRight;
				if (disallowed.GetValue(Direction.UpLeft) == false
					 && board.IsMovePossible(Direction.UpLeft))
					return Direction.UpLeft;

				if (disallowed.GetValue(Direction.Right) == false
					 && board.IsMovePossible(Direction.Right))
					return Direction.Right;
				if (disallowed.GetValue(Direction.Left) == false
					 && board.IsMovePossible(Direction.Left))
					return Direction.Left;

				if (disallowed.GetValue(Direction.DownRight) == false
					 && board.IsMovePossible(Direction.DownRight))
					return Direction.DownRight;
				if (disallowed.GetValue(Direction.DownLeft) == false
					 && board.IsMovePossible(Direction.DownLeft))
					return Direction.DownLeft;

				if (disallowed.GetValue(Direction.Down) == false
					 && board.IsMovePossible(Direction.Down))
					return Direction.Down;
			}
			throw new BallBlockedException("There is no valid move possible. "
				 + "The ball is blocked or movement is masked.");
		}

		//public static AIResult GenerateMovesTillEndOfTurn() {
		//   return GenerateOneMove();
		//}

		//private GameBoard board;
		//public GameBoard Board {
		//   get { return board; }
		//   //set { board = value; }
		//}

		///// <summary>
		///// Creates a new AI engine, which will simulate the game.
		///// </summary>
		///// <param name="board">initial state of the board</param>
		///// second's is on the top</param>
		//public AIEngine(GameBoard board) {
		//   this.board = board;
		//}

		///// <summary>
		///// Returns direction of the next move.
		///// </summary>
		///// <param name="board">game board, on which the guess is performed</param>
		///// <param name="disallowed">mask that causes the guess to not be repeated</param>
		///// <returns></returns>
		//private static Direction GetBasicMove(GameBoard board, GamePoint disallowed) {
		//   if (board.CurrPlayer.Equals(PlayerNumber.One)) {
		//      if (disallowed.GetValue(Direction.Down) == false
		//          && board.IsMovePossible(Direction.Down))
		//         return Direction.Down;

		//      //randomize left/right choice
		//      if (disallowed.GetValue(Direction.DownLeft) == false
		//          && board.IsMovePossible(Direction.DownLeft))
		//         return Direction.DownLeft;
		//      if (disallowed.GetValue(Direction.DownRight) == false
		//          && board.IsMovePossible(Direction.DownRight))
		//         return Direction.DownRight;

		//      if (disallowed.GetValue(Direction.Left) == false
		//          && board.IsMovePossible(Direction.Left))
		//         return Direction.Left;
		//      if (disallowed.GetValue(Direction.Right) == false
		//          && board.IsMovePossible(Direction.Right))
		//         return Direction.Right;

		//      if (disallowed.GetValue(Direction.UpLeft) == false
		//          && board.IsMovePossible(Direction.UpLeft))
		//         return Direction.UpLeft;
		//      if (disallowed.GetValue(Direction.UpRight) == false
		//          && board.IsMovePossible(Direction.UpRight))
		//         return Direction.UpRight;

		//      if (disallowed.GetValue(Direction.Up) == false
		//          && board.IsMovePossible(Direction.Up))
		//         return Direction.Up;
		//   } else if (board.CurrPlayer.Equals(PlayerNumber.Two)) {
		//      if (disallowed.GetValue(Direction.Up) == false
		//          && board.IsMovePossible(Direction.Up))
		//         return Direction.Up;

		//      if (disallowed.GetValue(Direction.UpRight) == false
		//          && board.IsMovePossible(Direction.UpRight))
		//         return Direction.UpRight;
		//      if (disallowed.GetValue(Direction.UpLeft) == false
		//          && board.IsMovePossible(Direction.UpLeft))
		//         return Direction.UpLeft;

		//      if (disallowed.GetValue(Direction.Right) == false
		//          && board.IsMovePossible(Direction.Right))
		//         return Direction.Right;
		//      if (disallowed.GetValue(Direction.Left) == false
		//          && board.IsMovePossible(Direction.Left))
		//         return Direction.Left;

		//      if (disallowed.GetValue(Direction.DownRight) == false
		//          && board.IsMovePossible(Direction.DownRight))
		//         return Direction.DownRight;
		//      if (disallowed.GetValue(Direction.DownLeft) == false
		//          && board.IsMovePossible(Direction.DownLeft))
		//         return Direction.DownLeft;

		//      if (disallowed.GetValue(Direction.Down) == false
		//          && board.IsMovePossible(Direction.Down))
		//         return Direction.Down;
		//   }
		//   throw new BallBlockedException("There is no valid move possible. "
		//       + "The ball is blocked or movement is masked.");
		//}

		//public List<Direction> GenerateMove(UInt32 milisecondsToTimeout,
		//   UInt32 movesNumberLimit) {
		//   List<Direction> path = new List<Direction>();
		//   List<GamePoint> disallowed = new List<GamePoint>();
		//   GameBoard copy = new GameBoard(board);

		//   GameBoard completeCopy = new GameBoard(board);

		//   bool completeMoveFound = false;
		//   while (!completeMoveFound) {
		//      bool partialMoveFound = false;
		//      GameBoard partialCopy = new GameBoard(completeCopy);
		//      while (!partialMoveFound) {
		//         try {
		//            Direction dir = GetBasicMove(copy, disallowed[disallowed.Count - 1]);
		//            path.Add(dir);
		//            partialCopy.AddMove(dir);
		//            disallowed.Add(new GamePoint());

		//            partialMoveFound = true;

		//         } catch (BallBlockedException) {
		//            if (disallowed.Count == 0)
		//               throw new AIException("Cannot generate any move, ball is blocked in initial state.");
		//            disallowed[disallowed.Count - 1].SetValue(path[path.Count - 1], true);
		//            path.RemoveAt(path.Count - 1);
		//            //break;
		//         }
		//      }
		//   }

		//   return path;

		//   //List<Direction> path = new List<Direction>();
		//   //List<GamePoint> disallowed = new List<GamePoint>();
		//   //GameBoard copy = new GameBoard(board);

		//   //if (milisecondsToTimeout == 0 || movesNumberLimit == 0) {


		//   //   //loop is needed to find a tree of moves
		//   //   bool moveFound = false;
		//   //   while (!moveFound) { //until a valid move is found
		//   //      try { //try to get a direction
		//   //         GameBoard copy2 = new GameBoard(copy);
		//   //         Direction dir;
		//   //         bool directionGot = false;
		//   //         if (disallowed.Count <= path.Count)
		//   //            disallowed.Add(new GamePoint()); //make a disallowed moves mask
		//   //         while (!directionGot) { //repeat until success of determination that
		//   //            try { //try to make a move
		//   //               dir = GetBasicMove(copy2, disallowed[disallowed.Count - 1]);
		//   //               directionGot = true;

		//   //               path.Add(dir); //add dir. to path
		//   //               copy2.AddMove(dir); // add dir to board's copy

		//   //               //collisions checking
		//   //               if (copy.GetPointOfBoard(copy.BallPos).IsEmpty()) {
		//   //                  moveFound = true;

		//   //                  copy2.EndTurn();
		//   //                  copy = new GameBoard(copy2);
		//   //               }
		//   //            } catch (BallBlockedException) { //the ball is blocked
		//   //               if (disallowed.Count > 0)
		//   //                  disallowed[disallowed.Count - 1].SetValue(path[path.Count - 1], true);
		//   //            }
		//   //            if (disallowed[disallowed.Count - 1].IsTrue())
		//   //               throw new BallBlockedException("this node is a dead-end, "
		//   //                   + "need to backtrack");
		//   //         }

		//   //      } catch (BallBlockedException) { //there is no valid direction
		//   //         disallowed[disallowed.Count - 1].SetValue(path[path.Count - 1], true);
		//   //         path.RemoveAt(path.Count - 1);
		//   //      }
		//   //   }
		//   //   board = new GameBoard(copy);
		//   //   return path;
		//   //}

		//   ////bool moveFound2 = false;
		//   ////while (!moveFound2) {
		//   ////    Direction dir = GetBasicMove(copy);
		//   ////    path.Add(dir);
		//   ////    if (board.GetPointOfBoard(Tools.GetPoint(board.BallPos, dir)).IsEmpty())
		//   ////        moveFound2 = true;
		//   ////}

		//   //return path;
		//}

	}

}
