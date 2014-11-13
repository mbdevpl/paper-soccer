using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Objects; //Point

namespace PaperSoccerAI {

	/// <summary>
	/// State of the game board. It is NOT implemented as described 
	/// in file game_protocol_2012en.pdf located in Docs/requirements. Coordinates 
	/// are set differently, therefore all coordinates must be mapped before adding 
	/// anything to the board.
	/// </summary>
	public class GameBoard {

		public Point PointToInner(Point outerPoint) {
			Point p = new Point();
			p.X = outerPoint.X + (int)halfWidth;
			p.Y = (-1 * outerPoint.Y) + (int)halfHeight;
			return p;
		}

		public Point PointToOuter(Point innerPoint) {
			Point p = new Point();
			p.X = innerPoint.X - (int)halfWidth;
			p.Y = -1 * (innerPoint.Y - (int)halfHeight);
			return p;
		}

		private UInt32 halfWidth;
		private UInt32 width;
		public UInt32 Width {
			get { return width - 1; }
			//set { width = value; }
		}

		private UInt32 halfHeight;
		private UInt32 height;
		public UInt32 Height {
			get { return height - 1; }
			//set { height = value; }
		}

		private UInt32 goalWidth;

		private UInt32 goalStartX;
		public UInt32 GoalStartX {
			get { return goalStartX; }
			//set { goalStartX = value; }
		}

		private UInt32 goalEndX;
		public UInt32 GoalEndX {
			get { return goalEndX; }
			//set { goalEndX = value; }
		}

		private PlayerNumber currPlayer;
		public PlayerNumber CurrPlayer {
			get { return currPlayer; }
			//set { currPlayer = value; }
		}

		/// <summary>
		/// Initial position of ball.
		/// </summary>
		private Point initialBallPos;

		/// <summary>
		/// Current position of the ball.
		/// </summary>
		private Point ballPos;
		public Point BallPos {
			get { return Tools.CopyPoint(ballPos); }
			//set { ballPos = value; }
		}

		/// <summary>
		/// Basic storage of contents of the board. Stores all of the edges.
		/// </summary>
		private List<List<GamePoint>> boardEdges;

		/// <summary>
		/// Alternative store for contents of the board. Stores the full path 
		/// of the ball through the game.
		/// </summary>
		private List<Direction> pathOfBall;

		/// <summary>
		/// Creates a new standard paper soccer board. The size measured in nodes is 9x11,
		/// but the size measured in edges is a standard 8x10.
		/// </summary>
		public GameBoard() {
			this.width = 9;
			this.height = 13;
			this.halfWidth = (width - 1) / 2;
			this.halfHeight = (height - 1) / 2;

			this.goalWidth = 3;
			this.goalStartX = halfWidth - ((goalWidth - 1) / 2);
			this.goalEndX = halfWidth + ((goalWidth - 1) / 2);

			this.currPlayer = PlayerNumber.One;
			this.ballPos = Tools.GetPoint(this.halfWidth, this.halfHeight);
			this.initialBallPos = Tools.CopyPoint(ballPos);

			InitBoard();
			ClearBoard();
		}

		public GameBoard(GameBoard board) {
			this.width = board.width;
			this.height = board.height;
			this.halfWidth = board.halfWidth;
			this.halfHeight = board.halfHeight;

			this.goalWidth = board.goalWidth;
			this.goalStartX = board.goalStartX;
			this.goalEndX = board.goalEndX;

			this.currPlayer = board.currPlayer;
			this.ballPos = Tools.CopyPoint(board.ballPos);
			this.initialBallPos = Tools.CopyPoint(board.initialBallPos);

			InitBoard();
			ClearBoard();
			CopyBoardFrom(board.pathOfBall, initialBallPos);
		}

		private void InitBoard() {
			boardEdges = new List<List<GamePoint>>();
			for (UInt32 x = 0; x < width; x++) {
				boardEdges.Add(new List<GamePoint>());
				for (UInt32 y = 0; y < height; y++)
					boardEdges[(int)x].Add(new GamePoint());
			}

			pathOfBall = new List<Direction>();
		}

