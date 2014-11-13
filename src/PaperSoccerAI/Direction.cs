using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {

	/// <summary>
	/// Possible directions of single move in paper soccer.
	/// </summary>
	public enum Direction {
		UpRight = 0x1001,
		Right = 0x1000,
		DownRight = 0x1100,
		Down = 0x0100,
		DownLeft = 0x0110,
		Left = 0x0010,
		UpLeft = 0x0011,
		Up = 0x0001
	}

}
