using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BigSister {
  public class Monster {

    public int ID;
    public DateTime LastUpdate;

    public string Name = string.Empty;
    public string Race = string.Empty;
    public int Level;
    public int Hits;

    public bool Aggressive;
    public bool Retreats;
    public bool Quest;
    public bool Members;
    public bool Poisonous;

    public List<string> Habitat = new List<string>();
    public List<string> Notes = new List<string>();

    // Drops related 
    public string Drops_Gold = string.Empty;
    public string DropsF2P_100 = string.Empty;
    public string DropsF2P_Weapon = string.Empty;
    public string DropsF2P_Armour = string.Empty;
    public string DropsF2P_RunesArrows = string.Empty;
    public string DropsF2P_Misc = string.Empty;

    public string Drops_100 = string.Empty;
    public string Drops_Weapon = string.Empty;
    public string Drops_Armour = string.Empty;
    public string Drops_RunesArrows = string.Empty;
    public string Drops_Misc = string.Empty;
    public string Drops_Seeds = string.Empty;

    public void Update() {
      try {
        string MonsterPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2monster_id=" + ID);

        Match TempMatch;

        // Get name 
        TempMatch = Regex.Match(MonsterPage, "<b>Name:</b>\\s+<td width=\"20%\">([^<]+)</td>");
        if (TempMatch.Success)
          this.Name = TempMatch.Groups[1].Value;

        // Get race 
        TempMatch = Regex.Match(MonsterPage, "<b>Race:</b></td>\\s+<td width=\"20%\">([^<]+)</td>");
        if (TempMatch.Success)
          this.Race = TempMatch.Groups[1].Value;

        // Get level 
        TempMatch = Regex.Match(MonsterPage, "<b>Level:</b></td>\\s+<td width=\"20%\">([^<]+)</td>");
        if (TempMatch.Success)
          int.TryParse(TempMatch.Groups[1].Value, out this.Level);

        // Get hitpoints 
        TempMatch = Regex.Match(MonsterPage, "<b>Hits:</b></td>\\s+<td width=\"20%\">([^<]+)</td>");
        if (TempMatch.Success)
          int.TryParse(TempMatch.Groups[1].Value, out this.Hits);

        // Get aggressive 
        TempMatch = Regex.Match(MonsterPage, "<b>Aggressive\\?</b>&nbsp;&nbsp;(Yes|No)</td>");
        if (TempMatch.Success)
          this.Aggressive = (TempMatch.Groups[1].Value == "Yes");

        // Get retreats 
        TempMatch = Regex.Match(MonsterPage, "<b>Retreats\\?</b>&nbsp;&nbsp;(Yes|No)</td>");
        if (TempMatch.Success)
          this.Retreats = (TempMatch.Groups[1].Value == "Yes");

        // Get quest 
        TempMatch = Regex.Match(MonsterPage, "<b>Quest\\?</b>&nbsp;&nbsp;(Yes|No)</td>");
        if (TempMatch.Success)
          this.Quest = (TempMatch.Groups[1].Value == "Yes");

        // Get members 
        TempMatch = Regex.Match(MonsterPage, "<b>Members\\?</b>&nbsp;&nbsp;(Yes|No)</td>");
        if (TempMatch.Success)
          this.Members = (TempMatch.Groups[1].Value == "Yes");

        // Get poisonous 
        TempMatch = Regex.Match(MonsterPage, "<b>Poisonous\\?</b>&nbsp;&nbsp;(Yes|No)</td>");
        if (TempMatch.Success)
          this.Poisonous = (TempMatch.Groups[1].Value == "Yes");

        // Get habitat 
        TempMatch = Regex.Match(MonsterPage, "<b>Habitat:</b></td>\\s+<td\\salign=\"left\"><ul>(.*?)</ul>", RegexOptions.Singleline);
        if (TempMatch.Success) {
          foreach (string Hab in Regex.Split(Regex.Replace(TempMatch.Groups[1].Value, "<li>|<a[^>]+>|</a>", string.Empty), "</li>|<br />")) {
            if (Hab.Trim().Length > 0)
              this.Habitat.Add(Hab.Trim());
          }
        }

        // Get notes 
        TempMatch = Regex.Match(MonsterPage, "<b>Notes:</b></td>\\s+<td\\salign=\"left\">(.*?)</td>", RegexOptions.Singleline);
        if (TempMatch.Success) {
          foreach (string Note in Regex.Split(TempMatch.Groups[1].Value, "<br />|<br")) {
            string note = Regex.Replace(Note.Trim(), "<[^>]+>", string.Empty);
            if (note.Length > 0)
              this.Notes.Add(note);
          }
        }

        // Get F2P drops 
        TempMatch = Regex.Match(MonsterPage, "<b>F2P/P2P drops:</b></td>\\s+<td\\salign=\"left\"><ul>(.*?)</td>", RegexOptions.Singleline);
        if (TempMatch.Success) {
          string D = Regex.Replace(TempMatch.Groups[1].Value, "\\s+", " ", RegexOptions.Singleline);
          Match Drop;
          Drop = Regex.Match(D, "<b>Gold:</b>([^<]+)");
          if (Drop.Success)
            Drops_Gold = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>100%:</b>([^<]+)");
          if (Drop.Success)
            DropsF2P_100 = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Weapon:</b>([^<]+)");
          if (Drop.Success)
            DropsF2P_Weapon = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Armour:</b>([^<]+)");
          if (Drop.Success)
            DropsF2P_Armour = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Runes/Arrows:</b>([^<]+)");
          if (Drop.Success)
            DropsF2P_RunesArrows = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Misc:</b>([^<]+)");
          if (Drop.Success)
            DropsF2P_Misc = Drop.Groups[1].Value.Trim();
        }

        // Get P2P drops 
        TempMatch = Regex.Match(MonsterPage, "<b>P2P Only drops:</b></td>\\s+<td\\salign=\"left\"><ul>(.*?)</td>", RegexOptions.Singleline);
        if (TempMatch.Success) {
          string D = Regex.Replace(TempMatch.Groups[1].Value, "\\s+", " ", RegexOptions.Singleline);
          Match Drop;
          Drop = Regex.Match(D, "<b>Gold:</b>([^<]+)");
          if (Drop.Success)
            Drops_Gold = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>100%:</b>([^<]+)");
          if (Drop.Success)
            Drops_100 = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Weapon:</b>([^<]+)");
          if (Drop.Success)
            Drops_Weapon = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Armour:</b>([^<]+)");
          if (Drop.Success)
            Drops_Armour = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Runes/Arrows:</b>([^<]+)");
          if (Drop.Success)
            Drops_RunesArrows = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Misc:</b>([^<]+)");
          if (Drop.Success)
            Drops_Misc = Drop.Groups[1].Value.Trim();
          Drop = Regex.Match(D, "<b>Seeds:</b>([^<]+)");
          if (Drop.Success)
            Drops_Seeds = Drop.Groups[1].Value.Trim();
        }

        LastUpdate = DateTime.Now;
      } catch {
        // An error ocurred
      }
    }

    public Monster(string name) {
      this.Name = name;
    }

    public Monster(int id) {
      this.ID = id;
    }

  }
}