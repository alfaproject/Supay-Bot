using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Supay.Bot
{
    internal class HerbloreItem : SkillItem
    {
        private readonly int[] _ingredientsIds;
        private readonly int _potionId;
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

        public async Task<long[]> GetIngredientsPrices()
        {
            var ingredientsPrices = new long[this._ingredientsIds.Length];
            for (int i = 0; i < this._ingredientsIds.Length; i++)
            {
                if (this._ingredientsIds[i] > 0)
                {
                    var p = await Price.FromCache(_ingredientsIds[i]);

                    int qty = 1;
                    Match matchQty = Regex.Match(this.Ingredients[i], @"(\d+)x ");
                    if (matchQty.Success)
                    {
                        qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
                    }

                    ingredientsPrices[i] = qty * p.MarketPrice;
                }
            }
            return ingredientsPrices;
        }

        public string Effect
        {
            get;
            set;
        }

        public async Task<long> GetPrice()
        {
            if (this._potionId == 0)
            {
                return 0;
            }

            int qty = 1;
            if (this._price == null)
            {
                this._price = await Price.FromCache(_potionId);
                Match matchQty = Regex.Match(this.Name, @"(\d+)x ");
                if (matchQty.Success)
                {
                    qty = int.Parse(matchQty.Groups[1].Value, CultureInfo.InvariantCulture);
                }
            }
            return qty * this._price.MarketPrice;
        }

        public async Task<long> GetCost()
        {
            var ingredientsPrices = await GetIngredientsPrices();
            return ingredientsPrices.Where(price => price != 0).Sum();
        }
    }
}
