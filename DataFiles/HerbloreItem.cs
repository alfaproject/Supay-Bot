using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Supay.Bot {
  internal class HerbloreItem : SkillItem {
    private readonly int[] _ingredientsIds;
    private readonly int _potionId;
    private int[] _ingredientsPrices;
    private Price _price;

    public HerbloreItem(string[] tokens)
      : base(tokens) {
      // Field 4: Potion ID
      _potionId = int.Parse(tokens[4], CultureInfo.InvariantCulture);

      // Field 5: Ingredients
      Ingredients = tokens[5].Split(';');

      // Field 6: Ingredients IDs
      string[] ingredientsIds = tokens[6].Split(';');
      _ingredientsIds = new int[ingredientsIds.Length];
      for (int i = 0; i < ingredientsIds.Length; i++) {
        _ingredientsIds[i] = int.Parse(ingredientsIds[i], CultureInfo.InvariantCulture);
      }

      // Field 7: Potion effects
      Effect = tokens[7];
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
              var p = new Price(_ingredientsIds[i]);
              p.LoadFromCache();

              int qty = 1;
              Match matchQty = Regex.Match(Ingredients[i], @"(\d+)x ");
              if (matchQty.Success) {
                qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
              }

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
        if (_potionId == 0) {
          return 0;
        }

        int qty = 1;
        if (_price == null) {
          _price = new Price(_potionId);
          _price.LoadFromCache();
          Match matchQty = Regex.Match(Name, @"(\d+)x ");
          if (matchQty.Success) {
            qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
          }
        }
        return qty * _price.MarketPrice;
      }
    }

    public int Cost {
      get {
        return IngredientsPrices.Where(price => price != 0).Sum();
      }
    }
  }
}
