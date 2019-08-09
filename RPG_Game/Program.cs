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
        public static int InclusiveIntRNG(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }
        public static uint UIntRNG(int min, int max)
        {
            // Very elegant way to generate a 32 bit UInt courtesy of:
            // https://stackoverflow.com/questions/17080112/generate-random-uint

            uint thirtyBits = (uint)rnd.Next(1 << 30);
            uint twoBits = (uint)rnd.Next(1 << 2);
            return (thirtyBits << 2) | twoBits;
        }

        public static float FloatRNG() {
            return (float)rnd.NextDouble();
        }

        public static void ConsoleColorWrite(string msg)
        {
            char[] msgCharArray = msg.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar)) {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                } else {
                    Console.Write(msgCharArray[i]);
                }
            }
        }
        public static void ConsoleColorWrite(string msg, ConsoleColor background)
        {
            ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = background;
            char[] msgCharArray = msg.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar)) {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                } else {
                    Console.Write(msgCharArray[i]);
                }
            }

            Console.BackgroundColor = defaultBackgroundColor;
        }


        public static void ConsoleColorWriteLine(string msg)
        {
            char[] msgCharArray = msg.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar)) {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                } else {
                    Console.Write(msgCharArray[i]);
                }
            }
            Console.WriteLine();
        }
        public static void ConsoleColorWriteLine(string msg, ConsoleColor background)
        {
            ConsoleColor defaultBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = background;
            char[] msgCharArray = msg.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar)) {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                } else {
                    Console.Write(msgCharArray[i]);
                }
            }
            Console.BackgroundColor = defaultBackgroundColor;
            Console.WriteLine();
        }

        public static string ToProperString(string str)
        {
            if (str.Length < 1) return string.Empty;
            return str[0].ToString().ToUpperInvariant() + str.Substring(1, str.Length - 1).ToLowerInvariant();
        }
    }
}
