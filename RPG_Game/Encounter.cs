using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public class Encounter
    {
        private string battleText;
        private uint goldYield;
        public string BattleText { get { return battleText; } private set { battleText = value; } }
        public uint GoldYield { get { return goldYield; } private set { goldYield = value; } }

        public Encounter(string BattleText, uint GoldYield)
        {
            this.BattleText = BattleText;
            this.GoldYield = GoldYield;
        }
    }
}
