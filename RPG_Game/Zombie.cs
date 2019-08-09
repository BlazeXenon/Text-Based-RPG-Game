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

        public Zombie(int difficulty)
        {
            enemyType = EnemyType.Zombie;
            healthMin = 12;
            increasingHealthInterval = 5;
            powerMin = 1;
            enemyDifficulty = difficulty / 2;

            Encounters = new Encounter[]
            {
                new Encounter("\nA figure wanders aimlessly amidst the field you stumble upon,\nhe sees you and starts to attack you!", 1),
                new Encounter("\nUpon going outside the town you find what looks like a man standing up underneath a tree, suddenly he starts to charge towards you!", 1),
                new Encounter("\nYou find a chest of what appears to be loot. As you approach it, a figure jumps out of a nearby shadow and attempts to attack you!", 1)
            };

            //battleTexts = new string[]{ "\nA figure wanders aimlessly amidst the field you stumble upon,\nhe sees you and starts to attack you!",
            //                            "\nUpon going outside the town you find what looks like a man standing up underneath a tree, suddenly he starts to charge towards you!",
            //                            "\nYou find a chest of what appears to be loot. As you approach it, a figure jumps out of a nearby shadow and attempts to attack you!" };



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
        }
    }
}
