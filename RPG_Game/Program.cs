using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game {
    class Program {
        private static Random rnd = new Random();
        static void Main(string[] args) {

            Menu menu = new Menu();

            menu.Start();

        }

        public static int IntRNG(int min, int max) {
            return rnd.Next(min, max);
        }

        public static float FloatRNG() {
            return (float)rnd.NextDouble();
        }
    }
}
