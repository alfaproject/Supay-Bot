using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using System.Data.SQLite;

namespace BigSister {
  public class Player {
    private int _id;
    private string _name;
    private string _clan = null;
    private string _combatclass;
    private DateTime _lastupdate;
    private bool _ranked;

    private Skills _skills;
    private Minigames _minigames;

    public int Id {
      get {
        return _id;
      }
    }

    public DateTime LastUpdate {
      get {
        return _lastupdate;
      }
      set {
        _lastupdate = value;
      }
    }

    public string Name {
      get {
        return _name;
      }
    }

    public string Clan {
      get {
        return _clan;
      }
    }

    public string CombatClass {
      get {
        return _combatclass;
      }
    }

    public bool Ranked {
      get {
        return _ranked;
      }
      set {
        _ranked = value;
      }
    }

    public Skills Skills {
      get {
        return _skills;
      }
    }

    public Dictionary<string, Minigame> Minigames {
      get {
        return _minigames;
      }
    }

    private void _GuessMissingSkills() {
      // Check if Overall has exp.
      if (_skills[Skill.OVER].Exp == -1) {
        // Fix Hitpoints
        if (_skills[Skill.HITP].Exp == -1)
          _skills[Skill.HITP].Exp = RSUtil.Lvl2Exp(10);

        // Fix Overall and all other unranked skills 
        _skills[Skill.OVER].Exp = 0;
        _skills[Skill.OVER].Level = 0;
        for (int i = 1; i < _skills.Count; i++) {
          if (_skills[i].Exp == -1)
            _skills[i].Exp = 0;
          else
            _skills[Skill.OVER].Exp += _skills[i].Exp;
          _skills[Skill.OVER].Level += _skills[i].Level;
        }
      } else {
        // Fix Hitpoints
        if (_skills[Skill.HITP].Exp == -1)
          _skills[Skill.HITP].Level = 10;

        // Find missing skills and calculate total known levels 
        int rankedLevels = 0;
        for (int i = 1; i < _skills.Count; i++)
          if (_skills[i].Exp != -1 || _skills[i].Name == Skill.HITP)
            rankedLevels += _skills[i].Level;

        // Fill unranked skills with some estimate xp and level 
        while (_skills[Skill.OVER].Level - rankedLevels != 0) {
          for (int i = 1; i < _skills.Count; i++) {
            if (_skills[i].Exp == -1) {
              if (_skills[i].Level == -1)
                _skills[i].Level = 1;
              else
                _skills[i].Level++;
              rankedLevels++;
            }
            if (_skills[Skill.OVER].Level - rankedLevels == 0)
              break;
          }
        }

        foreach (Skill s in _skills.Values)
          if (s.Exp == -1)
            s.Exp = RSUtil.Lvl2Exp(s.Level);
      }
    }

    private void _CreateCombatSkill() {
      int CmbLevel = RSUtil.CalculateCombat(_skills["Attack"].Level, _skills["Strength"].Level, _skills["Defence"].Level, _skills["Hitpoints"].Level, _skills["Ranged"].Level, _skills[Skill.PRAY].Level, _skills["Magic"].Level, _skills[Skill.SUMM].Level, out _combatclass);
      int CmbExp = _skills["Attack"].Exp + _skills["Strength"].Exp + _skills["Defence"].Exp + _skills["Hitpoints"].Exp + _skills["Ranged"].Exp + _skills[Skill.PRAY].Exp + _skills["Magic"].Exp + _skills[Skill.SUMM].Exp;
      _skills.Add("Combat", new Skill("Combat", -1, CmbLevel, CmbExp));
    }

