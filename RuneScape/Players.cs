using System;
using System.Collections.Generic;

using System.Data.SQLite;
using System.Linq;

namespace Supay.Bot {
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

    public Player Find(string name) {
      return Find(p => p.Name.EqualsI(name));
    }

    public bool Contains(string name) {
      return this.Any(p => p.Name.EqualsI(name));
    }

    public int IndexOf(string name) {
      for (int i = 0; i < Count; i++) {
        if (this[i].Name.EqualsI(name)) {
          return i;
        }
      }
      return -1;
    }

    public void SortBySkill(string skill, bool byExp) {
      RemoveAll(p => !p.Ranked);
      Sort((p1, p2) => {
        Skill s1 = p1.Skills[skill];
        Skill s2 = p2.Skills[skill];
        if (s1.Level == s2.Level && s1.Exp == s2.Exp && s1.Rank > 0 && s2.Rank > 0) {
          return s1.Rank.CompareTo(s2.Rank);
        }
        return byExp && p1.Skills[skill].Exp != p2.Skills[skill].Exp ?
        -p1.Skills[skill].Exp.CompareTo(p2.Skills[skill].Exp) :
        p1.Skills[skill].CompareTo(p2.Skills[skill]);
      });
    }

    public void SortByMinigame(string minigame) {
      this.RemoveAll(p => !p.Ranked);
      this.Sort((p1, p2) => p1.Minigames[minigame].CompareTo(p2.Minigames[minigame]));
    }

  } //class Players
} //namespace Supay.Bot