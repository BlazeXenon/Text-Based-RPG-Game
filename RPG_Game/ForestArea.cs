using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public class ForestArea : Area
    {
        private EnemyType[] enemyPool;
        public override EnemyType[] EnemyPool
        {
            get
            {
                return enemyPool;
            }

            set
            {
                enemyPool = value;
            }
        }

        public ForestArea()
        {
            EnemyPool = new[]{ EnemyType.Zombie, EnemyType.Witch };
        }
    }
}
