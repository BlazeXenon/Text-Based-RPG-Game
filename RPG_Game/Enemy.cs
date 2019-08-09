using System;

namespace RPG_Game {
    public class Enemy {

        public int Health { get { return health; } private set { health = value; } }
        public int EnemyDifficulty { get { return enemyDifficulty; } private set { enemyDifficulty = value; } }

        private EnemyType type;
        private int enemyDifficulty;
        private int health = -1, power = -1;
        private int healthMin = 15, healthIncreaseInterval = 5, powerMin = 1;

        private string[] zombieBattleTexts = { "\nA figure wanders aimlessly amidst the field you stumble upon, he sees you and starts to attack you!",
                                                "\nUpon going outside the town you find what looks like a man standing up underneath a tree, suddenly he starts to charge towards you!",
                                                "\nYou find a chest of what appears to be loot. As you approach it, a figure jumps out of a nearby shadow and attempts to attack you!"};

        public Enemy(int difficulty) {

            type = EnemyType.Zombie;

            enemyDifficulty = difficulty / 2;

            if (enemyDifficulty == 0) {
                health = Program.InclusiveIntRNG(healthMin, healthMin + healthIncreaseInterval);
                power = Program.InclusiveIntRNG(powerMin, powerMin + 4);
            } else {
                health = Program.InclusiveIntRNG(healthMin + (enemyDifficulty + 1), healthMin + (healthIncreaseInterval * (enemyDifficulty + 1) ));
                power = Program.InclusiveIntRNG(powerMin + (enemyDifficulty + 1), powerMin + (powerMin * (enemyDifficulty + 1)));
            }
        }

        public int Attack() {
            int min = power - Program.InclusiveIntRNG(1, 3),
                max = power + Program.InclusiveIntRNG(1, 3);


            if (min <= 0) min = 1;
            return Program.InclusiveIntRNG(min, max);
        }

        public void Damage(int amount) {
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public void WriteOutBattleText() {
            if (type == EnemyType.Zombie) 
                Animation.RunAnimation(interval: 35, textToType: zombieBattleTexts[Program.IntRNG(0, zombieBattleTexts.Length)]);
        }

        enum EnemyType {
            Zombie
        }
    }
}
