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

        protected abstract Encounter CurrentEncounter { get; set; }

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

        public void InitializeEnemy()
        {
            CurrentEncounter = Encounters[Program.IntRNG(0, Encounters.Length)];
            WriteOutBattleText();
        }
        private void WriteOutBattleText()
        {
            Animation.RunAnimation(interval: 35, textToType: CurrentEncounter.BattleText);
        }
        public uint GetEnemyGoldYield()
        {
            return CurrentEncounter.GoldYield;
        }
        protected uint generateGoldForEncounter(uint min, uint max)
        {
            var buffer = new byte[sizeof(uint)];
            new Random().NextBytes(buffer);
            uint result = BitConverter.ToUInt32(buffer, 0);

            return result % (max - min) + min;
        }

    }
    public enum EnemyType
    {
        Zombie,
        Witch
    }
}
