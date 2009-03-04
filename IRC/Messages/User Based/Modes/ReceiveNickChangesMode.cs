using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user will receive client nick changes messages.
	/// </summary>
	public class ReceiveNickChangesMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveNickChangesMode"/> class.
		/// </summary>
		public ReceiveNickChangesMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveNickChangesMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ReceiveNickChangesMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "n";
			}
		}

	}
}
