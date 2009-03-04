using System;

namespace BigSister.Irc.Messages.Modes {

	/// <summary>
	/// This mode signifies that the user is an operator on the current network.
	/// </summary>
	public class NetworkOperatorMode : UserMode {

		/// <summary>
		/// Creates a new instance of the <see cref="NetworkOperatorMode"/> class.
		/// </summary>
		public NetworkOperatorMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="NetworkOperatorMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public NetworkOperatorMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "o";
			}
		}

	}
}
