using System;
using System.Text.RegularExpressions;

namespace Supay.Bot
{
  internal class AlogItem
  {
    public readonly string[] Info;
    public readonly string Type;

    public AlogItem(RssItem item, Match M, string Type)
    {
      this.Amount = 1;
      this.Date = item.Date;
      this.Title = item.Title;
      this.Description = item.Description;
      this.Link = item.Link;
      this.Type = Type;
      if (M == null)
      {
        this.Info = new string[1];
        this.Info[0] = item.Title;
      }
      else
      {
        var groups = new string[2];
        for (int i = 1; i < M.Groups.Count; i++)
        {
          groups[i - 1] = M.Groups[i].Value.Trim();
        }
        this.Info = groups;
      }
      if (this.Type == "I killed")
      {
        string npc = this.Info[0];
        if (npc.Length == 0)
        {
          npc = this.Info[1];
        }
        Match N = Regex.Match(npc, @"^(\d+) (.+?)$");
        if (N.Success)
        {
          this.Amount = N.Groups[1].Value.ToInt32();
          this.Info[0] = N.Groups[2].Value.EndsWith("s") ? N.Groups[2].Value.Substring(0, N.Groups[2].Value.Length - 1) : N.Groups[2].Value;
        }
        else
        {
          this.Info[0] = npc.EndsWith("s") ? npc.Substring(0, npc.Length - 1) : npc;
        }
      }
    }

    public DateTime Date
    {
      get;
      set;
    }

    public string Description
    {
      get;
      set;
    }

    public string Link
    {
      get;
      set;
    }

    public string Title
    {
      get;
      set;
    }

    public int Amount
    {
      get;
      set;
    }
  }
}
