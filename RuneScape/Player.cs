using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Net;

namespace Supay.Bot {
  class Player {
    private string _name;
    private string _combatclass;
    private DateTime _lastupdate;
    private bool _ranked;

    private Skills _skills;
    private Minigames _minigames;

    public long Id {
      get;
      private set;
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
        // Fix Constitution
        if (_skills[Skill.HITP].Exp == -1)
          _skills[Skill.HITP].Exp = 10.ToExp();

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
        // Fix Constitution
        if (_skills[Skill.HITP].Exp == -1)
          _skills[Skill.HITP].Level = 10;

        // Find missing skills and calculate total known levels 
        int rankedLevels = 0;
        for (int i = 1; i < _skills.Count; i++)
          if (_skills[i].Exp != -1 || _skills[i].Name == Skill.HITP)
            rankedLevels += _skills[i].Level;

        // RuneScript hack
        if (_skills[Skill.OVER].Level == 0)
          _skills[Skill.OVER].Level = rankedLevels;

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
            s.Exp = s.Level.ToExp();
      }
    }

    private void _CreateCombatSkill() {
      _combatclass = Utils.CombatClass(_skills, false);
      int CmbLevel = Utils.CalculateCombat(_skills, false, false);
      int CmbExp = _skills[Skill.ATTA].Exp + _skills[Skill.STRE].Exp + _skills[Skill.DEFE].Exp + _skills[Skill.HITP].Exp + _skills[Skill.RANG].Exp + _skills[Skill.PRAY].Exp + _skills[Skill.MAGI].Exp + _skills[Skill.SUMM].Exp;
      _skills.Add(Skill.COMB, new Skill(Skill.COMB, -1, CmbLevel, CmbExp));
    }

    public void SaveToDB(string s_date) {
      Id = Database.Lookup<int>("id", "players", "rsn=@name", new[] { new SQLiteParameter("@name", _name) });

      if (this.Ranked) {
        Database.Insert("tracker", "pid", Id.ToStringI(),
                                   "date", s_date,
                                   "overall_level", _skills[0].Level.ToStringI(), "overall_xp", _skills[0].Exp.ToStringI(), "overall_rank", _skills[0].Rank.ToStringI(),
                                   "attack_xp", _skills[1].Exp.ToStringI(), "attack_rank", _skills[1].Rank.ToStringI(),
                                   "defence_xp", _skills[2].Exp.ToStringI(), "defence_rank", _skills[2].Rank.ToStringI(),
                                   "strength_xp", _skills[3].Exp.ToStringI(), "strength_rank", _skills[3].Rank.ToStringI(),
                                   "hitpoints_xp", _skills[4].Exp.ToStringI(), "hitpoints_rank", _skills[4].Rank.ToStringI(),
                                   "range_xp", _skills[5].Exp.ToStringI(), "range_rank", _skills[5].Rank.ToStringI(),
                                   "prayer_xp", _skills[6].Exp.ToStringI(), "prayer_rank", _skills[6].Rank.ToStringI(),
                                   "magic_xp", _skills[7].Exp.ToStringI(), "magic_rank", _skills[7].Rank.ToStringI(),
                                   "cook_xp", _skills[8].Exp.ToStringI(), "cook_rank", _skills[8].Rank.ToStringI(),
                                   "woodcut_xp", _skills[9].Exp.ToStringI(), "woodcut_rank", _skills[9].Rank.ToStringI(),
                                   "fletch_xp", _skills[10].Exp.ToStringI(), "fletch_rank", _skills[10].Rank.ToStringI(),
                                   "fish_xp", _skills[11].Exp.ToStringI(), "fish_rank", _skills[11].Rank.ToStringI(),
                                   "firemake_xp", _skills[12].Exp.ToStringI(), "firemake_rank", _skills[12].Rank.ToStringI(),
                                   "craft_xp", _skills[13].Exp.ToStringI(), "craft_rank", _skills[13].Rank.ToStringI(),
                                   "smith_xp", _skills[14].Exp.ToStringI(), "smith_rank", _skills[14].Rank.ToStringI(),
                                   "mine_xp", _skills[15].Exp.ToStringI(), "mine_rank", _skills[15].Rank.ToStringI(),
                                   "herb_xp", _skills[16].Exp.ToStringI(), "herb_rank", _skills[16].Rank.ToStringI(),
                                   "agility_xp", _skills[17].Exp.ToStringI(), "agility_rank", _skills[17].Rank.ToStringI(),
                                   "thief_xp", _skills[18].Exp.ToStringI(), "thief_rank", _skills[18].Rank.ToStringI(),
                                   "slay_xp", _skills[19].Exp.ToStringI(), "slay_rank", _skills[19].Rank.ToStringI(),
                                   "farm_xp", _skills[20].Exp.ToStringI(), "farm_rank", _skills[20].Rank.ToStringI(),
                                   "runecraft_xp", _skills[21].Exp.ToStringI(), "runecraft_rank", _skills[21].Rank.ToStringI(),
                                   "hunt_xp", _skills[22].Exp.ToStringI(), "hunt_rank", _skills[22].Rank.ToStringI(),
                                   "construction_xp", _skills[23].Exp.ToStringI(), "construction_rank", _skills[23].Rank.ToStringI(),
                                   "summ_xp", _skills[24].Exp.ToStringI(), "summ_rank", _skills[24].Rank.ToStringI(),
                                   "dt_rank", _minigames[Minigame.DUEL].Rank.ToStringI(), "dt_score", _minigames[Minigame.DUEL].Score.ToStringI(),
                                   "bh_rank", _minigames[Minigame.BOUN].Rank.ToStringI(), "bh_score", _minigames[Minigame.BOUN].Score.ToStringI(),
                                   "bhr_rank", _minigames[Minigame.ROGU].Rank.ToStringI(), "bhr_score", _minigames[Minigame.ROGU].Score.ToStringI(),
                                   "fist_rank", _minigames[Minigame.FIST].Rank.ToStringI(), "fist_score", _minigames[Minigame.FIST].Score.ToStringI(),
                                   "mob_rank", _minigames[Minigame.MOBI].Rank.ToStringI(), "mob_score", _minigames[Minigame.MOBI].Score.ToStringI(),
                                   "baat_rank", _minigames[Minigame.BAAT].Rank.ToStringI(), "baat_score", _minigames[Minigame.BAAT].Score.ToStringI(),
                                   "bade_rank", _minigames[Minigame.BADE].Rank.ToStringI(), "bade_score", _minigames[Minigame.BADE].Score.ToStringI(),
                                   "baco_rank", _minigames[Minigame.BACO].Rank.ToStringI(), "baco_score", _minigames[Minigame.BACO].Score.ToStringI(),
                                   "bahe_rank", _minigames[Minigame.BAHE].Rank.ToStringI(), "bahe_score", _minigames[Minigame.BAHE].Score.ToStringI());

        Database.Update("players", "id=" + Id, "lastupdate", s_date);
      }
    }

