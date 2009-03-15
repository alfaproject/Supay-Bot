using System;
using System.Collections.Specialized;
using System.Globalization;

namespace BigSister.Irc.Messages.Modes {
  /// <summary>
  ///   ChannelModesCreator parses, builds, and writes the modes used by the <see cref="ChannelModeMessage"/> class. </summary>
  class ChannelModesCreator {

    /// <summary>
    /// Creates a new instance of the <see cref="ChannelModesCreator"/> class.
    /// </summary>
    public ChannelModesCreator() {
    }

    /// <summary>
    ///   A <see cref="BigSister.Irc.ServerSupport"/> instance is required in order to parse non-standard modes. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public BigSister.Irc.ServerSupport ServerSupport {
      get {
        return this.serverSupports;
      }
      set {
        if (value == null) {
          throw new ArgumentNullException("value");
        }
        this.serverSupports = value;
      }
    }
    private BigSister.Irc.ServerSupport serverSupports = new BigSister.Irc.ServerSupport();

    #region Parsing

    /// <summary>
    /// Loads the given mode data into this <see cref="ChannelModesCreator"/>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public void Parse(string modeChanges, StringCollection modeArguments) {
      if (string.IsNullOrEmpty(modeChanges)) {
        return;
      }
      if (modeArguments == null) {
        modeArguments = new StringCollection();
      }

      this.modes.Clear();
      ModeAction currentAction = ModeAction.Add;
      int argIndex = 0;
      foreach (char c in modeChanges) {
        if (ModeAction.IsDefined(c.ToString())) {
          currentAction = ModeAction.Parse(c.ToString());
        } else {
          // PONDER This probably won't correctly parse incorrect mode messages, should I?
          switch (c) {
            case 'a':
              this.modes.Add(new AnonymousMode(currentAction));
              break;
            case 'b':
              BanMode banMode = new BanMode(currentAction, new User(modeArguments[argIndex]));
              argIndex++;
              this.modes.Add(banMode);
              break;
            case 'e':
              BanExceptionMode banExceptionMode = new BanExceptionMode(currentAction, new User(modeArguments[argIndex]));
              argIndex++;
              this.modes.Add(banExceptionMode);
              break;
            case 'h':
              HalfOpMode halfOpMode = new HalfOpMode(currentAction, modeArguments[argIndex]);
              argIndex++;
              this.modes.Add(halfOpMode);
              break;
            case 'I':
              InvitationExceptionMode invitationExceptionMode = new InvitationExceptionMode(currentAction, new User(modeArguments[argIndex]));
              argIndex++;
              this.modes.Add(invitationExceptionMode);
              break;
            case 'i':
              InviteOnlyMode inviteOnlyMode = new InviteOnlyMode(currentAction);
              this.modes.Add(inviteOnlyMode);
              break;
            case 'k':
              KeyMode keyMode = new KeyMode(currentAction);
              if (currentAction == ModeAction.Add) {
                keyMode.Password = modeArguments[argIndex];
                argIndex++;
              }
              this.modes.Add(keyMode);
              break;
            case 'l':
              LimitMode limitMode = new LimitMode(currentAction);
              if (currentAction == ModeAction.Add) {
                limitMode.UserLimit = Convert.ToInt32(modeArguments[argIndex], CultureInfo.InvariantCulture);
                argIndex++;
              }
              this.modes.Add(limitMode);
              break;
            case 'm':
              this.modes.Add(new ModeratedMode(currentAction));
              break;
            case 'n':
              this.modes.Add(new NoOutsideMessagesMode(currentAction));
              break;
            case 'O':
              CreatorMode creatorMode = new CreatorMode(currentAction, modeArguments[argIndex]);
              argIndex++;
              this.modes.Add(creatorMode);
              break;
            case 'o':
              OperatorMode operatorMode = new OperatorMode(currentAction, modeArguments[argIndex]);
              argIndex++;
              this.modes.Add(operatorMode);
              break;
            case 'p':
              this.modes.Add(new PrivateMode(currentAction));
              break;
            case 'q':
              this.modes.Add(new QuietMode(currentAction));
              break;
            case 's':
              this.modes.Add(new SecretMode(currentAction));
              break;
            case 'r':
              this.modes.Add(new ServerReopMode(currentAction));
              break;
            case 'R':
              this.modes.Add(new RegisteredNicksOnlyMode(currentAction));
              break;
            case 't':
              this.modes.Add(new TopicGuardedMode(currentAction));
              break;
            case 'v':
              VoiceMode voiceMode = new VoiceMode(currentAction, modeArguments[argIndex]);
              argIndex++;
              this.modes.Add(voiceMode);
              break;
            default:
              string unknownMode = c.ToString();
              if (this.serverSupports.ModesWithParameters.Contains(unknownMode) || (this.serverSupports.ModesWithParametersWhenSet.Contains(unknownMode) && currentAction == ModeAction.Add)) {
                // I want to yank a parameter	
                this.modes.Add(new UnknownChannelMode(currentAction, unknownMode, modeArguments[argIndex]));
                argIndex++;
              } else {
                // I don't
                this.modes.Add(new UnknownChannelMode(currentAction, unknownMode));
              }
              break;
          }
        }
      }
      this.CollapseModes();
    }

    /// <summary>
    ///   Loads the given mode data into this <see cref="ChannelModesCreator"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public void Parse(ChannelModeMessage msg) {
      if (msg == null) {
        return;
      }
      this.Parse(msg.ModeChanges, msg.ModeArguments);
    }

    /// <summary>
    ///   Loads the given mode data into this <see cref="ChannelModesCreator"/>. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public void Parse(string modeChanges) {
      if (string.IsNullOrEmpty(modeChanges)) {
        return;
      }
      this.Parse(modeChanges, new StringCollection());
    }

    #endregion

    /// <summary>
    ///   Removes redundant or overridden modes from the modes collection. </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    private void CollapseModes() {
      //TODO Implement CollapseModes
    }


    /// <summary>
    /// Applies the current modes to the given <see cref="ChannelModeMessage"/>.
    /// </summary>
    /// <param name="msg">The message to be altered.</param>
    public virtual void ApplyTo(ChannelModeMessage msg) {
      if (msg == null) {
        return;
      }
      msg.ModeChanges = string.Empty;
      msg.ModeArguments.Clear();
      if (this.modes.Count > 0) {
        // The first one always adds its mode
        ChannelMode currentMode = this.modes[0];
        ModeAction currentAction = currentMode.Action;
        currentMode.ApplyTo(msg, true);
        // The rest compare to the current
        for (int i = 1; i < this.modes.Count; i++) {
          currentMode = this.modes[i];
          currentMode.ApplyTo(msg, currentAction != currentMode.Action);
          currentAction = currentMode.Action;
        }
      }
    }


    /// <summary>
    /// Gets the collection of modes parsed or to be applied.
    /// </summary>
    public virtual ChannelModeCollection Modes {
      get {
        return modes;
      }
    }

    private ChannelModeCollection modes = new ChannelModeCollection();

  }
}