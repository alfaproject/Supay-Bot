using System;
using System.Globalization;

namespace Supay.Bot {
  internal class SummoningItem : SkillItem {
    private readonly int _pouchId;

    public SummoningItem(string[] tokens)
      : base(tokens) {
      _pouchId = int.Parse(tokens[4], CultureInfo.InvariantCulture);
      HighAlch = int.Parse(tokens[5], CultureInfo.InvariantCulture);
      Combat = int.Parse(tokens[6], CultureInfo.InvariantCulture);
      Shards = int.Parse(tokens[7], CultureInfo.InvariantCulture);
      Charm = tokens[8];
      Components = tokens[9];
      ComponentsIds = tokens[10];
      Time = int.Parse(tokens[11], CultureInfo.InvariantCulture);
      Abilities = tokens[12];
    }

    public int HighAlch {
      get;
      set;
    }

    public int Combat {
      get;
      set;
    }

    public int Shards {
      get;
      set;
    }

    public string Charm {
      get;
      set;
    }

    public string Components {
      get;
      set;
    }

    public string ComponentsIds {
      get;
      set;
    }

    public int Time {
      get;
      set;
    }

    public string Abilities {
      get;
      set;
    }

    public override string IrcColour {
      get {
        switch (Charm) {
          case "Crimson":
            return "04";
          case "Green":
            return "03";
          case "Blue":
            return "10";
          default: // gold
            return "07";
        }
      }
    }

    public string NameCombat {
      get {
        if (Combat > 0) {
          return Name + " (" + Combat + ")";
        }
        return Name;
      }
    }

    public int ComponentsPrice {
      get {
        if (ComponentsIds == "0") {
          return 0;
        }

        int componentsPrice = 0;
        foreach (string component in ComponentsIds.Split('+')) {
          var price = new Price(int.Parse(component, CultureInfo.InvariantCulture));
          price.LoadFromCache();
          componentsPrice += price.MarketPrice;
        }
        return componentsPrice;
      }
    }

    public int PouchPrice {
      get {
        var price = new Price(_pouchId);
        price.LoadFromCache();
        return price.MarketPrice;
      }
    }

    public int TotalCost {
      get {
        return ComponentsPrice + Shards * 25 + 1;
      }
    }

    public int BogrogCost {
      get {
        return ComponentsPrice + (int) Math.Ceiling(.3 * Shards) * 25 + 1;
      }
    }

    public double CheapestExpCost {
      get {
        var nature = new Price(561);
        nature.LoadFromCache();

        int componentsPrice = ComponentsPrice;
        int totalCost = componentsPrice + Shards * 25 + 1;

        double bogrogExp = (componentsPrice + Math.Ceiling(.3 * Shards) * 25 + 1) / Exp;
        //double marketExp = (totalCost - this.PouchPrice) / this.Exp;
        double alchExp = (totalCost + nature.MarketPrice - HighAlch) / Exp;

        return Math.Min(bogrogExp, alchExp);
      }
    }
  }
}