    public void SaveToDB(string s_date) {
      _id = Convert.ToInt32(DataBase.GetValue("players", "id", "rsn='" + _name + "'"));

      if (this.Ranked) {
        DataBase.Insert("tracker", "pid", _id.ToString(),
                                   "date", s_date,
                                   "overall_level", _skills[0].Level.ToString(), "overall_xp", _skills[0].Exp.ToString(), "overall_rank", _skills[0].Rank.ToString(),
                                   "attack_xp", _skills[1].Exp.ToString(), "attack_rank", _skills[1].Rank.ToString(),
                                   "defence_xp", _skills[2].Exp.ToString(), "defence_rank", _skills[2].Rank.ToString(),
                                   "strength_xp", _skills[3].Exp.ToString(), "strength_rank", _skills[3].Rank.ToString(),
                                   "hitpoints_xp", _skills[4].Exp.ToString(), "hitpoints_rank", _skills[4].Rank.ToString(),
                                   "range_xp", _skills[5].Exp.ToString(), "range_rank", _skills[5].Rank.ToString(),
                                   "prayer_xp", _skills[6].Exp.ToString(), "prayer_rank", _skills[6].Rank.ToString(),
                                   "magic_xp", _skills[7].Exp.ToString(), "magic_rank", _skills[7].Rank.ToString(),
                                   "cook_xp", _skills[8].Exp.ToString(), "cook_rank", _skills[8].Rank.ToString(),
                                   "woodcut_xp", _skills[9].Exp.ToString(), "woodcut_rank", _skills[9].Rank.ToString(),
                                   "fletch_xp", _skills[10].Exp.ToString(), "fletch_rank", _skills[10].Rank.ToString(),
                                   "fish_xp", _skills[11].Exp.ToString(), "fish_rank", _skills[11].Rank.ToString(),
                                   "firemake_xp", _skills[12].Exp.ToString(), "firemake_rank", _skills[12].Rank.ToString(),
                                   "craft_xp", _skills[13].Exp.ToString(), "craft_rank", _skills[13].Rank.ToString(),
                                   "smith_xp", _skills[14].Exp.ToString(), "smith_rank", _skills[14].Rank.ToString(),
                                   "mine_xp", _skills[15].Exp.ToString(), "mine_rank", _skills[15].Rank.ToString(),
                                   "herb_xp", _skills[16].Exp.ToString(), "herb_rank", _skills[16].Rank.ToString(),
                                   "agility_xp", _skills[17].Exp.ToString(), "agility_rank", _skills[17].Rank.ToString(),
                                   "thief_xp", _skills[18].Exp.ToString(), "thief_rank", _skills[18].Rank.ToString(),
                                   "slay_xp", _skills[19].Exp.ToString(), "slay_rank", _skills[19].Rank.ToString(),
                                   "farm_xp", _skills[20].Exp.ToString(), "farm_rank", _skills[20].Rank.ToString(),
                                   "runecraft_xp", _skills[21].Exp.ToString(), "runecraft_rank", _skills[21].Rank.ToString(),
                                   "hunt_xp", _skills[22].Exp.ToString(), "hunt_rank", _skills[22].Rank.ToString(),
                                   "construction_xp", _skills[23].Exp.ToString(), "construction_rank", _skills[23].Rank.ToString(),
                                   "summ_xp", _skills[24].Exp.ToString(), "summ_rank", _skills[24].Rank.ToString(),
                                   "dt_rank", _minigames[Minigame.DUEL].Rank.ToString(), "dt_score", _minigames[Minigame.DUEL].Score.ToString(),
                                   "bh_rank", _minigames[Minigame.BOUN].Rank.ToString(), "bh_score", _minigames[Minigame.BOUN].Score.ToString(),
                                   "bhr_rank", _minigames[Minigame.ROGU].Rank.ToString(), "bhr_score", _minigames[Minigame.ROGU].Score.ToString(),
                                   "fist_rank", _minigames[Minigame.FIST].Rank.ToString(), "fist_score", _minigames[Minigame.FIST].Score.ToString());

        DataBase.Update("players", "id=" + _id, "lastupdate", s_date);
      }
    }

