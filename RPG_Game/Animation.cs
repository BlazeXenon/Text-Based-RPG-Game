using System;
using System.Collections.Generic;
using System.Threading;

namespace RPG_Game {
    public class Animation {


        //Variables defined for static and single use. (Only 1 animation at a time, or 1 queue of animations at a time)
        private static List<Animation> queue = new List<Animation>();
        private static char[] _Characters;
        private static int _Repeat = 1, _Interval;
        public static bool _Skipped = false;


        //Variables defined for when queueing up multiple animations.
        private AnimationType internalType;
        private string internalText;
        private int internalInterval;
        private bool internalShouldSkipLine;

        public Animation(AnimationType type = AnimationType.TextTyping, int interval = 20, string text = "", bool skipLine = true) {
            internalType = type;
            internalInterval = interval;
            internalText = text;
            internalShouldSkipLine = skipLine;
        }

        public static void Queue(Animation animation) {
            queue.Add(animation);
        }

        public static void PlayQueue(bool resetQueue = true) {
            foreach (Animation a in queue) {
                RunAnimation(a.internalType, a.internalInterval, a.internalText, a.internalShouldSkipLine);
            }
            _Skipped = false;
            if (resetQueue)
                queue = new List<Animation>();
        }

        public static void RunAnimation(AnimationType type = AnimationType.TextTyping, int interval = 20, string textToType = "", bool skipLine = true) {

            LoadAnimation(type, interval, textToType);

            int actualRepeat = _Repeat;
            int remainingCharacterIndex = -1;
            while (actualRepeat > 0) 
            {
                for (int i = 0; i < _Characters.Length; i++) 
                {
                    if (Console.KeyAvailable) 
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter) 
                        {
                            _Skipped = true;
                        }
                    }

                    if (_Skipped == false) 
                    {
                        if (CheckForImmediateEscapeCharacters(_Characters, i, out var escapeChar)) 
                        {
                            if (InterpretEscapeCharacter(escapeChar))
                            {
                                i += 1;
                            }
                            else
                            {
                                Console.Write(_Characters[i]);
                            }
                        } 
                        else 
                        {
                            Console.Write(_Characters[i]);
                        }
                        Thread.Sleep(interval);
                    } 
                    else 
                    {
                        remainingCharacterIndex = i;
                        break;
                    }
                }

                if (remainingCharacterIndex != -1) {

                    string compiledCharacters = new string(_Characters);

                    if (HasEscapeCharacters(compiledCharacters)) { //Write them out individually to handle escape characters.
                        for (int i = remainingCharacterIndex; i < compiledCharacters.Length; i++) {
                            var EscapeCharCaught = CheckForImmediateEscapeCharacters(_Characters, i, out var escapeChar);

                            if (InterpretEscapeCharacter(escapeChar))
                            {
                                i += 1;
                            }
                            else
                            {
                                Console.Write(compiledCharacters[i]);
                            }
                        }
                    } else { //Write the characters out all at once.
                        Console.Write(compiledCharacters.Substring(remainingCharacterIndex));
                    }
                }

                actualRepeat--;
            }


            if (skipLine) 
                Console.WriteLine();

            ClearAnimation();

        }
        public static bool CheckForImmediateEscapeCharacters(char[] characterArray, int startingIndex, out string escapeCharacterDetected)
        {
            if (startingIndex + 1 < characterArray.Length)
            {
                char first = characterArray[startingIndex];
                char second = characterArray[startingIndex + 1];
                string compiled = first.ToString() + second;

                bool foundEscapeChar = false;
                
                if (first == '\\' && second == 'n')
                {
                    foundEscapeChar = true;
                }
                else if (first == '\\' && second == 't')
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/0")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/B")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/G")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/C")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/R")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/M")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/Y")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/A")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/b")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/g")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/c")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/r")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/m")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/y")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/w")
                {
                    foundEscapeChar = true;
                }
                else if (compiled == "/e")
                {
                    foundEscapeChar = true;
                }

                if (foundEscapeChar)
                {
                    escapeCharacterDetected = compiled;
                    return true;
                }

            }

            escapeCharacterDetected = null;
            return false;
        }
        public static bool HasEscapeCharacters(string str) {
            for (int index = 0; index < str.Length; index++) {
                if (index + 1 < str.Length) {
                    if (str[index] == '\\' && (str[index + 1] == 'n' || str[index + 1] == 't'))
                        return true;
                    if (str[index] == '/' && (str[index + 1] == '0' || str[index + 1] == 'B' || str[index + 1] == 'G' || str[index + 1] == 'C' || str[index + 1] == 'R' || str[index + 1] == 'M' || str[index + 1] == 'Y' || str[index + 1] == 'A' || str[index + 1] == 'b' || str[index + 1] == 'g' || str[index + 1] == 'c' || str[index + 1] == 'r' || str[index + 1] == 'm' || str[index + 1] == 'y' || str[index + 1] == 'w' || str[index + 1] == 'e'))
                        return true;
                }
            }
            return false;
        }
        public static bool InterpretEscapeCharacter(string escapeChar)
        {
            if (escapeChar == null)
                return false;

            if (escapeChar.Length < 2)
                return false;

            if (escapeChar[0] == '\\' && escapeChar[1] == 'n')
            {
                Console.Write("\n");
                return true;
            }
            else if (escapeChar[0] == '\\' && escapeChar[1] == 't')
            {
                Console.Write("\t");
                return true;
            }
            else if (escapeChar == "/0")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                return true;
            }
            else if (escapeChar == "/B")
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                return true;
            }
            else if (escapeChar == "/G")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                return true;
            }
            else if (escapeChar == "/C")
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                return true;
            }
            else if (escapeChar == "/R")
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                return true;
            }
            else if (escapeChar == "/M")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                return true;
            }
            else if (escapeChar == "/Y")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                return true;
            }
            else if (escapeChar == "/A")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                return true;
            }
            else if (escapeChar == "/b")
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                return true;
            }
            else if (escapeChar == "/g")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                return true;
            }
            else if (escapeChar == "/c")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                return true;
            }
            else if (escapeChar == "/r")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                return true;
            }
            else if (escapeChar == "/m")
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                return true;
            }
            else if (escapeChar == "/y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                return true;
            }
            else if (escapeChar == "/w")
            {
                Console.ForegroundColor = ConsoleColor.White;
                return true;
            }
            else if (escapeChar == "/e")
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadAnimation(AnimationType type, int interval, string text) {
            _Interval = interval;
            _Characters = text.ToCharArray();
            if (type == AnimationType.Dot) {
                _Characters = new char[2];
                _Characters[0] = '.';
                _Characters[1] = ' ';
                _Repeat = 3;
            } else if (type == AnimationType.TextTyping) {
                _Characters = text.ToCharArray();
            }
        }

        public static void ClearAnimation() {
            _Characters = null;
            _Repeat = 1;
            if (queue != null && queue.Count == 0) {
                _Skipped = false;
            }
        }
    }

    public enum AnimationType {
        Dot,
        TextTyping
    }
}
