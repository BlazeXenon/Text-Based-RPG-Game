using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public class Witch : BaseEnemy
    {
        private int health;
        private int enemyDifficulty;
        private EnemyType enemyType;
        private int power;
        private int healthMin;
        private int increasingHealthInterval;
        private int powerMin;
        private Encounter[] encounters;
        private Encounter currentEncounter;
        public override int Health
        {
            get => health;

            set => health = value;
        }

        public override int EnemyDifficulty
        {
            get => enemyDifficulty;

            set => enemyDifficulty = value;
        }

        protected override EnemyType EnemyType
        {
            get => enemyType;

            set => enemyType = value;
        }

        protected override int Power
        {
            get => power;

            set => power = value;
        }

        protected override int HealthMin
        {
            get => healthMin;

            set => healthMin = value;
        }

        protected override int IncreasingHealthInterval
        {
            get => increasingHealthInterval;

            set => increasingHealthInterval = value;
        }

        protected override int PowerMin
        {
            get => powerMin;

            set => powerMin = value;
        }

        protected override Encounter[] Encounters
        {
            get => encounters;

            set => encounters = value;
        }

        protected override Encounter CurrentEncounter
        {
            get => currentEncounter;

            set => currentEncounter = value;
        }

        public Witch(int difficulty)
        {
            enemyType = EnemyType.Witch;
            healthMin = 6;
            increasingHealthInterval = 3;
            powerMin = 2;
            enemyDifficulty = difficulty / 2;

            health = Program.InclusiveIntRNG(healthMin * (healthMin / 2) + (enemyDifficulty), healthMin * (healthMin / 2) + (increasingHealthInterval * (enemyDifficulty + 1)));
            power = Program.InclusiveIntRNG(powerMin + (Game.ps.Health + enemyDifficulty) / 2, (powerMin + (Game.ps.Health + enemyDifficulty + 2)) / 2);

            uint min = (uint)(Health + Power) * 6,
                 max = (uint)(Health + Power) * 9;

            Encounters = new[]
            {
                new Encounter("When wandering down a narrow curvy road, you stumble upon what looks to be an old lady. You attempt to ask her whats wrong but before you can react she attacks you!", generateGoldForEncounter(min, max)),
                new Encounter("While venturing in the wilderness, you notice what seems to be arrow carved into the side of a tree. You follow this arrow to another arrow, and to another, until you finally reach a mushroom shaped house. As you walk towards the house you see a figure with a long pointy hat throw a glass bottle at you!", generateGoldForEncounter(min, max)),
                new Encounter("Upon entering the local forest you encounter a old, wooden chest. While rummaging through the chest, you are interupted by a small, screeching voice. You turn around and find a little old women getting ready to cast a spell at you!", generateGoldForEncounter(min, max))
            };
        }
    }
}
