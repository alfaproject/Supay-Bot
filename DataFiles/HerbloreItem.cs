using System.Globalization;
using System.Text.RegularExpressions;

namespace BigSister {

  class HerbloreItem : ASkillItem {
    private int _potionId;
    private Price _price;

    private int[] _ingredientsIds;
    private int[] _ingredientsPrices;

    public HerbloreItem(string[] tokens)
      : base(tokens) {

      // Field 4: Potion ID
      _potionId = int.Parse(tokens[4], CultureInfo.InvariantCulture);

      // Field 5: Ingredients
      this.Ingredients = tokens[5].Split(';');

      // Field 6: Ingredients IDs
      string[] ingredientsIds = tokens[6].Split(';');
      _ingredientsIds = new int[ingredientsIds.Length];
      for (int i = 0; i < ingredientsIds.Length; i++)
        _ingredientsIds[i] = int.Parse(ingredientsIds[i], CultureInfo.InvariantCulture);

      // Field 7: Potion effects
      this.Effect = tokens[7];
    }

    public string[] Ingredients {
      get;
      set;
    }

    public int[] IngredientsPrices {
      get {
        if (_ingredientsPrices == null) {
          _ingredientsPrices = new int[_ingredientsIds.Length];
          for (int i = 0; i < _ingredientsIds.Length; i++) {
            if (_ingredientsIds[i] > 0) {
              Price p = new Price(_ingredientsIds[i]);
              p.LoadFromCache();

              int qty = 1;
              Match matchQty = Regex.Match(this.Ingredients[i], @"(\d+)x ");
              if (matchQty.Success)
                qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);

              _ingredientsPrices[i] = qty * p.MarketPrice;
            }
          }
        }
        return _ingredientsPrices;
      }
    }

    public string Effect {
      get;
      set;
    }

    public int Price {
      get {
        int qty = 1;
        if (_price == null && _potionId != 0) {
          _price = new Price(_potionId);
          _price.LoadFromCache();
          Match matchQty = Regex.Match(this.Name, @"(\d+)x ");
          if (matchQty.Success)
            qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
        }
        return qty * _price.MarketPrice;
      }
    }

    public int Cost {
      get {
        int cost = 0;
        foreach (int price in this.IngredientsPrices)
          if (price != 0)
            cost += price;
        return cost;
      }
    }

    public override string IrcColour {
      get {
        return "07";
      }
    }

  } //class HerbloreItem
} //namespace BigSister