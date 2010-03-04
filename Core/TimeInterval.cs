using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Supay.Bot {
  public class TimeInterval {

    public string Name;
    public TimeSpan Time;

    public TimeInterval() {
    }

    public TimeInterval(int defaultTime, string defaultName) {
      this.Time = new TimeSpan(0, 0, defaultTime);
      this.Name = defaultName;
    }

    public bool Parse(string timeInterval) {

      int time = 0;
      int _time;
      string name = string.Empty;
      string _name = string.Empty;

      foreach (Match interval in Regex.Matches(timeInterval, @"(\d+)?(second|minute|month|hour|week|year|sec|min|day|s|m|h|d|w|y)s?", RegexOptions.IgnoreCase)) {
        if (interval.Groups[1].Value.Length > 0)
          _time = int.Parse(interval.Groups[1].Value, CultureInfo.InvariantCulture);
        else
          _time = 1;
        if (_time < 1)
          _time = 1;
        switch (interval.Groups[2].Value) {
          case "second":
          case "sec":
          case "s":
            _name = _time + " second" + (_time == 1 ? string.Empty : "s");
            break;
          case "minute":
          case "min":
          case "m":
            _name = _time + " minute" + (_time == 1 ? string.Empty : "s");
            _time *= 60;
            break;
          case "hour":
          case "h":
            _name = _time + " hour" + (_time == 1 ? string.Empty : "s");
            _time *= 3600;
            break;
          case "day":
          case "d":
            _name = _time + " day" + (_time == 1 ? string.Empty : "s");
            _time *= 86400;
            break;
          case "week":
          case "w":
            _name = _time + " week" + (_time == 1 ? string.Empty : "s");
            _time *= 604800;
            break;
          case "month":
            _name = _time + " month" + (_time == 1 ? string.Empty : "s");
            _time *= 2629746;
            break;
          case "year":
          case "y":
            _name = _time + " year" + (_time == 1 ? string.Empty : "s");
            _time *= 31556952;
            break;
        }

        time += _time;
        name += " " + _name;
      }
      if (name != string.Empty) {
        this.Name = name.Trim();
        this.Time = new TimeSpan(0, 0, time);
        return true;
      }
      return false;
    }

    public bool Parse(int timeInterval) {
      TimeSpan timeSpan = new TimeSpan(timeInterval);
      try {
        this.Name = timeSpan.ToLongString();
        return true;
      } catch {
        return false;
      }
    }

  } //class TimeInterval
} //namespace Supay.Bot