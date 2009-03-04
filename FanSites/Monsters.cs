using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BigSister
{
  //http://www.tip.it/runescape/index.php?rs2monster&levels=All 
  public class Monsters : Dictionary<string, Monster>
  {
    //public void SaveDB() {
    //  StreamWriter Outputfile = new StreamWriter("Data\\Monsters.txt", false);
    //  foreach (Monster M in this.Values) {
    //    Outputfile.WriteLine(M.Name + " {");

    //    Outputfile.WriteLine("ID: " + M.ID + ";");
    //    Outputfile.WriteLine("LastUpdate: " + M.LastUpdate.ToString + ";");
    //    if (M.Race.Length > 0)
    //      Outputfile.WriteLine("Race: " + M.Race + ";");
    //    if (M.Level > 0)
    //      Outputfile.WriteLine("Level: " + M.Level + ";");
    //    if (M.Hits > 0)
    //      Outputfile.WriteLine("Hits: " + M.Hits + ";");

    //    if (M.Aggressive)
    //      Outputfile.WriteLine("Aggressive: Yes;");
    //    if (M.Retreats)
    //      Outputfile.WriteLine("Retreats: Yes;");
    //    if (M.Quest)
    //      Outputfile.WriteLine("Quest: Yes;");
    //    if (M.Members)
    //      Outputfile.WriteLine("Members: Yes;");
    //    if (M.Poisonous)
    //      Outputfile.WriteLine("Poisonous: Yes;");

    //    if (M.Habitat.Count > 0)
    //      Outputfile.WriteLine("Habitat: " + string.Join("|", M.Habitat.ToArray()) + ";");
    //    if (M.Notes.Count > 0)
    //      Outputfile.WriteLine("Notes: " + string.Join("|", M.Notes.ToArray()) + ";");

    //    if (M.Drops_Gold.Length > 0)
    //      Outputfile.WriteLine("Drops_Gold: " + M.Drops_Gold + ";");
    //    if (M.Drops_100.Length > 0)
    //      Outputfile.WriteLine("Drops_100: " + M.Drops_100 + ";");
    //    if (M.Drops_Weapon.Length > 0)
    //      Outputfile.WriteLine("Drops_Weapon: " + M.Drops_Weapon + ";");
    //    if (M.Drops_Armour.Length > 0)
    //      Outputfile.WriteLine("Drops_Armour: " + M.Drops_Armour + ";");
    //    if (M.Drops_RunesArrows.Length > 0)
    //      Outputfile.WriteLine("Drops_RunesArrows: " + M.Drops_RunesArrows + ";");
    //    if (M.Drops_Misc.Length > 0)
    //      Outputfile.WriteLine("Drops_Misc: " + M.Drops_Misc + ";");

    //    if (M.DropsP2P_Gold.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_Gold: " + M.DropsP2P_Gold + ";");
    //    if (M.DropsP2P_100.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_100: " + M.DropsP2P_100 + ";");
    //    if (M.DropsP2P_Weapon.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_Weapon: " + M.DropsP2P_Weapon + ";");
    //    if (M.DropsP2P_Armour.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_Armour: " + M.DropsP2P_Armour + ";");
    //    if (M.DropsP2P_RunesArrows.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_RunesArrows: " + M.DropsP2P_RunesArrows + ";");
    //    if (M.DropsP2P_Misc.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_Misc: " + M.DropsP2P_Misc + ";");
    //    if (M.DropsP2P_Seeds.Length > 0)
    //      Outputfile.WriteLine("DropsP2P_Seeds: " + M.DropsP2P_Seeds + ";");

    //    Outputfile.WriteLine("}");
    //  }
    //  Outputfile.Close();
    //}

    //public void LoadDB() {
    //  try {
    //    StreamReader InputFile = new StreamReader("Data\\Monsters.txt");
    //    string InputString = InputFile.ReadToEnd();
    //    InputFile.Close();

    //    foreach (Match M in Regex.Matches(InputString, "([^{]+) {([^}]+)}", RegexOptions.Singleline)) {
    //      Monster NewMonster = new Monster(M.Groups(1).Value.Trim());
    //      foreach (Match Var in Regex.Matches(M.Groups(2).Value, "(\\w+): ([^;]+);")) {
    //        switch (Var.Groups(1).Value) {
    //          case "ID":
    //            NewMonster.ID = int.Parse(Var.Groups(2).Value);
    //            break;
    //          case "LastUpdate":
    //            NewMonster.LastUpdate = System.DateTime.Parse(Var.Groups(2).Value);
    //            break;
    //          case "Race":
    //            NewMonster.Race = Var.Groups(2).Value;
    //            break;
    //          case "Level":
    //            NewMonster.Level = int.Parse(Var.Groups(2).Value);
    //            break;
    //          case "Hits":
    //            NewMonster.Hits = int.Parse(Var.Groups(2).Value);
    //            break;

    //          case "Aggresive":
    //            NewMonster.Aggressive = true;
    //            break;
    //          case "Retreats":
    //            NewMonster.Retreats = true;
    //            break;
    //          case "Quest":
    //            NewMonster.Quest = true;
    //            break;
    //          case "Members":
    //            NewMonster.Members = true;
    //            break;
    //          case "Poisonous":
    //            NewMonster.Poisonous = true;
    //            break;

    //          case "Habitat":
    //            NewMonster.Habitat = new List<string>(Var.Groups(2).Value.Split('|'));
    //            break;
    //          case "Notes":
    //            NewMonster.Notes = new List<string>(Var.Groups(2).Value.Split('|'));
    //            break;

    //          case "Drops_Gold":
    //            NewMonster.Drops_Gold = Var.Groups(2).Value;
    //            break;
    //          case "Drops_100":
    //            NewMonster.Drops_100 = Var.Groups(2).Value;
    //            break;
    //          case "Drops_Weapon":
    //            NewMonster.Drops_Weapon = Var.Groups(2).Value;
    //            break;
    //          case "Drops_Armour":
    //            NewMonster.Drops_Armour = Var.Groups(2).Value;
    //            break;
    //          case "Drops_RunesArrows":
    //            NewMonster.Drops_RunesArrows = Var.Groups(2).Value;
    //            break;
    //          case "Drops_Misc":
    //            NewMonster.Drops_Misc = Var.Groups(2).Value;
    //            break;

    //          case "DropsP2P_Gold":
    //            NewMonster.DropsP2P_Gold = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_100":
    //            NewMonster.DropsP2P_100 = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_Weapon":
    //            NewMonster.DropsP2P_Weapon = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_Armour":
    //            NewMonster.DropsP2P_Armour = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_RunesArrows":
    //            NewMonster.DropsP2P_RunesArrows = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_Misc":
    //            NewMonster.DropsP2P_Misc = Var.Groups(2).Value;
    //            break;
    //          case "DropsP2P_Seeds":
    //            NewMonster.DropsP2P_Seeds = Var.Groups(2).Value;
    //            break;
    //        }
    //      }
    //      this.Add(NewMonster.Name.ToUpperInvariant(), NewMonster);
    //    }
    //  }
    //  catch (Exception ex) {
    //  }
    //}

    public List<Monster> SearchOnline(string Criteria) {
      List<Monster> ResultList = new List<Monster>();

      string MonsterSearchPage = new System.Net.WebClient().DownloadString("http://www.tip.it/runescape/index.php?rs2monster=&orderby=0&levels=All&race=0&keywords=" + Criteria);

      MatchCollection Results = Regex.Matches(MonsterSearchPage, "<td>(\\d+)</td>\\s+<td><a href=\"\\?rs2monster_id=(\\d+)\">([^<]+)</a>");
      if (Results.Count > 0) {
        foreach (Match M in Results) {
          if (this.ContainsKey(M.Groups[3].Value.ToUpperInvariant())) {
            ResultList.Add(this[M.Groups[3].Value.ToUpperInvariant()]);
          }
          else {
            Monster NewMonster = new Monster(int.Parse(M.Groups[2].Value));
            NewMonster.Name = M.Groups[3].Value;
            NewMonster.Level = int.Parse(M.Groups[1].Value);
            this.Add(M.Groups[3].Value.ToUpperInvariant(), NewMonster);

            ResultList.Add(NewMonster);
          }
        }
      }

      return ResultList;
    }
  }
}