		private void ClearBoard() {
			int boardTop = 1, boardBottom = (int)height - 2,
				 boardLeftSide = 0, boardRightSide = (int)width - 1;

			for (int y = 0; y < boardTop; y++)
				for (int x = 0; x < width; x++)
					boardEdges[x][y].Nullify();

			for (int y = boardBottom + 1; y < height; y++)
				for (int x = 0; x < width; x++)
					boardEdges[x][y].Nullify();

			for (int x = 0; x < width; x++) {
				boardEdges[x][boardTop].Left = null;
				boardEdges[x][boardTop].UpLeft = null;
				boardEdges[x][boardTop].Up = null;
				boardEdges[x][boardTop].UpRight = null;
				boardEdges[x][boardTop].Right = null;

				boardEdges[x][boardBottom].Right = null;
				boardEdges[x][boardBottom].DownRight = null;
				boardEdges[x][boardBottom].Down = null;
				boardEdges[x][boardBottom].DownLeft = null;
				boardEdges[x][boardBottom].Left = null;
			}
			for (int y = 0; y < height; y++) {
				boardEdges[boardLeftSide][y].Down = null;
				boardEdges[boardLeftSide][y].DownLeft = null;
				boardEdges[boardLeftSide][y].Left = null;
				boardEdges[boardLeftSide][y].UpLeft = null;
				boardEdges[boardLeftSide][y].Up = null;

				boardEdges[boardRightSide][y].Up = null;
				boardEdges[boardRightSide][y].UpRight = null;
				boardEdges[boardRightSide][y].Right = null;
				boardEdges[boardRightSide][y].DownRight = null;
				boardEdges[boardRightSide][y].Down = null;
			}

			int boardStart = 0;
			int boardEnd = (int)height - 1;
			for (int x = (int)goalStartX; x <= goalEndX; x++) {
				bool notStart = x > goalStartX;
				bool notEnd = x < goalEndX;

				if (notEnd) {
					boardEdges[x][boardStart].DownRight = false;
					boardEdges[x + 1][boardStart + 1].UpLeft = false;
					boardEdges[x][boardStart + 1].Right = false;
					boardEdges[x][boardEnd].UpRight = false;
					boardEdges[x + 1][boardEnd - 1].DownLeft = false;
					boardEdges[x][boardEnd - 1].Right = false;
				}
				if (notStart && notEnd) {
					boardEdges[x][boardStart].Down = false;
					boardEdges[x][boardStart + 1].Up = false;
					boardEdges[x][boardEnd - 1].Down = false;
					boardEdges[x][boardEnd].Up = false;
				}
				if (notStart) {
					boardEdges[x][boardStart].DownLeft = false;
					boardEdges[x - 1][boardStart + 1].UpRight = false;
					boardEdges[x][boardStart + 1].Left = false;
					boardEdges[x][boardEnd].UpLeft = false;
					boardEdges[x - 1][boardEnd - 1].DownRight = false;
					boardEdges[x][boardEnd - 1].Left = false;
				}

			}
		}

		private void CopyBoardFrom(List<Direction> directions, Point initialPosition) {
			foreach (Direction dir in directions) {
				AddMove(dir, initialPosition, false);
				initialPosition = Tools.GetPoint(initialPosition, dir);
			}
		}

		//private void AddToPath(Direction dir) {
		//    pathOfBall.Add(dir);
		//}

		//public GamePoint GetPointOfBoardAtBall() {
		//    return GetPointOfBoard(ballPos);
		//}

		/// <summary>
		/// Returns a copy of the point of the board.
		/// </summary>
		/// <param name="p">coordinates</param>
		/// <returns>copy of the selected point of the board</returns>
		public GamePoint GetPointOfBoard(Point p) {
			if (p.X < this.width && p.Y < this.height)
				return new GamePoint(boardEdges[p.X][p.Y]);

			return null;
		}

		private GamePoint GetBoardEdges(Point p) {
			return boardEdges[p.X][p.Y];
		}

		//private GamePoint GetBoardEdges(UInt32 x, UInt32 y) {
		//    return boardEdges[(int)x][(int)y];
		//}

		private bool? GetBoardEdge(Point p, Direction dir) {
			return GetBoardEdges(p).GetValue(dir);
		}

		private void SetBoardEdge(Point p, Direction dir, bool? value) {
			GetBoardEdges(p).SetValue(dir, value);
		}

		//private bool? GetEdgeInBoardAtBall(Direction dir) {
		//    return GetEdgeInBoard(ballPos, dir);
		//}

