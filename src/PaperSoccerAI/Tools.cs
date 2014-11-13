using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PaperSoccerAI {

	/// <summary>
	/// Methods that save a lot of time.
	/// </summary>
	public class Tools {

		public static Point GetPoint(UInt32 X, UInt32 Y) {
			Point p = new Point();
			p.X = (int)X;
			p.Y = (int)Y;
			return p;
		}

		public static Point GetPoint(Point source, Direction dir) {
			return GetPoint((UInt32)source.X, (UInt32)source.Y, dir);
		}

		public static Point GetPoint(UInt32 X, UInt32 Y, Direction dir) {
			Point p = new Point();
			p.X = (int)X;
			p.Y = (int)Y;

			if (dir.Equals(Direction.UpRight)) {
				p.X++;
				p.Y--;
			} else if (dir.Equals(Direction.Right))
				p.X++;
			else if (dir.Equals(Direction.DownRight)) {
				p.X++;
				p.Y++;
			} else if (dir.Equals(Direction.Down))
				p.Y++;
			else if (dir.Equals(Direction.DownLeft)) {
				p.X--;
				p.Y++;
			} else if (dir.Equals(Direction.Left))
				p.X--;
			else if (dir.Equals(Direction.UpLeft)) {
				p.X--;
				p.Y--;
			} else if (dir.Equals(Direction.Up))
				p.Y--;
			else throw new AIException("Unhandled direction, cannot create a new Point.");

			return p;
		}

		public static Point CopyPoint(Point source) {
			Point p = new Point();
			p.X = source.X;
			p.Y = source.Y;
			return p;
		}

		// implemented in GameBoard!
		//public static Point ToInnerCoords(Point outerCoords) {
		//}
		//public static Point ToOuterCoords(Point innerCoords) {
		//}

		public static Direction GetOpposite(Direction dir) {
			if (dir.Equals(Direction.UpRight))
				return Direction.DownLeft;
			if (dir.Equals(Direction.Right))
				return Direction.Left;
			if (dir.Equals(Direction.DownRight))
				return Direction.UpLeft;
			if (dir.Equals(Direction.Down))
				return Direction.Up;

			if (dir.Equals(Direction.DownLeft))
				return Direction.UpRight;
			if (dir.Equals(Direction.Left))
				return Direction.Right;
			if (dir.Equals(Direction.UpLeft))
				return Direction.DownRight;
			if (dir.Equals(Direction.Up))
				return Direction.Down;

			throw new AIException("Unhandled direction, cannot return the opposite.");
		}

		public static bool IsDiagonal(Direction dir) {
			if (dir.Equals(Direction.Left) || dir.Equals(Direction.Down)
				 || dir.Equals(Direction.Right) || dir.Equals(Direction.Up))
				return false;
			return true;
		}

		public static Direction? GetHorizontalPart(Direction dir) {
			if (dir.Equals(Direction.Right) || dir.Equals(Direction.UpRight)
				 || dir.Equals(Direction.DownRight))
				return Direction.Right;

			if (dir.Equals(Direction.Left) || dir.Equals(Direction.UpLeft)
				 || dir.Equals(Direction.DownLeft))
				return Direction.Left;

			return null;
		}

		public static Direction? GetVerticalPart(Direction dir) {
			if (dir.Equals(Direction.Up) || dir.Equals(Direction.UpRight)
				 || dir.Equals(Direction.UpLeft))
				return Direction.Up;

			if (dir.Equals(Direction.Down) || dir.Equals(Direction.DownRight)
				 || dir.Equals(Direction.DownLeft))
				return Direction.Down;

			return null;
		}

		public static Direction ChangeVertical(Direction dir) {
			if (dir.Equals(Direction.UpRight))
				return Direction.DownRight;
			if (dir.Equals(Direction.Right))
				return Direction.Right;
			if (dir.Equals(Direction.DownRight))
				return Direction.UpRight;
			if (dir.Equals(Direction.Down))
				return Direction.Up;

			if (dir.Equals(Direction.DownLeft))
				return Direction.UpLeft;
			if (dir.Equals(Direction.Left))
				return Direction.Left;
			if (dir.Equals(Direction.UpLeft))
				return Direction.DownLeft;
			if (dir.Equals(Direction.Up))
				return Direction.Down;

			throw new AIException("Unhandled direction, cannot change vertical part.");
		}

		public static Direction ChangeHorizontal(Direction dir) {
			if (dir.Equals(Direction.UpRight))
				return Direction.UpLeft;
			if (dir.Equals(Direction.Right))
				return Direction.Left;
			if (dir.Equals(Direction.DownRight))
				return Direction.DownLeft;
			if (dir.Equals(Direction.Down))
				return Direction.Down;

			if (dir.Equals(Direction.DownLeft))
				return Direction.DownRight;
			if (dir.Equals(Direction.Left))
				return Direction.Right;
			if (dir.Equals(Direction.UpLeft))
				return Direction.UpRight;
			if (dir.Equals(Direction.Up))
				return Direction.Up;

			throw new AIException("Unhandled direction, cannot change horizontal part.");
		}

		public static PlayerNumber GetNextPlayer(PlayerNumber plNum) {
			if (plNum.Equals(PlayerNumber.One))
				return PlayerNumber.Two;
			else
				return PlayerNumber.One;
		}

		public static Direction GetNextDirection(Direction dir) {
			if (dir.Equals(Direction.UpRight))
				return Direction.Right;
			if (dir.Equals(Direction.Right))
				return Direction.DownRight;
			if (dir.Equals(Direction.DownRight))
				return Direction.Down;
			if (dir.Equals(Direction.Down))
				return Direction.DownLeft;

			if (dir.Equals(Direction.DownLeft))
				return Direction.Left;
			if (dir.Equals(Direction.Left))
				return Direction.UpLeft;
			if (dir.Equals(Direction.UpLeft))
				return Direction.Up;
			if (dir.Equals(Direction.Up))
				return Direction.UpRight;

			throw new AIException("Unhandled direction, cannot get the next direction.");
		}

	}

}
