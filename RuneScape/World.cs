using System;

namespace BigSister {
  class World : IComparable<World> {

    public int Number;
    public bool Member;
    public string Status; //Online, Offline, Full
    public string Activity;
    public int Players;
    public string Location;

    public bool LootShare;
    public bool QuickChat;
    public bool PVP;

    public World() {
    }

    #region IComparable<World> Members

    public int CompareTo(World other) {
      if (Players == other.Players)
        return 0;
      else if (Players > other.Players)
        return -1;
      else
        return 1;
    }

    #endregion

  }
}