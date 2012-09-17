using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
    internal class HerbloreItem : SkillItem
    {
        private readonly int[] _ingredientsIds;
        private readonly int _potionId;
        private int[] _ingredientsPrices;
        private Price _price;

        public HerbloreItem(string[] tokens)
            : base(tokens)
        {
            // Field 4: Potion ID
            this._potionId = int.Parse(tokens[4], CultureInfo.InvariantCulture);

            // Field 5: Ingredients
            this.Ingredients = tokens[5].Split(';');

            // Field 6: Ingredients IDs
            string[] ingredientsIds = tokens[6].Split(';');
            this._ingredientsIds = new int[ingredientsIds.Length];
            for (int i = 0; i < ingredientsIds.Length; i++)
            {
                this._ingredientsIds[i] = int.Parse(ingredientsIds[i], CultureInfo.InvariantCulture);
            }

            // Field 7: Potion effects
            this.Effect = tokens[7];
        }

        public string[] Ingredients
        {
            get;
            set;
        }

        public int[] IngredientsPrices
        {
            get
            {
                if (this._ingredientsPrices == null)
                {
                    this._ingredientsPrices = new int[this._ingredientsIds.Length];
                    for (int i = 0; i < this._ingredientsIds.Length; i++)
                    {
                        if (this._ingredientsIds[i] > 0)
                        {
                            var p = new Price(this._ingredientsIds[i]);
                            p.LoadFromCache();

                            int qty = 1;
                            Match matchQty = Regex.Match(this.Ingredients[i], @"(\d+)x ");
                            if (matchQty.Success)
                            {
                                qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
                            }

                            this._ingredientsPrices[i] = qty * p.MarketPrice;
                        }
                    }
                }
                return this._ingredientsPrices;
            }
        }

        public string Effect
        {
            get;
            set;
        }

        public int Price
        {
            get
            {
                if (this._potionId == 0)
                {
                    return 0;
                }

                int qty = 1;
                if (this._price == null)
                {
                    this._price = new Price(this._potionId);
                    this._price.LoadFromCache();
                    Match matchQty = Regex.Match(this.Name, @"(\d+)x ");
                    if (matchQty.Success)
                    {
                        qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
                    }
                }
                return qty * this._price.MarketPrice;
            }
        }

        public int Cost
        {
            get
            {
                return this.IngredientsPrices.Where(price => price != 0).Sum();
            }
        }
    }
}