    // Constructor that retrieves player data from RuneScript tracker
    public Player(string rsn, int time) {
      time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - time;
      try {
        // Get the tracker SOAP for this RSN
        RScript.RScriptLookupPortTypeClient RScriptClient = new BigSister.RScript.RScriptLookupPortTypeClient();
        RScript.skills RScriptSkills = RScriptClient.trackGetTimeAll(rsn, time);

        // Initialize variables
        _skills = new Skills();
        _skills.Add(Skill.OVER, new Skill(Skill.OVER, RScriptSkills.overall.rank, RScriptSkills.overall.level, RScriptSkills.overall.exp));
        _skills.Add(Skill.ATTA, new Skill(Skill.ATTA, RScriptSkills.attack.rank, RScriptSkills.attack.exp));
        _skills.Add(Skill.DEFE, new Skill(Skill.DEFE, RScriptSkills.defence.rank, RScriptSkills.defence.exp));
        _skills.Add(Skill.STRE, new Skill(Skill.STRE, RScriptSkills.strength.rank, RScriptSkills.strength.exp));
        _skills.Add(Skill.HITP, new Skill(Skill.HITP, RScriptSkills.hitpoints.rank, RScriptSkills.hitpoints.exp));
        _skills.Add(Skill.RANG, new Skill(Skill.RANG, RScriptSkills.ranged.rank, RScriptSkills.ranged.exp));
        _skills.Add(Skill.PRAY, new Skill(Skill.PRAY, RScriptSkills.prayer.rank, RScriptSkills.prayer.exp));
        _skills.Add(Skill.MAGI, new Skill(Skill.MAGI, RScriptSkills.magic.rank, RScriptSkills.magic.exp));
        _skills.Add(Skill.COOK, new Skill(Skill.COOK, RScriptSkills.cooking.rank, RScriptSkills.cooking.exp));
        _skills.Add(Skill.WOOD, new Skill(Skill.WOOD, RScriptSkills.woodcutting.rank, RScriptSkills.woodcutting.exp));
        _skills.Add(Skill.FLET, new Skill(Skill.FLET, RScriptSkills.fletching.rank, RScriptSkills.fletching.exp));
        _skills.Add(Skill.FISH, new Skill(Skill.FISH, RScriptSkills.fishing.rank, RScriptSkills.fishing.exp));
        _skills.Add(Skill.FIRE, new Skill(Skill.FIRE, RScriptSkills.firemaking.rank, RScriptSkills.firemaking.exp));
        _skills.Add(Skill.CRAF, new Skill(Skill.CRAF, RScriptSkills.crafting.rank, RScriptSkills.crafting.exp));
        _skills.Add(Skill.SMIT, new Skill(Skill.SMIT, RScriptSkills.smithing.rank, RScriptSkills.smithing.exp));
        _skills.Add(Skill.MINI, new Skill(Skill.MINI, RScriptSkills.mining.rank, RScriptSkills.mining.exp));
        _skills.Add(Skill.HERB, new Skill(Skill.HERB, RScriptSkills.herblore.rank, RScriptSkills.herblore.exp));
        _skills.Add(Skill.AGIL, new Skill(Skill.AGIL, RScriptSkills.agility.rank, RScriptSkills.agility.exp));
        _skills.Add(Skill.THIE, new Skill(Skill.THIE, RScriptSkills.thieving.rank, RScriptSkills.thieving.exp));
        _skills.Add(Skill.SLAY, new Skill(Skill.SLAY, RScriptSkills.slayer.rank, RScriptSkills.slayer.exp));
        _skills.Add(Skill.FARM, new Skill(Skill.FARM, RScriptSkills.farming.rank, RScriptSkills.farming.exp));
        _skills.Add(Skill.RUNE, new Skill(Skill.RUNE, RScriptSkills.runecraft.rank, RScriptSkills.runecraft.exp));
        _skills.Add(Skill.HUNT, new Skill(Skill.HUNT, RScriptSkills.hunter.rank, RScriptSkills.hunter.exp));
        _skills.Add(Skill.CONS, new Skill(Skill.CONS, RScriptSkills.construction.rank, RScriptSkills.construction.exp));
        _skills.Add(Skill.SUMM, new Skill(Skill.SUMM, RScriptSkills.summoning.rank, RScriptSkills.summoning.exp));
        _minigames = new Minigames();
        _ranked = true;

        // Make it compatible with RuneScape
        foreach (Skill s in _skills.Values) {
          if (s.Rank == 0) {
            s.Level = -1;
            s.Exp = -1;
            s.Rank = -1;
          }
        }

        // Try to guess missing skills and update our skill list
        _GuessMissingSkills();

        // Create combat skill and update combat class
        _CreateCombatSkill();
      } catch {
        _ranked = false;
      }
    }