		//private bool? GetEdgeInBoard(Point p, Direction dir) {
		//    return GetEdgeInBoard((UInt32)p.X, (UInt32)p.Y, dir);
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="points"></param>
		/// <returns>negative value in case of error</returns>
		public GameEvent IsMoveValid(List<point> points) {
			if (points == null || points.Count != 2)
				throw new AIException("expected a list of points of length 2!");

			Point start = PointToInner(new Point(points[0]._x, points[0]._y));
			Point end = PointToInner(new Point(points[1]._x, points[1]._y));

			//starting point has to be equal to the current ball position
			if (!start.Equals(this.ballPos))
				return GameEvent.IllegalMove;

			//start and end points have to be adjecent
			bool isNearby = false;
			Direction dir = Direction.UpRight;
			for (int i = 0; i < 8; i++) {
				if (Tools.GetPoint(start, dir).Equals(end)) {
					isNearby = true;
					break;
				}
				dir = Tools.GetNextDirection(dir);
			}
			if (!isNearby)
				return GameEvent.IllegalMove;

			if (!IsMovePossible(dir))
				return GameEvent.IllegalMove;

			var state = this.GetPointState(end);

			if (state == null) {
				if (this.GetPointOfBoard(end).IsEmpty()) {
					return GameEvent.EndOfTurn;
				} else
					return GameEvent.ExtraTurn;
			} else if (state.Equals(PlayerNumber.One))
				return GameEvent.PlayerOneWon;
			else if (state.Equals(PlayerNumber.Two))
				return GameEvent.PlayerTwoWon;

			return GameEvent.InternalError;
		}


		public bool AddMove(List<point> points) {
			if (points == null || points.Count != 2)
				throw new AIException("expected a list of points of length 2!");

			Point start = PointToInner(new Point(points[0]._x, points[0]._y));
			Point end = PointToInner(new Point(points[1]._x, points[1]._y));

			//starting point has to be equal to the current ball position
			if (!start.Equals(this.ballPos))
				return false;

			//start and end points have to be adjecent
			bool isNearby = false;
			Direction dir = Direction.UpRight;
			for (int i = 0; i < 8; i++) {
				if (Tools.GetPoint(start, dir).Equals(end)) {
					isNearby = true;
					break;
				}
				dir = Tools.GetNextDirection(dir);
			}
			if (!isNearby)
				return false;

			return AddMove(dir);
		}

		/// <summary>
		/// Checks if movement from selected point in the selected direction is in bounds 
		/// of the game board.
		/// </summary>
		/// <param name="p">starting point</param>
		/// <param name="dir">direction of movement</param>
		/// <returns>true if the checked move is in the bounds of the board</returns>
		private bool MoveDirectionValid(Point p, Direction dir) {
			if (dir == Direction.UpRight) {
				if (p.X == width - 1 || p.Y == 0)
					return false;
				return true;
			}
			if (dir == Direction.Right) {
				if (p.X == width - 1 || p.Y == 0 || p.Y == height - 1)
					return false;
				return true;
			}
			if (dir == Direction.DownRight) {
				if (p.X == width - 1 || p.Y == height - 1)
					return false;
				return true;
			}
			if (dir == Direction.Down) {
				if (p.X == 0 || p.X == width - 1 || p.Y == height - 1)
					return false;
				return true;
			}
			if (dir == Direction.DownLeft) {
				if (p.X == 0 || p.Y == height - 1)
					return false;
				return true;
			}
			if (dir == Direction.Left) {
				if (p.X == 0 || p.Y == 0 || p.Y == height - 1)
					return false;
				return true;
			}
			if (dir == Direction.UpLeft) {
				if (p.X == 0 || p.Y == 0)
					return false;
				return true;
			}
			if (dir == Direction.Up) {
				if (p.X == 0 || p.X == width - 1 || p.Y == 0)
					return false;
				return true;
			}
			throw new AIException("Unhandled move direction!");
		}

		/// <summary>
		/// This method can check all board conditions before performing a move.
		/// </summary>
		/// <param name="dir">direction of movement, from the ball</param>
		/// <param name="startingPoint"></param>
		/// <param name="validateMove">if true, board conditions are checked</param>
		/// <returns>true if adding an edge succeded</returns>
		private bool AddToBoard(Direction dir, Point startingPoint, bool validateMove) {
			if (validateMove && !MoveDirectionValid(startingPoint, dir))
				return false;

			var pt = GetBoardEdges(startingPoint);
			var edge = pt.GetValue(dir);
			if (validateMove && (edge == null || edge == true))
				return false;

			pt.SetValue(dir, true);
			Direction opDir = Tools.GetOpposite(dir);
			SetBoardEdge(Tools.GetPoint(startingPoint, dir), opDir, true);

			return true;
		}

		public bool IsMovePossible(Direction dir) {
			return MoveDirectionValid(ballPos, dir)
				 && GetBoardEdge(ballPos, dir) == false;
		}

		private bool AddMove(Direction dir, Point startingPoint, bool validateMove) {
			if (!AddToBoard(dir, startingPoint, validateMove))
				return false;

			if (startingPoint.Equals(ballPos))
				ballPos = Tools.GetPoint(ballPos, dir);
			pathOfBall.Add(dir);
			return true;
		}

