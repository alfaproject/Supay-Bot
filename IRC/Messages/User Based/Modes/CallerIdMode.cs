using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user using CallerId.
	/// </summary>
	public class CallerIdMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="CallerIdMode"/> class.
		/// </summary>
		public CallerIdMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="CallerIdMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public CallerIdMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "g";
			}
		}

	}
}