    // Constructor that retrieves player data from Database
    public Player(string rsn, DateTime day) {
      _name = rsn;
      _lastupdate = day;

      try {
        // Query database
        SQLiteDataReader rs = DataBase.ExecuteReader("SELECT tracker.* FROM tracker INNER JOIN players ON tracker.pid=players.id WHERE players.rsn='" + _name + "' AND tracker.date='" + day.ToString("yyyyMMdd") + "';");
        if (rs.Read()) {
          // Initialize variables
          _id = Convert.ToInt32(rs["pid"]);
          _skills = new Skills();
          _minigames = new Minigames();
          _ranked = true;

          _skills.Add(Skill.OVER, new Skill(Skill.OVER, Convert.ToInt32(rs["overall_rank"]), Convert.ToInt32(rs["overall_level"]), Convert.ToInt32(rs["overall_xp"])));
          _skills.Add(Skill.ATTA, new Skill(Skill.ATTA, Convert.ToInt32(rs["attack_rank"]), Convert.ToInt32(rs["attack_xp"])));
          _skills.Add(Skill.DEFE, new Skill(Skill.DEFE, Convert.ToInt32(rs["defence_rank"]), Convert.ToInt32(rs["defence_xp"])));
          _skills.Add(Skill.STRE, new Skill(Skill.STRE, Convert.ToInt32(rs["strength_rank"]), Convert.ToInt32(rs["strength_xp"])));
          _skills.Add(Skill.HITP, new Skill(Skill.HITP, Convert.ToInt32(rs["hitpoints_rank"]), Convert.ToInt32(rs["hitpoints_xp"])));
          _skills.Add(Skill.RANG, new Skill(Skill.RANG, Convert.ToInt32(rs["range_rank"]), Convert.ToInt32(rs["range_xp"])));
          _skills.Add(Skill.PRAY, new Skill(Skill.PRAY, Convert.ToInt32(rs["prayer_rank"]), Convert.ToInt32(rs["prayer_xp"])));
          _skills.Add(Skill.MAGI, new Skill(Skill.MAGI, Convert.ToInt32(rs["magic_rank"]), Convert.ToInt32(rs["magic_xp"])));
          _skills.Add(Skill.COOK, new Skill(Skill.COOK, Convert.ToInt32(rs["cook_rank"]), Convert.ToInt32(rs["cook_xp"])));
          _skills.Add(Skill.WOOD, new Skill(Skill.WOOD, Convert.ToInt32(rs["woodcut_rank"]), Convert.ToInt32(rs["woodcut_xp"])));
          _skills.Add(Skill.FLET, new Skill(Skill.FLET, Convert.ToInt32(rs["fletch_rank"]), Convert.ToInt32(rs["fletch_xp"])));
          _skills.Add(Skill.FISH, new Skill(Skill.FISH, Convert.ToInt32(rs["fish_rank"]), Convert.ToInt32(rs["fish_xp"])));
          _skills.Add(Skill.FIRE, new Skill(Skill.FIRE, Convert.ToInt32(rs["firemake_rank"]), Convert.ToInt32(rs["firemake_xp"])));
          _skills.Add(Skill.CRAF, new Skill(Skill.CRAF, Convert.ToInt32(rs["craft_rank"]), Convert.ToInt32(rs["craft_xp"])));
          _skills.Add(Skill.SMIT, new Skill(Skill.SMIT, Convert.ToInt32(rs["smith_rank"]), Convert.ToInt32(rs["smith_xp"])));
          _skills.Add(Skill.MINI, new Skill(Skill.MINI, Convert.ToInt32(rs["mine_rank"]), Convert.ToInt32(rs["mine_xp"])));
          _skills.Add(Skill.HERB, new Skill(Skill.HERB, Convert.ToInt32(rs["herb_rank"]), Convert.ToInt32(rs["herb_xp"])));
          _skills.Add(Skill.AGIL, new Skill(Skill.AGIL, Convert.ToInt32(rs["agility_rank"]), Convert.ToInt32(rs["agility_xp"])));
          _skills.Add(Skill.THIE, new Skill(Skill.THIE, Convert.ToInt32(rs["thief_rank"]), Convert.ToInt32(rs["thief_xp"])));
          _skills.Add(Skill.SLAY, new Skill(Skill.SLAY, Convert.ToInt32(rs["slay_rank"]), Convert.ToInt32(rs["slay_xp"])));
          _skills.Add(Skill.FARM, new Skill(Skill.FARM, Convert.ToInt32(rs["farm_rank"]), Convert.ToInt32(rs["farm_xp"])));
          _skills.Add(Skill.RUNE, new Skill(Skill.RUNE, Convert.ToInt32(rs["runecraft_rank"]), Convert.ToInt32(rs["runecraft_xp"])));
          _skills.Add(Skill.HUNT, new Skill(Skill.HUNT, Convert.ToInt32(rs["hunt_rank"]), Convert.ToInt32(rs["hunt_xp"])));
          _skills.Add(Skill.CONS, new Skill(Skill.CONS, Convert.ToInt32(rs["construction_rank"]), Convert.ToInt32(rs["construction_xp"])));
          _skills.Add(Skill.SUMM, new Skill(Skill.SUMM, Convert.ToInt32(rs["summ_rank"]), Convert.ToInt32(rs["summ_xp"])));

          _minigames.Add(Minigame.DUEL, new Minigame(Minigame.DUEL, Convert.ToInt32(rs["dt_rank"]), Convert.ToInt32(rs["dt_score"])));
          _minigames.Add(Minigame.BOUN, new Minigame(Minigame.BOUN, Convert.ToInt32(rs["bh_rank"]), Convert.ToInt32(rs["bh_score"])));
          _minigames.Add(Minigame.ROGU, new Minigame(Minigame.ROGU, Convert.ToInt32(rs["bhr_rank"]), Convert.ToInt32(rs["bhr_score"])));
          _minigames.Add(Minigame.FIST, new Minigame(Minigame.FIST, Convert.ToInt32(rs["fist_rank"]), Convert.ToInt32(rs["fist_score"])));

          // Create combat skill and update combat class
          _CreateCombatSkill();
        } else {
          _ranked = false;
        }
        rs.Close();
      } catch (Exception) {
        _ranked = false;
      }
    }

