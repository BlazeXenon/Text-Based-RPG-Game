using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public abstract class BaseEnemy
    {
        public abstract int Health { get; set; }
        public abstract int EnemyDifficulty { get; set; }

        protected abstract EnemyType EnemyType { get; set; }

        protected abstract int Power { get; set; }

        protected abstract int HealthMin { get; set; }
        protected abstract int IncreasingHealthInterval { get; set; }
        protected abstract int PowerMin { get; set; }

        protected abstract Encounter[] Encounters { get; set; }

        public int Attack()
        {
            int min = Power - Program.InclusiveIntRNG(1, 3),
                max = Power + Program.InclusiveIntRNG(1, 3);

            if (min <= 0) min = 1;
            return Program.InclusiveIntRNG(min, max);
        }
        public void Damage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }
        public void WriteOutBattleText()
        {
            Animation.RunAnimation(interval: 35, textToType: Encounters[Program.IntRNG(0, Encounters.Length)].BattleText);
        }
        public uint GetEnemyGoldYield()
        {
            return Program.UIntRNG((Health + Power) * 5, (Health + Power) * 8);
        }

    }
    public enum EnemyType
    {
        Zombie
    }
}
