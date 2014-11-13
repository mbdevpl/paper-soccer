using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {
	public class BallBlockedException : AIException {
		public BallBlockedException(string message) : base(message) { }
	}
}