    // Constructor that retrieves player data from RuneScript tracker
    public Player(string rsn, int time) {
      time = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - time;
      try {
        // Get the tracker SOAP for this RSN
        RuneScript.skills RScriptSkills = new RuneScript.RScriptLookupPortTypeClient().trackGetTimeAll(rsn, time);

        // Initialize variables
        _skills = new Skills();
        _skills.Add(Skill.OVER, new Skill(Skill.OVER, RScriptSkills.overall.rank, RScriptSkills.overall.level, RScriptSkills.overall.exp));
        _skills.Add(Skill.ATTA, new Skill(Skill.ATTA, RScriptSkills.attack.rank, RScriptSkills.attack.exp));
        _skills.Add(Skill.DEFE, new Skill(Skill.DEFE, RScriptSkills.defence.rank, RScriptSkills.defence.exp));
        _skills.Add(Skill.STRE, new Skill(Skill.STRE, RScriptSkills.strength.rank, RScriptSkills.strength.exp));
        _skills.Add(Skill.HITP, new Skill(Skill.HITP, RScriptSkills.constitution.rank, RScriptSkills.constitution.exp));
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
        SQLiteDataReader rs = Database.ExecuteReader("SELECT tracker.* FROM tracker INNER JOIN players ON tracker.pid=players.id WHERE players.rsn='" + _name + "' AND tracker.date='" + day.ToStringI("yyyyMMdd") + "';");
        if (rs.Read()) {
          // Initialize variables
          Id = Convert.ToInt32(rs["pid"], CultureInfo.InvariantCulture);
          _skills = new Skills();
          _minigames = new Minigames();
          _ranked = true;

          _skills.Add(Skill.OVER, new Skill(Skill.OVER, Convert.ToInt32(rs["overall_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["overall_level"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["overall_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.ATTA, new Skill(Skill.ATTA, Convert.ToInt32(rs["attack_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["attack_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.DEFE, new Skill(Skill.DEFE, Convert.ToInt32(rs["defence_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["defence_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.STRE, new Skill(Skill.STRE, Convert.ToInt32(rs["strength_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["strength_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.HITP, new Skill(Skill.HITP, Convert.ToInt32(rs["hitpoints_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["hitpoints_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.RANG, new Skill(Skill.RANG, Convert.ToInt32(rs["range_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["range_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.PRAY, new Skill(Skill.PRAY, Convert.ToInt32(rs["prayer_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["prayer_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.MAGI, new Skill(Skill.MAGI, Convert.ToInt32(rs["magic_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["magic_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.COOK, new Skill(Skill.COOK, Convert.ToInt32(rs["cook_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["cook_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.WOOD, new Skill(Skill.WOOD, Convert.ToInt32(rs["woodcut_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["woodcut_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.FLET, new Skill(Skill.FLET, Convert.ToInt32(rs["fletch_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fletch_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.FISH, new Skill(Skill.FISH, Convert.ToInt32(rs["fish_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fish_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.FIRE, new Skill(Skill.FIRE, Convert.ToInt32(rs["firemake_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["firemake_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.CRAF, new Skill(Skill.CRAF, Convert.ToInt32(rs["craft_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["craft_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.SMIT, new Skill(Skill.SMIT, Convert.ToInt32(rs["smith_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["smith_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.MINI, new Skill(Skill.MINI, Convert.ToInt32(rs["mine_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["mine_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.HERB, new Skill(Skill.HERB, Convert.ToInt32(rs["herb_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["herb_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.AGIL, new Skill(Skill.AGIL, Convert.ToInt32(rs["agility_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["agility_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.THIE, new Skill(Skill.THIE, Convert.ToInt32(rs["thief_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["thief_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.SLAY, new Skill(Skill.SLAY, Convert.ToInt32(rs["slay_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["slay_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.FARM, new Skill(Skill.FARM, Convert.ToInt32(rs["farm_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["farm_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.RUNE, new Skill(Skill.RUNE, Convert.ToInt32(rs["runecraft_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["runecraft_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.HUNT, new Skill(Skill.HUNT, Convert.ToInt32(rs["hunt_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["hunt_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.CONS, new Skill(Skill.CONS, Convert.ToInt32(rs["construction_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["construction_xp"], CultureInfo.InvariantCulture)));
          _skills.Add(Skill.SUMM, new Skill(Skill.SUMM, Convert.ToInt32(rs["summ_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["summ_xp"], CultureInfo.InvariantCulture)));

          _minigames.Add(Minigame.DUEL, new Minigame(Minigame.DUEL, Convert.ToInt32(rs["dt_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["dt_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.BOUN, new Minigame(Minigame.BOUN, Convert.ToInt32(rs["bh_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bh_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.ROGU, new Minigame(Minigame.ROGU, Convert.ToInt32(rs["bhr_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bhr_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.FIST, new Minigame(Minigame.FIST, Convert.ToInt32(rs["fist_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fist_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.MOBI, new Minigame(Minigame.MOBI, Convert.ToInt32(rs["mob_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["mob_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.BAAT, new Minigame(Minigame.BAAT, Convert.ToInt32(rs["baat_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["baat_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.BADE, new Minigame(Minigame.BADE, Convert.ToInt32(rs["bade_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bade_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.BACO, new Minigame(Minigame.BACO, Convert.ToInt32(rs["baco_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["baco_score"], CultureInfo.InvariantCulture)));
          _minigames.Add(Minigame.BAHE, new Minigame(Minigame.BAHE, Convert.ToInt32(rs["bahe_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bahe_score"], CultureInfo.InvariantCulture)));

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
        System.Threading.ThreadPool.QueueUserWorkItem(_updateRuneScriptTracker, rsn);

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
              _skills.Add(SkillName, new Skill(SkillName, int.Parse(HiscoreLine[0], CultureInfo.InvariantCulture), int.Parse(HiscoreLine[1], CultureInfo.InvariantCulture), int.Parse(HiscoreLine[2], CultureInfo.InvariantCulture)));
              break;

            case 2:
              // Minigame
              string MinigameName = Minigame.IdToName(_minigames.Count);
              _minigames.Add(MinigameName, new Minigame(MinigameName, int.Parse(HiscoreLine[0], CultureInfo.InvariantCulture), int.Parse(HiscoreLine[1], CultureInfo.InvariantCulture)));
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

    private static void _updateRuneScriptTracker(object rsn) {
      try {
        new RuneScript.RScriptLookupPortTypeClient().trackUpdateUser((string)rsn);
      } catch {
      }
    }

  }
}