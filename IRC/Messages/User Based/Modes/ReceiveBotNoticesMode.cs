using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user will receive bot notice messages.
	/// </summary>
	public class ReceiveBotNoticesMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveBotNoticesMode"/> class.
		/// </summary>
		public ReceiveBotNoticesMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReceiveBotNoticesMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public ReceiveBotNoticesMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "b";
			}
		}

	}
}
