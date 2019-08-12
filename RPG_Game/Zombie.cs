using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public class Zombie : BaseEnemy
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
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }
        public override int EnemyDifficulty
        {
            get
            {
                return enemyDifficulty;
            }

            set
            {
                enemyDifficulty = value;
            }
        }
        protected override EnemyType EnemyType
        {
            get
            {
                return enemyType;
            }

            set
            {
                enemyType = value;
            }
        }
        protected override int Power
        {
            get
            {
                return power;
            }

            set
            {
                power = value;
            }
        }
        protected override int HealthMin
        {
            get
            {
                return healthMin;
            }

            set
            {
                healthMin = value;
            }
        }
        protected override int IncreasingHealthInterval
        {
            get
            {
                return increasingHealthInterval;
            }

            set
            {
                increasingHealthInterval = value;
            }
        }
        protected override int PowerMin
        {
            get
            {
                return powerMin;
            }

            set
            {
                powerMin = value;
            }
        }
        protected override Encounter[] Encounters
        {
            get
            {
                return encounters;
            }

            set
            {
                encounters = value;
            }
        }
        protected override Encounter CurrentEncounter
        {
            get
            {
                return currentEncounter;
            }
            set
            {
                currentEncounter = value;
            }
        }

        public Zombie(int difficulty)
        {
            enemyType = EnemyType.Zombie;
            healthMin = 12;
            increasingHealthInterval = 5;
            powerMin = 1;
            enemyDifficulty = difficulty / 2;

            if (enemyDifficulty == 0)
            {
                health = Program.InclusiveIntRNG(healthMin, healthMin + increasingHealthInterval);
                power = Program.InclusiveIntRNG(powerMin, powerMin + 4);
            }
            else
            {
                health = Program.InclusiveIntRNG(healthMin + (enemyDifficulty + 1), healthMin + (increasingHealthInterval * (enemyDifficulty + 1)));
                power = Program.InclusiveIntRNG(powerMin + (enemyDifficulty + 1), powerMin + (powerMin * (enemyDifficulty + 1)));
            }

            uint min = (uint)(Health + Power) * 5, 
                 max = (uint)(Health + Power) * 8;

            Encounters = new[]
            {
                new Encounter("A figure wanders aimlessly amidst the field you stumble upon, he sees you and starts to attack you!", generateGoldForEncounter(min, max)),
                new Encounter("Upon going outside the town you find what looks like a man standing up underneath a tree, suddenly he starts to charge towards you!", generateGoldForEncounter(min, max)),
                new Encounter("You find a chest of what appears to be loot. As you approach it, a figure jumps out of a nearby shadow and attempts to attack you!", generateGoldForEncounter(min, max))
            };
        }
    }
}
