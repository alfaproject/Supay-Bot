using System;

namespace BigSister.Irc.Messages.Modes {

 	/// <summary>
 	/// When thi mode is set, 
 	/// new members are only accepted if their mask matches Invite-list (See <see cref="InvitationExceptionMode"/>) 
 	/// or they have been invited by a channel operator.
 	/// </summary>
	public class InviteOnlyMode : ChannelMode {

		/// <summary>
		/// Creates a new instance of the <see cref="InviteOnlyMode"/> class.
		/// </summary>
		public InviteOnlyMode() {
		}

		/// <summary>
		/// Creates a new instance of the <see cref="InviteOnlyMode"/> class with the given <see cref="ModeAction"/>.
		/// </summary>
		public InviteOnlyMode(ModeAction action) {
			this.Action = action;
		}

		/// <summary>
		/// Gets the irc string representation of the mode being changed or applied.
		/// </summary>
		protected override string Symbol {
			get {
				return "i";
			}
		}
	}
}
