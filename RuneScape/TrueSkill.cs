using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Supay.Bot
{
    internal class TrueSkill : Skill
    {
        private const int MAX_LEVEL = 120;

        public TrueSkill(string name, int rank, int level, long exp)
            : base(name, rank, level, exp)
        {
        }

        public TrueSkill(string name, JToken rank, JToken level, JToken exp)
            : this(name, (string) rank == null ? -1 : int.Parse((string) rank, CultureInfo.InvariantCulture), (string) level == null ? -1 : int.Parse((string) level, CultureInfo.InvariantCulture), (string) exp == null ? -1 : int.Parse((string) exp, CultureInfo.InvariantCulture))
        {
        }

        public TrueSkill(string name, int rank, long exp)
            : base(name, rank)
        {
            this.Exp = exp;

            this.Level = exp.ToLevel();
            if (this.Level > MAX_LEVEL)
            {
                this.Level = MAX_LEVEL;
            }
        }

        public TrueSkill(string name, JToken rank, JToken exp)
            : this(name, (string) rank == null ? -1 : int.Parse((string) rank, CultureInfo.InvariantCulture), (string) exp == null ? -1 : int.Parse((string) exp, CultureInfo.InvariantCulture))
        {
        }

        public override int MaxLevel
        {
            get
            {
                return MAX_LEVEL;
            }
        }
    }
}
