using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal class FarmingItem : SkillItem {
    private readonly int _paymentId;
    private readonly int _produceId;
    private readonly int _seedId;

    public FarmingItem(string[] tokens)
      : base(tokens) {
      // Field 4: Seed
      Seed = tokens[4];

      // Field 5: Seed id
      _seedId = int.Parse(tokens[5], CultureInfo.InvariantCulture);

      // Field 6: Produce
      Produce = tokens[6];

      // Field 7: Produce id
      _produceId = int.Parse(tokens[7], CultureInfo.InvariantCulture);

      // Field 8: Patch
      Patch = tokens[8];

      // Field 9: Plant exp
      PlantExp = double.Parse(tokens[9], CultureInfo.InvariantCulture);

      // Field 10: Harvest exp
      HarvestExp = double.Parse(tokens[10], CultureInfo.InvariantCulture);

      // Field 11: Check-health exp
      CheckHealthExp = double.Parse(tokens[11], CultureInfo.InvariantCulture);

      // Field 12: Grow time
      GrowTime = int.Parse(tokens[12], CultureInfo.InvariantCulture);

      // Field 13: Payment
      Payment = tokens[13];

      // Field 14: Payment id
      _paymentId = int.Parse(tokens[14], CultureInfo.InvariantCulture);
    }

    public string Seed {
      get;
      set;
    }

    public int SeedPrice {
      get {
        var price = new Price(_seedId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(Seed, @"(\d+)x ");
        if (matchQty.Success) {
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        return qty * price.MarketPrice;
      }
    }

    public string Produce {
      get;
      set;
    }

    public int ProducePrice {
      get {
        var price = new Price(_produceId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(Produce, @"(\d+)x ");
        if (matchQty.Success) {
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        return qty * price.MarketPrice;
      }
    }

    public string Patch {
      get;
      set;
    }

    public double PlantExp {
      get;
      set;
    }

    public double HarvestExp {
      get;
      set;
    }

    public double CheckHealthExp {
      get;
      set;
    }

    public int GrowTime {
      get;
      set;
    }

    public string Payment {
      get;
      set;
    }

    public int PaymentPrice {
      get {
        var price = new Price(_paymentId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(Payment, @"(\d+)x ");
        if (matchQty.Success) {
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        return qty * price.MarketPrice;
      }
    }
  }
}
