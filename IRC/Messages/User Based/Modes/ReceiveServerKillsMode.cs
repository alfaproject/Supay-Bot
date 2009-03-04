using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user will receive wallop messages.
	/// </summary>
	public class ReceiveServerKillsMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveServerKillsMode"/> class.
		/// </summary>
		public ReceiveServerKillsMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveServerKillsMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ReceiveServerKillsMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "k";
			}
		}

	}
}