    // Constructor that retrieves player data from RuneScape Hiscores 
    public Player(string rsn) {
      _name = rsn;

      try {
        // Get hiscores page for this RSN 
        WebClient WC = new WebClient();
        string HiscorePage = WC.DownloadString("http://hiscore.runescape.com/index_lite.ws?player=" + _name);

        // Update RuneScript tracker database
        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(_updateRuneScriptTracker), rsn);

        // Initialize variables 
        _skills = new Skills();
        _minigames = new Minigames();
        _ranked = true;

        // Parse Hiscores page for this player 
        string[] HiscoreList = HiscorePage.Split('\n');
        string[] HiscoreLine;
        for (int i = 0; i < HiscoreList.Length - 1; i++) {
          HiscoreLine = HiscoreList[i].Split(',');
          switch (HiscoreLine.Length) {
            case 3:
              // Skill
              string SkillName = Skill.IdToName(_skills.Count);
              _skills.Add(SkillName, new Skill(SkillName, int.Parse(HiscoreLine[0]), int.Parse(HiscoreLine[1]), int.Parse(HiscoreLine[2])));
              break;

            case 2:
              // Minigame
              string MinigameName = Minigame.IdToName(_minigames.Count);
              _minigames.Add(MinigameName, new Minigame(MinigameName, int.Parse(HiscoreLine[0]), int.Parse(HiscoreLine[1])));
              break;

            default:
              // Unknown Hiscore line
              break;
          }
        }

        // Try to guess missing skills and update our skill list 
        _GuessMissingSkills();

        // Create combat skill and update combat class 
        _CreateCombatSkill();
      } catch (Exception) {
        _ranked = false;
      }
    }

    private static void _updateRuneScriptTracker(Object rsn) {
      try {
        RScript.RScriptLookupPortTypeClient RScriptClient = new BigSister.RScript.RScriptLookupPortTypeClient();
        RScriptClient.trackUpdateUser((string)rsn);
      } catch {
      }
    }

  }
}