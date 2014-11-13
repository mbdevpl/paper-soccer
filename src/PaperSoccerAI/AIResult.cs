using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Objects;

namespace PaperSoccerAI {

	/// <summary>
	/// Stores the work result of AI. 
	/// </summary>
	public class AIResult {

		/// <summary>
		/// What move was the latest generated move.
		/// </summary>
		public List<point> Moves;

		/// <summary>
		/// How does the board currently look.
		/// </summary>
		public GameBoard Board;

		/// <summary>
		/// What happened with this move, what is the current situation on the board.
		/// </summary>
		public GameEvent Event;

		public AIResult(Point moveStart, Point moveEnd, GameBoard gameBoard, GameEvent gameEvent) {
			
			Moves = new List<point>();
			point start = new point();
			start._x = moveStart.X;
			start._y = moveStart.Y;
			Moves.Add(start);
			point end = new point();
			end._x = moveEnd.X;
			end._y = moveEnd.Y;
			Moves.Add(end);
			
			Board = gameBoard;
			Event = gameEvent;
		}

	}

}
