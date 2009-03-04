using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user is an operator on the current server.
	/// </summary>
	public class ServerOperatorMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ServerOperatorMode"/> class.
		/// </summary>
		public ServerOperatorMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ServerOperatorMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ServerOperatorMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "O";
			}
		}

	}
}
