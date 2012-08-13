using System;
using System.Data.SQLite;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Supay.Bot
{
  internal class Player
  {
    private readonly ActivityDictionary _activities;
    private readonly string _name;

    private readonly SkillDictionary _skills;
    private string _combatclass;

    // Constructor that retrieves player data from RuneScript tracker
    public Player(string rsn, int time)
    {
      time = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds - time;
      try
      {
        // RuneTracker: sets or renews the 7-day long auto-update for a user and looks them up once a day.
        var runeTrackerWebClient = new WebClient();
        runeTrackerWebClient.DownloadString("http://runetracker.org/api.php?run=updateUserAutoLookup&user=" + rsn);

        // RuneTracker: get a player entry from database at a specified time.
        var runeTrackerRawJson = runeTrackerWebClient.DownloadString("http://runetracker.org/api.php?run=getTrackTable&limit=1&timeEnd=" + time + "&user=" + rsn);
        var runeTrackerJson = JObject.Parse(runeTrackerRawJson);

        // Initialize variables
        this._skills = new SkillDictionary {
          { Skill.OVER, new Skill(Skill.OVER, runeTrackerJson["skill0rank"], runeTrackerJson["skill0lvl"], runeTrackerJson["skill0exp"]) },
          { Skill.ATTA, new Skill(Skill.ATTA, runeTrackerJson["skill1rank"], runeTrackerJson["skill1exp"]) },
          { Skill.DEFE, new Skill(Skill.DEFE, runeTrackerJson["skill2rank"], runeTrackerJson["skill2exp"]) },
          { Skill.STRE, new Skill(Skill.STRE, runeTrackerJson["skill3rank"], runeTrackerJson["skill3exp"]) },
          { Skill.HITP, new Skill(Skill.HITP, runeTrackerJson["skill4rank"], runeTrackerJson["skill4exp"]) },
          { Skill.RANG, new Skill(Skill.RANG, runeTrackerJson["skill5rank"], runeTrackerJson["skill5exp"]) },
          { Skill.PRAY, new Skill(Skill.PRAY, runeTrackerJson["skill6rank"], runeTrackerJson["skill6exp"]) },
          { Skill.MAGI, new Skill(Skill.MAGI, runeTrackerJson["skill7rank"], runeTrackerJson["skill7exp"]) },
          { Skill.COOK, new Skill(Skill.COOK, runeTrackerJson["skill8rank"], runeTrackerJson["skill8exp"]) },
          { Skill.WOOD, new Skill(Skill.WOOD, runeTrackerJson["skill9rank"], runeTrackerJson["skill9exp"]) },
          { Skill.FLET, new Skill(Skill.FLET, runeTrackerJson["skill10rank"], runeTrackerJson["skill10exp"]) },
          { Skill.FISH, new Skill(Skill.FISH, runeTrackerJson["skill11rank"], runeTrackerJson["skill11exp"]) },
          { Skill.FIRE, new Skill(Skill.FIRE, runeTrackerJson["skill12rank"], runeTrackerJson["skill12exp"]) },
          { Skill.CRAF, new Skill(Skill.CRAF, runeTrackerJson["skill13rank"], runeTrackerJson["skill13exp"]) },
          { Skill.SMIT, new Skill(Skill.SMIT, runeTrackerJson["skill14rank"], runeTrackerJson["skill14exp"]) },
          { Skill.MINI, new Skill(Skill.MINI, runeTrackerJson["skill15rank"], runeTrackerJson["skill15exp"]) },
          { Skill.HERB, new Skill(Skill.HERB, runeTrackerJson["skill16rank"], runeTrackerJson["skill16exp"]) },
          { Skill.AGIL, new Skill(Skill.AGIL, runeTrackerJson["skill17rank"], runeTrackerJson["skill17exp"]) },
          { Skill.THIE, new Skill(Skill.THIE, runeTrackerJson["skill18rank"], runeTrackerJson["skill18exp"]) },
          { Skill.SLAY, new Skill(Skill.SLAY, runeTrackerJson["skill19rank"], runeTrackerJson["skill19exp"]) },
          { Skill.FARM, new Skill(Skill.FARM, runeTrackerJson["skill20rank"], runeTrackerJson["skill20exp"]) },
          { Skill.RUNE, new Skill(Skill.RUNE, runeTrackerJson["skill21rank"], runeTrackerJson["skill21exp"]) },
          { Skill.HUNT, new Skill(Skill.HUNT, runeTrackerJson["skill22rank"], runeTrackerJson["skill22exp"]) },
          { Skill.CONS, new Skill(Skill.CONS, runeTrackerJson["skill23rank"], runeTrackerJson["skill23exp"]) },
          { Skill.SUMM, new Skill(Skill.SUMM, runeTrackerJson["skill24rank"], runeTrackerJson["skill24exp"]) },
          { Skill.DUNG, new TrueSkill(Skill.DUNG, runeTrackerJson["skill25rank"], runeTrackerJson["skill25exp"]) }
        };

        this._activities = new ActivityDictionary {
          { Activity.DUEL, new Activity(Activity.DUEL, runeTrackerJson["mg0rank"], runeTrackerJson["mg0exp"]) },
          { Activity.BOUN, new Activity(Activity.BOUN, runeTrackerJson["mg1rank"], runeTrackerJson["mg1exp"]) },
          { Activity.ROGU, new Activity(Activity.ROGU, runeTrackerJson["mg2rank"], runeTrackerJson["mg2exp"]) },
          { Activity.FIST, new Activity(Activity.FIST, runeTrackerJson["mg3rank"], runeTrackerJson["mg3exp"]) },
          { Activity.MOBI, new Activity(Activity.MOBI, runeTrackerJson["mg4rank"], runeTrackerJson["mg4exp"]) },
          { Activity.BAAT, new Activity(Activity.BAAT, runeTrackerJson["mg5rank"], runeTrackerJson["mg5exp"]) },
          { Activity.BADE, new Activity(Activity.BADE, runeTrackerJson["mg6rank"], runeTrackerJson["mg6exp"]) },
          { Activity.BACO, new Activity(Activity.BACO, runeTrackerJson["mg7rank"], runeTrackerJson["mg7exp"]) },
          { Activity.BAHE, new Activity(Activity.BAHE, runeTrackerJson["mg8rank"], runeTrackerJson["mg8exp"]) },
          { Activity.CWAR, new Activity(Activity.CWAR, runeTrackerJson["mg9rank"], runeTrackerJson["mg9exp"]) },
          { Activity.CONQ, new Activity(Activity.CONQ, runeTrackerJson["mg10rank"], runeTrackerJson["mg10exp"]) },
          { Activity.DOMI, new Activity(Activity.DOMI, runeTrackerJson["mg11rank"], runeTrackerJson["mg11exp"]) }
        };

        this.Ranked = true;

        // Try to guess missing skills and update our skill list
        this._GuessMissingSkills();

        // Create combat skill and update combat class
        this._CreateCombatSkill();
      }
      catch
      {
        this.Ranked = false;
      }
    }

    // Constructor that retrieves player data from Database
    public Player(string rsn, DateTime day)
    {
      this._name = rsn;
      this.LastUpdate = day;

      try
      {
        // Query database
        SQLiteDataReader rs = Database.ExecuteReader("SELECT tracker.* FROM tracker INNER JOIN players ON tracker.pid=players.id WHERE players.rsn = '" + this._name + "' AND tracker.date='" + day.ToStringI("yyyyMMdd") + "';");
        if (rs.Read())
        {
          // Initialize variables
          this.Id = Convert.ToInt32(rs["pid"], CultureInfo.InvariantCulture);
          this._skills = new SkillDictionary();
          this._activities = new ActivityDictionary();
          this.Ranked = true;

          this._skills.Add(Skill.OVER, new Skill(Skill.OVER, Convert.ToInt32(rs["overall_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["overall_level"], CultureInfo.InvariantCulture), Convert.ToInt64(rs["overall_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.ATTA, new Skill(Skill.ATTA, Convert.ToInt32(rs["attack_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["attack_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.DEFE, new Skill(Skill.DEFE, Convert.ToInt32(rs["defence_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["defence_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.STRE, new Skill(Skill.STRE, Convert.ToInt32(rs["strength_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["strength_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.HITP, new Skill(Skill.HITP, Convert.ToInt32(rs["hitpoints_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["hitpoints_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.RANG, new Skill(Skill.RANG, Convert.ToInt32(rs["range_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["range_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.PRAY, new Skill(Skill.PRAY, Convert.ToInt32(rs["prayer_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["prayer_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.MAGI, new Skill(Skill.MAGI, Convert.ToInt32(rs["magic_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["magic_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.COOK, new Skill(Skill.COOK, Convert.ToInt32(rs["cook_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["cook_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.WOOD, new Skill(Skill.WOOD, Convert.ToInt32(rs["woodcut_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["woodcut_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.FLET, new Skill(Skill.FLET, Convert.ToInt32(rs["fletch_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fletch_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.FISH, new Skill(Skill.FISH, Convert.ToInt32(rs["fish_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fish_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.FIRE, new Skill(Skill.FIRE, Convert.ToInt32(rs["firemake_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["firemake_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.CRAF, new Skill(Skill.CRAF, Convert.ToInt32(rs["craft_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["craft_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.SMIT, new Skill(Skill.SMIT, Convert.ToInt32(rs["smith_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["smith_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.MINI, new Skill(Skill.MINI, Convert.ToInt32(rs["mine_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["mine_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.HERB, new Skill(Skill.HERB, Convert.ToInt32(rs["herb_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["herb_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.AGIL, new Skill(Skill.AGIL, Convert.ToInt32(rs["agility_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["agility_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.THIE, new Skill(Skill.THIE, Convert.ToInt32(rs["thief_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["thief_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.SLAY, new Skill(Skill.SLAY, Convert.ToInt32(rs["slay_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["slay_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.FARM, new Skill(Skill.FARM, Convert.ToInt32(rs["farm_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["farm_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.RUNE, new Skill(Skill.RUNE, Convert.ToInt32(rs["runecraft_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["runecraft_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.HUNT, new Skill(Skill.HUNT, Convert.ToInt32(rs["hunt_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["hunt_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.CONS, new Skill(Skill.CONS, Convert.ToInt32(rs["construction_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["construction_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.SUMM, new Skill(Skill.SUMM, Convert.ToInt32(rs["summ_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["summ_xp"], CultureInfo.InvariantCulture)));
          this._skills.Add(Skill.DUNG, new TrueSkill(Skill.DUNG, Convert.ToInt32(rs["dungRank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["dungExp"], CultureInfo.InvariantCulture)));

          this._activities.Add(Activity.DUEL, new Activity(Activity.DUEL, Convert.ToInt32(rs["dt_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["dt_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.BOUN, new Activity(Activity.BOUN, Convert.ToInt32(rs["bh_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bh_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.ROGU, new Activity(Activity.ROGU, Convert.ToInt32(rs["bhr_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bhr_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.FIST, new Activity(Activity.FIST, Convert.ToInt32(rs["fist_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["fist_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.MOBI, new Activity(Activity.MOBI, Convert.ToInt32(rs["mob_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["mob_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.BAAT, new Activity(Activity.BAAT, Convert.ToInt32(rs["baat_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["baat_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.BADE, new Activity(Activity.BADE, Convert.ToInt32(rs["bade_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bade_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.BACO, new Activity(Activity.BACO, Convert.ToInt32(rs["baco_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["baco_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.BAHE, new Activity(Activity.BAHE, Convert.ToInt32(rs["bahe_rank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["bahe_score"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.CWAR, new Activity(Activity.CWAR, Convert.ToInt32(rs["cwarRank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["cwarScore"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.CONQ, new Activity(Activity.CONQ, Convert.ToInt32(rs["conqRank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["conqScore"], CultureInfo.InvariantCulture)));
          this._activities.Add(Activity.DOMI, new Activity(Activity.DOMI, Convert.ToInt32(rs["domiRank"], CultureInfo.InvariantCulture), Convert.ToInt32(rs["domiScore"], CultureInfo.InvariantCulture)));

          // Create combat skill and update combat class
          this._CreateCombatSkill();
        }
        else
        {
          SQLiteDataReader player = Database.ExecuteReader("SELECT * FROM players WHERE rsn LIKE '" + this._name + "';");
          if (player.Read())
          {
            this.Id = Convert.ToInt32(player["id"], CultureInfo.InvariantCulture);
          }
          this.Ranked = false;
        }
        rs.Close();
      }
      catch (Exception)
      {
        this.Ranked = false;
      }
    }

    // Constructor that retrieves player data from RuneScape Hiscores
    public Player(string rsn)
    {
      this._name = rsn;

      try
      {
        // Get hiscores page for this RSN
        string hiscorePage = new WebClient().DownloadString("http://hiscore.runescape.com/index_lite.ws?player=" + this._name);

        // Initialize variables
        this._skills = new SkillDictionary();
        this._activities = new ActivityDictionary();
        this.Ranked = true;

        // Parse Hiscores page for this player
        foreach (string hiscoreLine in hiscorePage.Split('\n'))
        {
          string[] hiscoreTokens = hiscoreLine.Split(',');
          switch (hiscoreTokens.Length)
          {
            case 3: // skill
              string skillName = Skill.IdToName(this._skills.Count);
              if (skillName == Skill.DUNG)
              {
                this._skills.Add(skillName, new TrueSkill(skillName, int.Parse(hiscoreTokens[0], CultureInfo.InvariantCulture), int.Parse(hiscoreTokens[1], CultureInfo.InvariantCulture), int.Parse(hiscoreTokens[2], CultureInfo.InvariantCulture)));
              }
              else
              {
                this._skills.Add(skillName, new Skill(skillName, int.Parse(hiscoreTokens[0], CultureInfo.InvariantCulture), int.Parse(hiscoreTokens[1], CultureInfo.InvariantCulture), long.Parse(hiscoreTokens[2], CultureInfo.InvariantCulture)));
              }
              break;
            case 2: // activity
              string activityName = Activity.IdToName(this._activities.Count);
              this._activities.Add(activityName, new Activity(activityName, int.Parse(hiscoreTokens[0], CultureInfo.InvariantCulture), int.Parse(hiscoreTokens[1], CultureInfo.InvariantCulture)));
              break;
          }
        }

        // Try to guess missing skills and update our skill list
        this._GuessMissingSkills();

        // Create combat skill and update combat class
        this._CreateCombatSkill();
      }
      catch (Exception)
      {
        this.Ranked = false;
      }
    }

    public long Id
    {
      get;
      private set;
    }

    public DateTime LastUpdate
    {
      get;
      set;
    }

    public string Name
    {
      get
      {
        return this._name;
      }
    }

    public string CombatClass
    {
      get
      {
        return this._combatclass;
      }
    }

    public bool Ranked
    {
      get;
      set;
    }

    public SkillDictionary Skills
    {
      get
      {
        return this._skills;
      }
    }

    public ActivityDictionary Activities
    {
      get
      {
        return this._activities;
      }
    }

    private void _GuessMissingSkills()
    {
      // Check if Overall has exp.
      if (this._skills[Skill.OVER].Exp == -1)
      {
        // Fix Constitution
        if (this._skills[Skill.HITP].Exp == -1)
        {
          this._skills[Skill.HITP].Exp = 10.ToExp();
          this._skills[Skill.HITP].Level = 10;
        }

        // Fix Overall and all other unranked skills 
        this._skills[Skill.OVER].Exp = 0;
        this._skills[Skill.OVER].Level = 0;
        for (int i = 1; i < this._skills.Count; i++)
        {
          if (this._skills[i].Exp == -1)
          {
            this._skills[i].Exp = 0;
            this._skills[i].Level = 1;
          }
          else
          {
            this._skills[Skill.OVER].Exp += this._skills[i].Exp;
          }
          this._skills[Skill.OVER].Level += this._skills[i].Level;
        }
      }
      else
      {
        // Fix Constitution
        if (this._skills[Skill.HITP].Exp == -1)
        {
          this._skills[Skill.HITP].Level = 10;
        }

        // Find missing skills and calculate total known levels 
        int rankedLevels = 0;
        for (int i = 1; i < this._skills.Count; i++)
        {
          if (this._skills[i].Exp != -1 || this._skills[i].Name == Skill.HITP)
          {
            rankedLevels += this._skills[i].Level;
          }
        }

        // RuneScript hack
        if (this._skills[Skill.OVER].Level == 0)
        {
          this._skills[Skill.OVER].Level = rankedLevels;
        }

        // Fill unranked skills with some estimate xp and level 
        while (this._skills[Skill.OVER].Level - rankedLevels != 0)
        {
          for (int i = 1; i < this._skills.Count; i++)
          {
            if (this._skills[i].Exp == -1)
            {
              if (this._skills[i].Level == -1)
              {
                this._skills[i].Level = 1;
              }
              else
              {
                this._skills[i].Level++;
              }
              rankedLevels++;
            }
            if (this._skills[Skill.OVER].Level - rankedLevels == 0)
            {
              break;
            }
          }
        }

        foreach (Skill s in this._skills.Values)
        {
          if (s.Exp == -1)
          {
            s.Exp = s.Level.ToExp();
          }
        }
      }
    }

    private void _CreateCombatSkill()
    {
      this._combatclass = Utils.CombatClass(this._skills, false);
      int CmbLevel = Utils.CalculateCombat(this._skills, false, false);
      long CmbExp = this._skills[Skill.ATTA].Exp + this._skills[Skill.STRE].Exp + this._skills[Skill.DEFE].Exp + this._skills[Skill.HITP].Exp + this._skills[Skill.RANG].Exp + this._skills[Skill.PRAY].Exp + this._skills[Skill.MAGI].Exp + this._skills[Skill.SUMM].Exp;
      this._skills.Add(Skill.COMB, new Skill(Skill.COMB, -1, CmbLevel, CmbExp));
    }

    public void SaveToDB(string s_date)
    {
      this.Id = Database.Lookup<long>("id", "players", "rsn=@name", new[] { new SQLiteParameter("@name", this._name) });

      if (this.Ranked)
      {
        Database.Insert(
          "tracker",
          "pid", this.Id.ToStringI(),
          "date", s_date,
          "overall_level", this._skills[0].Level.ToStringI(), "overall_xp", this._skills[0].Exp.ToStringI(), "overall_rank", this._skills[0].Rank.ToStringI(),
          "attack_xp", this._skills[1].Exp.ToStringI(), "attack_rank", this._skills[1].Rank.ToStringI(),
          "defence_xp", this._skills[2].Exp.ToStringI(), "defence_rank", this._skills[2].Rank.ToStringI(),
          "strength_xp", this._skills[3].Exp.ToStringI(), "strength_rank", this._skills[3].Rank.ToStringI(),
          "hitpoints_xp", this._skills[4].Exp.ToStringI(), "hitpoints_rank", this._skills[4].Rank.ToStringI(),
          "range_xp", this._skills[5].Exp.ToStringI(), "range_rank", this._skills[5].Rank.ToStringI(),
          "prayer_xp", this._skills[6].Exp.ToStringI(), "prayer_rank", this._skills[6].Rank.ToStringI(),
          "magic_xp", this._skills[7].Exp.ToStringI(), "magic_rank", this._skills[7].Rank.ToStringI(),
          "cook_xp", this._skills[8].Exp.ToStringI(), "cook_rank", this._skills[8].Rank.ToStringI(),
          "woodcut_xp", this._skills[9].Exp.ToStringI(), "woodcut_rank", this._skills[9].Rank.ToStringI(),
          "fletch_xp", this._skills[10].Exp.ToStringI(), "fletch_rank", this._skills[10].Rank.ToStringI(),
          "fish_xp", this._skills[11].Exp.ToStringI(), "fish_rank", this._skills[11].Rank.ToStringI(),
          "firemake_xp", this._skills[12].Exp.ToStringI(), "firemake_rank", this._skills[12].Rank.ToStringI(),
          "craft_xp", this._skills[13].Exp.ToStringI(), "craft_rank", this._skills[13].Rank.ToStringI(),
          "smith_xp", this._skills[14].Exp.ToStringI(), "smith_rank", this._skills[14].Rank.ToStringI(),
          "mine_xp", this._skills[15].Exp.ToStringI(), "mine_rank", this._skills[15].Rank.ToStringI(),
          "herb_xp", this._skills[16].Exp.ToStringI(), "herb_rank", this._skills[16].Rank.ToStringI(),
          "agility_xp", this._skills[17].Exp.ToStringI(), "agility_rank", this._skills[17].Rank.ToStringI(),
          "thief_xp", this._skills[18].Exp.ToStringI(), "thief_rank", this._skills[18].Rank.ToStringI(),
          "slay_xp", this._skills[19].Exp.ToStringI(), "slay_rank", this._skills[19].Rank.ToStringI(),
          "farm_xp", this._skills[20].Exp.ToStringI(), "farm_rank", this._skills[20].Rank.ToStringI(),
          "runecraft_xp", this._skills[21].Exp.ToStringI(), "runecraft_rank", this._skills[21].Rank.ToStringI(),
          "hunt_xp", this._skills[22].Exp.ToStringI(), "hunt_rank", this._skills[22].Rank.ToStringI(),
          "construction_xp", this._skills[23].Exp.ToStringI(), "construction_rank", this._skills[23].Rank.ToStringI(),
          "summ_xp", this._skills[24].Exp.ToStringI(), "summ_rank", this._skills[24].Rank.ToStringI(),
          "dungExp", this._skills[25].Exp.ToStringI(), "dungRank", this._skills[25].Rank.ToStringI(),
          "dt_rank", this._activities[Activity.DUEL].Rank.ToStringI(), "dt_score", this._activities[Activity.DUEL].Score.ToStringI(),
          "bh_rank", this._activities[Activity.BOUN].Rank.ToStringI(), "bh_score", this._activities[Activity.BOUN].Score.ToStringI(),
          "bhr_rank", this._activities[Activity.ROGU].Rank.ToStringI(), "bhr_score", this._activities[Activity.ROGU].Score.ToStringI(),
          "fist_rank", this._activities[Activity.FIST].Rank.ToStringI(), "fist_score", this._activities[Activity.FIST].Score.ToStringI(),
          "mob_rank", this._activities[Activity.MOBI].Rank.ToStringI(), "mob_score", this._activities[Activity.MOBI].Score.ToStringI(),
          "baat_rank", this._activities[Activity.BAAT].Rank.ToStringI(), "baat_score", this._activities[Activity.BAAT].Score.ToStringI(),
          "bade_rank", this._activities[Activity.BADE].Rank.ToStringI(), "bade_score", this._activities[Activity.BADE].Score.ToStringI(),
          "baco_rank", this._activities[Activity.BACO].Rank.ToStringI(), "baco_score", this._activities[Activity.BACO].Score.ToStringI(),
          "bahe_rank", this._activities[Activity.BAHE].Rank.ToStringI(), "bahe_score", this._activities[Activity.BAHE].Score.ToStringI(),
          "cwarRank", this._activities[Activity.CWAR].Rank.ToStringI(), "cwarScore", this._activities[Activity.CWAR].Score.ToStringI(),
          "conqRank", this._activities[Activity.CONQ].Rank.ToStringI(), "conqScore", this._activities[Activity.CONQ].Score.ToStringI(),
          "domiRank", this._activities[Activity.DOMI].Rank.ToStringI(), "domiScore", this._activities[Activity.DOMI].Score.ToStringI());

        Database.Update("players", "id=" + this.Id, "lastupdate", s_date);
      }
    }

    public override string ToString()
    {
      return this._name;
    }
  }
}
