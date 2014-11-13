using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperSoccerAI {

	/// <summary>
	/// Consequence of AI move. Non zero status means that the player changes 
	/// or the game stops. Two digit status means the game has ended properly.
	/// Negative status means that something went wrong with the AI.
	/// </summary>
	public enum GameEvent {

		/// <summary>
		/// Illegal move was to be performed.
		/// </summary>
		IllegalMove = -3,

		/// <summary>
		/// AI encountered an unrecoverable error.
		/// </summary>
		InternalError = -2,

		/// <summary>
		/// There is no possible valid move available. AI cannot make a move. 
		/// This should not be returned by the AI engine, since no move means victory 
		/// of oposing player.
		/// </summary>
		BallBlocked = -1,

		/// <summary>
		/// Move ended on an empty vertex. Other player may take a move.
		/// </summary>
		EndOfTurn = 1,

		/// <summary>
		/// Move ended on the intersection of some edges or on the side of the board. 
		/// Player that has just moved has an extra turn.
		/// </summary>
		ExtraTurn = 0,

		/// <summary>
		/// Game ended, player no. 1 has won. Either by scoring a goal, or by the fact that
		/// turn of player two ended with a BallBlockedException.
		/// </summary>
		PlayerOneWon = 10,

		/// <summary>
		/// Game ended, player no. 2 has won.
		/// </summary>
		PlayerTwoWon = 20,

		/// <summary>
		/// Can be used for game status checking.
		/// 
		/// Try 'Event > GameEvent.SomeoneWon' to check if anyone won.
		/// </summary>
		SomeoneWon = 9

	}

}
