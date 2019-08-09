using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public abstract class Area
    {
        public abstract EnemyType[] EnemyPool
        {
            get;
            set;
        }

        public BaseEnemy GenerateEnemy(int difficulty)
        {
            return ConvertEnemy(EnemyPool[Program.IntRNG(0, EnemyPool.Length)], difficulty);
        }

        private BaseEnemy ConvertEnemy(EnemyType type, int difficulty)
        {
            switch (type)
            {
                case EnemyType.Zombie:
                    return new Zombie(difficulty);
                default:
                    throw new NullReferenceException($"Error: Enemy Type \"{type}\" is not defined!");
            }
        }
    }
}