		/// <summary>
		/// Adds a single move to the board.
		/// </summary>
		/// <param name="dir"></param>
		/// <returns>true if the move was successful</returns>
		public bool AddMove(Direction dir) {
			bool turnWillEnd = GetBoardEdges(Tools.GetPoint(ballPos, dir)).IsEmpty();

			bool moveResult = AddMove(dir, ballPos, true);

			if (moveResult == false)
				return moveResult;

			if (turnWillEnd)
				this.EndTurn();

			return moveResult;
		}

		public bool AddMoveSequence(List<Direction> dirList) {
			if (dirList == null || dirList.Count == 0)
				return false;
			for (int i = 0; i < dirList.Count; i++) {
				//it should be done on the copy of the board
				bool result = AddMove(dirList[i]);
				if (result == false)
					return false;
			}
			return true;
		}

		public void EndTurn() {
			currPlayer = Tools.GetNextPlayer(currPlayer);
		}

		private PlayerNumber? GetPointState(Point p) {
			if (p.Y == 0 && p.X >= this.goalStartX && p.X <= this.goalEndX)
				return PlayerNumber.Two;
			if (p.Y == this.height - 1 && p.X >= this.goalStartX && p.X <= this.goalEndX)
				return PlayerNumber.One;
			return null;
		}

		/// <summary>
		/// Gets the state of the game. Possible states are: "in game", 
		/// or "player no.# won".
		/// </summary>
		/// <returns>null if the game hasn't ended, number of the winner otherwise</returns>
		public PlayerNumber? GetGameState() {
			return GetPointState(ballPos);
		}

		public bool CheckIntegrity() {

			for (UInt32 y = 0; y < height; y++) {
				for (UInt32 x = 0; x < width; x++) {
					Point p = Tools.GetPoint(x, y);

					Direction dir = Direction.UpRight;
					for (int i = 0; i < 8; i++) {
						if (MoveDirectionValid(p, dir)) {
							Point p2 = Tools.GetPoint(p, dir);
							Direction dir2 = Tools.GetOpposite(dir);
							if (this.GetBoardEdge(p, dir) != this.GetBoardEdge(p2, dir2))
								return false;
						}
						dir = Tools.GetNextDirection(dir);
					}
				}
			}
			return true;
		}

		private char ToChar(Point p) {
			Char ball = 'O', nodeNull = 'N', nodeNormal = 'o', nodeGoal = 'G';

			GamePoint pt = GetBoardEdges(p);

			if (pt.IsNull())
				return nodeNull;
			else if (this.GetPointState(p) != null)
				return nodeGoal;
			else if (p.Equals(ballPos))
				return ball;

			return nodeNormal;

		}

		private char ToChar(Point p, Direction dir) {
			Char edgeNull = '#', edgeNone = ' ', edgeVertical = '|', edgeHorizontal = '-',
				 edgeDownRight = '\\', edgeUpRight = '/', edgeBothDiagonals = 'X';

			Point movedPoint = Tools.GetPoint(p, dir);
			Direction opposite = Tools.GetOpposite(dir);

			//out of bounds
			if (GetBoardEdge(p, dir) == null) {
				if (MoveDirectionValid(p, dir)
					 && GetBoardEdge(movedPoint, opposite) != null)
					return '!';
				return edgeNull;
			}

			//there is no edge
			if (GetBoardEdge(p, dir) == false) {
				if (MoveDirectionValid(p, dir)
					 && GetBoardEdge(movedPoint, opposite) != false)
					return '!';

				//diagonal may have intersecting edge
				if (Tools.IsDiagonal(dir)) {

					Direction vert = (Direction)Tools.GetVerticalPart(dir);
					Point ptVert = Tools.GetPoint(p, vert);
					Direction opVert = Tools.ChangeVertical(dir);

					if (MoveDirectionValid(p, vert)
					  && GetBoardEdge(ptVert, opVert) == true) {
						if (dir.Equals(Direction.DownRight)
							 || dir.Equals(Direction.UpLeft))
							return edgeUpRight;
						else
							return edgeDownRight;
					}

					Direction hori = (Direction)Tools.GetHorizontalPart(dir);
					Point ptHori = Tools.GetPoint(p, hori);
					Direction opHori = Tools.ChangeHorizontal(dir);

					if (MoveDirectionValid(p, hori)
					  && GetBoardEdge(ptHori, opHori) == true) {
						if (dir.Equals(Direction.DownRight)
							 || dir.Equals(Direction.UpLeft))
							return edgeUpRight;
						else
							return edgeDownRight;
					}
				}

				return edgeNone;
			}

			//there is an edge
			if (GetBoardEdge(p, dir) == true) {
				if (MoveDirectionValid(p, dir)
					 && GetBoardEdge(movedPoint, opposite) != true)
					return '!';

				//vertical or horizontal case is simple
				if (dir.Equals(Direction.Left) || dir.Equals(Direction.Right))
					return edgeHorizontal;
				if (dir.Equals(Direction.Up) || dir.Equals(Direction.Down))
					return edgeVertical;

				//diagonal may have intersecting edge
				Direction vert = (Direction)Tools.GetVerticalPart(dir);
				Direction hori = (Direction)Tools.GetHorizontalPart(dir);
				if (MoveDirectionValid(p, vert)
					 || MoveDirectionValid(p, hori)) {
					Point ptVert = Tools.GetPoint(p, vert);
					Direction opVert = Tools.ChangeVertical(dir);

					if (MoveDirectionValid(p, vert)
					  && GetBoardEdge(ptVert, opVert) == true)
						return edgeBothDiagonals;

					Point ptHori = Tools.GetPoint(p, hori);
					Direction opHori = Tools.ChangeHorizontal(dir);

					if (MoveDirectionValid(p, hori)
					  && GetBoardEdge(ptHori, opHori) == true)
						return edgeBothDiagonals;
				}

				//there can be no diagonal intersection

				if (dir.Equals(Direction.DownRight)
					 || dir.Equals(Direction.UpLeft))
					return edgeDownRight;
				else
					return edgeUpRight;

			}
			return '!';
		}

