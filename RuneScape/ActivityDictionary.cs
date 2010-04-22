using System.Collections.Generic;

namespace Supay.Bot {
  class ActivityDictionary : OrderedDictionary<string, Activity> {

    public ActivityDictionary()
      : base(9) {
    }

  } // class ActivityDictionary
} //namespace Supay.Bot