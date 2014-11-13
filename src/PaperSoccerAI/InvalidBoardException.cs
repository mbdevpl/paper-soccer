using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {
	public class InvalidBoardException : AIException {
		public InvalidBoardException(string message) : base(message) { }
	}
}