		public override string ToString() {
			//Char ball = 'O', nodeNull = 'N', nodeNormal = '#', nodeGoal = 'G',
			//    edgeNull = '?', edgeNone = '.', edgeVertical = '|', edgeHorizontal = '-',
			//    edgeDownRight = '\\', edgeUpRight = '/', edgeBothDiagonals = 'X';

			StringBuilder bld = new StringBuilder();
			for (UInt32 y = 0; y < height; y++) {

				//first row
				if (y == 0) {
					for (UInt32 x = 0; x < width; x++) {
						Point p = Tools.GetPoint(x, y);
						if (x == 0)
							bld.Append(ToChar(p, Direction.UpLeft));

						bld.Append(ToChar(p, Direction.Up));
						bld.Append(ToChar(p, Direction.UpRight));
					}
					bld.Append(" currPlayer = " + CurrPlayer);
					bld.Append('\n');
				}

				for (UInt32 x = 0; x < width; x++) {
					Point p = Tools.GetPoint(x, y);

					if (x == 0)
						bld.Append(ToChar(p, Direction.Left));
					bld.Append(ToChar(p));
					bld.Append(ToChar(p, Direction.Right));


					//if (pt.IsNull())
					//    bld.Append(nodeNull);
					//else if (this.GetPointState(p) != null)
					//    bld.Append(nodeGoal);
					//else if (ballPos.X == x && ballPos.Y == y)
					//    bld.Append(ball);
					//else
					//    bld.Append(nodeNormal);

					//if (pt.Right == true)
					//    bld.Append(edgeHorizontal);
					//else bld.Append(edgeNone);
				}

				if (y == 1) {
					bld.Append(" integrity check: " + this.CheckIntegrity());
				}

				if (y == halfHeight) {
					bld.Append(" ||");
				}

				bld.Append('\n');

				for (UInt32 x = 0; x < width; x++) {
					Point p = Tools.GetPoint(x, y);
					GamePoint pt = GetBoardEdges(p);

					if (x == 0)
						bld.Append(ToChar(p, Direction.DownLeft));
					bld.Append(ToChar(p, Direction.Down));
					bld.Append(ToChar(p, Direction.DownRight));

					//if (pt.Down == true)
					//    bld.Append(edgeVertical);
					//else
					//    bld.Append(edgeNone);

					//if (y < height - 1) {
					//    Point p2 = Tools.GetPoint(p, Direction.Down);
					//    GamePoint pt2 = GetBoardEdges(p2);
					//    if (pt.DownRight == true) {
					//        if (pt2.UpRight == true)
					//            bld.Append(edgeBothDiagonals);
					//        else
					//            bld.Append(edgeDownRight);
					//    } else if (pt2.UpRight == true)
					//        bld.Append(edgeUpRight);
					//    else
					//        bld.Append(edgeNone);
					//}
				}

				if (y == halfHeight - 1) {
					bld.Append(" /\\ Player Two");
				}

				if (y == halfHeight) {
					bld.Append(" \\/ Player One");
				}

				if (y != height - 1)
					bld.Append('\n');

			}
			return bld.ToString();
		}

	}

}
