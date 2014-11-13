using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {

	/// <summary>
	/// Contains edges coming from a single point. True means there is an edge, 
	/// false that there isn't, and null means that edge would lie outside 
	/// of the board.
	/// </summary>
	public class GamePoint {

		/// <summary>
		/// One of base statements.
		/// </summary>
		public bool? UpRight;

		/// <summary>
		/// One of base statements.
		/// </summary>
		public bool? Right;

		/// <summary>
		/// One of base statements.
		/// </summary>
		public bool? DownRight;

		/// <summary>
		/// One of base statements.
		/// </summary>
		public bool? Down;

		/// <summary>
		/// One of the derived statements.
		/// </summary>
		public bool? DownLeft;

		/// <summary>
		/// One of the derived statements.
		/// </summary>
		public bool? Left;

		/// <summary>
		/// One of the derived statements.
		/// </summary>
		public bool? UpLeft;

		/// <summary>
		/// One of the derived statements.
		/// </summary>
		public bool? Up;

		/// <summary>
		/// Sets all of the edges to false.
		/// </summary>
		public GamePoint() {
			Falsify();
		}

		/// <summary>
		/// Creates a copy of the source point.
		/// </summary>
		/// <param name="pt">source point</param>
		public GamePoint(GamePoint pt) {
			UpRight = pt.UpRight;
			Right = pt.Right;
			DownRight = pt.DownRight;
			Down = pt.Down;

			DownLeft = pt.DownLeft;
			Left = pt.Left;
			UpLeft = pt.UpLeft;
			Up = pt.Up;
		}

		/// <summary>
		/// Checks if this point is all empty.
		/// </summary>
		/// <returns>true if all of the edges are false</returns>
		public bool IsEmpty() {
			if (UpRight == false && Right == false
					  && DownRight == false && Down == false
					  && DownLeft == false && Left == false
					  && UpLeft == false && Up == false)
				return true;

			return false;

			//if (UpRight == true || Right == true 
			//        || DownRight == true || Down == true 
			//        || DownLeft == true || Left == true 
			//        || UpLeft == true || Up == true)
			//    return false;

			//return true;
		}

		/// <summary>
		/// Checks if this point is all null.
		/// </summary>
		/// <returns>true if all edges are null</returns>
		public bool IsNull() {
			if (UpRight == null && Right == null
					  && DownRight == null && Down == null
					  && DownLeft == null && Left == null
					  && UpLeft == null && Up == null)
				return true;

			return false;
		}

		public bool IsTrue() {
			if (UpRight == true && Right == true
					  && DownRight == true && Down == true
					  && DownLeft == true && Left == true
					  && UpLeft == true && Up == true)
				return true;

			return false;
		}

		public bool? GetValue(Direction dir) {
			if (dir.Equals(Direction.UpRight))
				return UpRight;
			if (dir.Equals(Direction.Right))
				return Right;
			if (dir.Equals(Direction.DownRight))
				return DownRight;
			if (dir.Equals(Direction.Down))
				return Down;

			if (dir.Equals(Direction.DownLeft))
				return DownLeft;
			if (dir.Equals(Direction.Left))
				return Left;
			if (dir.Equals(Direction.UpLeft))
				return UpLeft;
			if (dir.Equals(Direction.Up))
				return Up;

			throw new AIException("Unhandled direction!");
		}

		public void SetValue(Direction dir, bool? value) {
			if (dir.Equals(Direction.UpRight))
				UpRight = value;
			else if (dir.Equals(Direction.Right))
				Right = value;
			else if (dir.Equals(Direction.DownRight))
				DownRight = value;
			else if (dir.Equals(Direction.Down))
				Down = value;

			else if (dir.Equals(Direction.DownLeft))
				DownLeft = value;
			else if (dir.Equals(Direction.Left))
				Left = value;
			else if (dir.Equals(Direction.UpLeft))
				UpLeft = value;
			else if (dir.Equals(Direction.Up))
				Up = value;

			else
				throw new AIException("Unhandled direction!");
		}

		public void Nullify() {
			UpRight = null;
			Right = null;
			DownRight = null;
			Down = null;

			DownLeft = null;
			Left = null;
			UpLeft = null;
			Up = null;
		}

		public void Falsify() {
			UpRight = false;
			Right = false;
			DownRight = false;
			Down = false;

			DownLeft = false;
			Left = false;
			UpLeft = false;
			Up = false;
		}
	}

}
