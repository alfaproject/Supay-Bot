using System.Collections.Generic;
using System.IO;

namespace Supay.Bot {
  class SkillItems : List<SkillItem> {

    public SkillItems(string skill)
      : base() {

      StreamReader dataFile = new StreamReader("Data/Items.txt");
      string dataLine;
      while ((dataLine = dataFile.ReadLine()) != null) {
        string[] tokens = dataLine.Split('\t');
        if (tokens[0] == skill) {
          switch (tokens[0]) {
            case Skill.FARM:
              this.Add(new FarmingItem(tokens));
              break;
            case Skill.MAGI:
              this.Add(new MagicItem(tokens));
              break;
            case Skill.HERB:
              this.Add(new HerbloreItem(tokens));
              break;
            case Skill.SUMM:
              this.Add(new SummoningItem(tokens));
              break;
            default:
              this.Add(new SkillItem(tokens));
              break;
          }
        }
      }
      dataFile.Close();
    }

    public SkillItems()
      : base() {

      StreamReader dataFile = new StreamReader("Data/Items.txt");
      string dataLine;
      while ((dataLine = dataFile.ReadLine()) != null) {
        string[] tokens = dataLine.Split('\t');

        switch (tokens[0]) {
          case Skill.FARM:
            this.Add(new FarmingItem(tokens));
            break;
          case Skill.MAGI:
            this.Add(new MagicItem(tokens));
            break;
          case Skill.HERB:
            this.Add(new HerbloreItem(tokens));
            break;
          case Skill.SUMM:
            this.Add(new SummoningItem(tokens));
            break;
          default:
            this.Add(new SkillItem(tokens));
            break;
        }
      }
      dataFile.Close();
    }

  } //class SkillItems
} ////namespace Supay.Bot
