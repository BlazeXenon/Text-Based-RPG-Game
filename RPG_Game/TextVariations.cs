using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public class TextVariations
    {
        public static string[] GoldVariations = { "Upon searching the body you notice something in his pocket, you find /w{/e /ygold/e!",
                                                  "Your attack was so powerful, it knocked your enemy off their feet! While flying in the air you notice that /w{/e /ygold/e pieces fell out from their pocket!",
                                                  "You notice that there are some gold pieces on the ground. You think to yourself that the enemy must've dropped these and they have no use for them anymore! You gain /w{/e /ygold/e!",
                                                  "After your random and troublesome encounter, you are rewarded for your efforts... Your foe seemed to have /w{/e /ygold/e on them!" };


        public static string GetRandomGoldText(uint goldAmount)
        {
            int rndTextIndex = Program.IntRNG(0, GoldVariations.Length);
            string rndText = GoldVariations[rndTextIndex];

            return rndText.Replace("{", goldAmount.ToString());
        }
    }
}
