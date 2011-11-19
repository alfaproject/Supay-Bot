using System.Collections.Generic;

namespace Supay.Bot
{
  internal class ActivityDictionary : OrderedDictionary<string, Activity>
  {
    public ActivityDictionary()
      : base(12)
    {
    }
  }
}
