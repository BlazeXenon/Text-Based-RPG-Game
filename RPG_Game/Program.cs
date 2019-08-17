using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPG_Game {
    class Program
    {
        private static Random rnd = new Random();
        public static float VersionNumber = 1.32f;

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.WindowWidth + 5, Console.WindowHeight + 5);
            Menu menu = new Menu();
            DatabaseHelper dbHelper = new DatabaseHelper("GameData");

            menu.Start();
        }

        public static int IntRNG(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public static int InclusiveIntRNG(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }

        public static uint UIntRNG(uint min, uint max)
        {
            // Generates a 32 bit UInt courtesy of:
            // https://stackoverflow.com/questions/31451485/how-do-i-generate-a-random-uint-with-a-maximum

            var buffer = new byte[sizeof(uint)];
            new Random().NextBytes(buffer);
            uint result = BitConverter.ToUInt32(buffer, 0);

            return result % (max - min) + min;
        }

        public static float FloatRNG()
        {
            return (float) rnd.NextDouble();
        }

        public static void ConsoleColorWrite(string msg)
        {
            char[] msgCharArray = msg.ToCharArray();
            for (int i = 0; i < msg.Length; i++)
            {
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar))
                {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                }
                else
                {
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
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar))
                {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                }
                else
                {
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
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar))
                {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                }
                else
                {
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
                if (Animation.CheckForImmediateEscapeCharacters(msgCharArray, i, out var escapeChar))
                {
                    if (Animation.InterpretEscapeCharacter(escapeChar))
                    {
                        i += 1;
                    }
                    else
                    {
                        Console.Write(msgCharArray[i]);
                    }
                }
                else
                {
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

        public static byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object Deserialize(byte[] serialBytes)
        {
            using (MemoryStream ms = new MemoryStream(serialBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }
    }
}
