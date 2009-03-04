using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user will receive debug messages.
	/// </summary>
	public class ReceiveDebugMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveDebugMode"/> class.
		/// </summary>
		public ReceiveDebugMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveDebugMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ReceiveDebugMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "d";
			}
		}

	}
}
