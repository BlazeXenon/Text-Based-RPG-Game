using System;

namespace RPG_Game {
    class Program {
        private static Random rnd = new Random();
        public static float VersionNumber = 1.0f;
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

        public static void ConsoleColorWrite(string msg, ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            Console.Write(msg);
            Console.ResetColor();
        }
        public static void ConsoleColorWrite(string msg, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(msg);
            Console.ResetColor();
        }


        public static void ConsoleColorWriteLine(string msg, ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        public static void ConsoleColorWriteLine(string msg, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }
}
