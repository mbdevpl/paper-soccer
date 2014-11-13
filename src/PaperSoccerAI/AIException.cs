using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {
	public class AIException : InvalidOperationException {
		public AIException(string message) : base(message) { }
	}
}
