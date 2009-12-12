using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  class FarmingItem : SkillItem {

    private int _seedId;
    private int _produceId;
    private int _paymentId;

    public FarmingItem(string[] tokens)
      : base(tokens) {

      // Field 4: Seed
      this.Seed = tokens[4];

      // Field 5: Seed id
      _seedId = int.Parse(tokens[5], CultureInfo.InvariantCulture);

      // Field 6: Produce
      this.Produce = tokens[6];

      // Field 7: Produce id
      _produceId = int.Parse(tokens[7], CultureInfo.InvariantCulture);

      // Field 8: Patch
      this.Patch = tokens[8];

      // Field 9: Plant exp
      this.PlantExp = double.Parse(tokens[9], CultureInfo.InvariantCulture);

      // Field 10: Harvest exp
      this.HarvestExp = double.Parse(tokens[10], CultureInfo.InvariantCulture);

      // Field 11: Check-health exp
      this.CheckHealthExp = double.Parse(tokens[11], CultureInfo.InvariantCulture);

      // Field 12: Grow time
      this.GrowTime = int.Parse(tokens[12], CultureInfo.InvariantCulture);

      // Field 13: Payment
      this.Payment = tokens[13];

      // Field 14: Payment id
      _paymentId = int.Parse(tokens[14], CultureInfo.InvariantCulture);
    }

    public string Seed {
      get;
      set;
    }

    public int SeedPrice {
      get {
        Price price = new Price(_seedId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(this.Seed, @"(\d+)x ");
        if (matchQty.Success)
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);

        return qty * price.MarketPrice;
      }
    }

    public string Produce {
      get;
      set;
    }

    public int ProducePrice {
      get {
        Price price = new Price(_produceId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(this.Produce, @"(\d+)x ");
        if (matchQty.Success)
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);

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
        Price price = new Price(_paymentId);
        price.LoadFromCache();

        int qty = 1;
        Match matchQty = Regex.Match(this.Payment, @"(\d+)x ");
        if (matchQty.Success)
          qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);

        return qty * price.MarketPrice;
      }
    }

  } //class FarmingItem
} ////namespace Supay.Bot