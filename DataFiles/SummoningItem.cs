using System;
using System.Globalization;

namespace Supay.Bot
{
    internal class SummoningItem : SkillItem
    {
        private readonly int _pouchId;

        public SummoningItem(string[] tokens)
            : base(tokens)
        {
            this._pouchId = int.Parse(tokens[4], CultureInfo.InvariantCulture);
            this.HighAlch = int.Parse(tokens[5], CultureInfo.InvariantCulture);
            this.Combat = int.Parse(tokens[6], CultureInfo.InvariantCulture);
            this.Shards = int.Parse(tokens[7], CultureInfo.InvariantCulture);
            this.Charm = tokens[8];
            this.Components = tokens[9];
            this.ComponentsIds = tokens[10];
            this.Time = int.Parse(tokens[11], CultureInfo.InvariantCulture);
            this.Abilities = tokens[12];
        }

        public int HighAlch
        {
            get;
            set;
        }

        public int Combat
        {
            get;
            set;
        }

        public int Shards
        {
            get;
            set;
        }

        public string Charm
        {
            get;
            set;
        }

        public string Components
        {
            get;
            set;
        }

        public string ComponentsIds
        {
            get;
            set;
        }

        public int Time
        {
            get;
            set;
        }

        public string Abilities
        {
            get;
            set;
        }

        public override string IrcColour
        {
            get
            {
                switch (this.Charm)
                {
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

        public string NameCombat
        {
            get
            {
                if (this.Combat > 0)
                {
                    return this.Name + " (" + this.Combat + ")";
                }
                return this.Name;
            }
        }

        public long ComponentsPrice
        {
            get
            {
                if (this.ComponentsIds == "0")
                {
                    return 0;
                }

                long componentsPrice = 0;
                foreach (string component in this.ComponentsIds.Split('+'))
                {
                    var price = new Price(int.Parse(component, CultureInfo.InvariantCulture));
                    price.LoadFromCache();
                    componentsPrice += price.MarketPrice;
                }
                return componentsPrice;
            }
        }

        public long PouchPrice
        {
            get
            {
                var price = new Price(this._pouchId);
                price.LoadFromCache();
                return price.MarketPrice;
            }
        }

        public long TotalCost
        {
            get
            {
                return this.ComponentsPrice + this.Shards * 25 + 1;
            }
        }

        public long BogrogCost
        {
            get
            {
                return this.ComponentsPrice + (int) Math.Ceiling(.3 * this.Shards) * 25 + 1;
            }
        }

        public double CheapestExpCost
        {
            get
            {
                var nature = new Price(561);
                nature.LoadFromCache();

                var componentsPrice = this.ComponentsPrice;
                var totalCost = componentsPrice + this.Shards * 25 + 1;

                double bogrogExp = (componentsPrice + Math.Ceiling(.3 * this.Shards) * 25 + 1) / this.Exp;
                ////double marketExp = (totalCost - this.PouchPrice) / this.Exp;
                double alchExp = (totalCost + nature.MarketPrice - this.HighAlch) / this.Exp;

                return Math.Min(bogrogExp, alchExp);
            }
        }
    }
}
