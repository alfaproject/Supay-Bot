using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user will receive wallop messages.
	/// </summary>
	public class ReceiveWallopsMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveWallopsMode"/> class.
		/// </summary>
		public ReceiveWallopsMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveWallopsMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ReceiveWallopsMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "w";
			}
		}

	}
}
