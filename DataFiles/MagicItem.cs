using System.Globalization;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
    internal class MagicItem : SkillItem
    {
        public MagicItem(string[] tokens)
            : base(tokens)
        {
            // Field 4: Runes
            this.Runes = tokens[4].Split(';');

            // Field 5: MaxHit
            this.MaxHit = int.Parse(tokens[5], CultureInfo.InvariantCulture);

            // Field 6: Book
            this.Book = tokens[6];

            // Field 7: Effect
            this.Effect = tokens[7];
        }

        public string[] Runes
        {
            get;
            set;
        }

        public int RunesCost
        {
            get
            {
                int cost = 0;
                foreach (string rune in this.Runes)
                {
                    Match matchRune = Regex.Match(rune, @"(\d+)x (\w+)");
                    if (matchRune.Success)
                    {
                        int runeId = 0;
                        switch (matchRune.Groups[2].Value)
                        {
                            case "Air":
                                runeId = 556;
                                break;
                            case "Mind":
                                runeId = 558;
                                break;
                            case "Water":
                                runeId = 555;
                                break;
                            case "Earth":
                                runeId = 557;
                                break;
                            case "Fire":
                                runeId = 554;
                                break;
                            case "Body":
                                runeId = 559;
                                break;
                            case "Cosmic":
                                runeId = 564;
                                break;
                            case "Chaos":
                                runeId = 562;
                                break;
                            case "Astral":
                                runeId = 9075;
                                break;
                            case "Nature":
                                runeId = 561;
                                break;
                            case "Law":
                                runeId = 563;
                                break;
                            case "Death":
                                runeId = 560;
                                break;
                            case "Blood":
                                runeId = 565;
                                break;
                            case "Soul":
                                runeId = 566;
                                break;
                        }
                        var price = new Price(runeId);
                        price.LoadFromCache();
                        cost += int.Parse(matchRune.Groups[1].Value, CultureInfo.InvariantCulture) * price.MarketPrice;
                    }
                }
                return cost;
            }
        }

        public int MaxHit
        {
            get;
            set;
        }

        public string Book
        {
            get;
            set;
        }

        public string Effect
        {
            get;
            set;
        }
    }
}
