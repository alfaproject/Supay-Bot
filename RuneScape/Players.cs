using System;
using System.Collections.Generic;

using System.Data.SQLite;

namespace BigSister {
  class Players : List<Player> {

    public Players()
      : base() {
    }

    public Players(string clan) {
      SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn, lastupdate FROM players WHERE clan LIKE '%" + clan + "%'");
      while (rs.Read())
        this.Add(new Player(rs.GetString(0), rs.GetString(1).ToDateTime()));
      rs.Close();
    }

    public Players(string clan, DateTime day) {
      SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn FROM players WHERE clan LIKE '%" + clan + "%'");
      while (rs.Read()) {
        Player p = new Player(rs.GetString(0), day);
        if (p.Ranked)
          this.Add(p);
      }
      rs.Close();
    }

    // performance constructor
    public Players(string clan, DateTime firstDay, DateTime lastDay) {
      SQLiteDataReader rs = Database.ExecuteReader("SELECT rsn FROM players WHERE clan LIKE '%" + clan + "%'");
      while (rs.Read()) {
        Player playerBegin = new Player(rs.GetString(0), firstDay);
        if (playerBegin.Ranked) {
          Player playerEnd = new Player(rs.GetString(0), lastDay);
          if (playerEnd.Ranked) {
            for (int i = 0; i < playerEnd.Skills.Count; i++)
              playerEnd.Skills[i] -= playerBegin.Skills[i];
            this.Add(playerEnd);
          }
        }
      }
      rs.Close();
    }

    public Player Find(string rsn) {
      return this.Find(p => p.Name == rsn);
    }

    public bool Contains(string rsn) {
      foreach (Player p in this)
        if (p.Name == rsn)
          return true;
      return false;
    }

    public int IndexOf(string rsn) {
      for (int i = 0; i < this.Count; i++)
        if (this[i].Name == rsn)
          return i;
      return -1;
    }

    public void SortBySkill(string skill, bool byexp) {
      this.RemoveAll(p => !p.Ranked);

      if (byexp)
        this.Sort((p1, p2) => -p1.Skills[skill].Exp.CompareTo(p2.Skills[skill].Exp));
      else
        this.Sort((p1, p2) => p1.Skills[skill].CompareTo(p2.Skills[skill]));
    }

    public void SortByMinigame(string minigame) {
      this.Sort((p1, p2) => p1.Minigames[minigame].CompareTo(p2.Minigames[minigame]));
    }

  } //class Players
} //namespace BigSister