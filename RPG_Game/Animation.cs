using System;
using System.Collections.Generic;
using System.Threading;

namespace RPG_Game {
    public class Animation {


        //Variables definited for static and single use. (Only 1 animation at a time, or 1 queue of animations at a time)
        private static List<Animation> queue = new List<Animation>();
        private static char[] _Characters;
        private static int _Repeat = 1, _Interval;
        public static bool _Skipped = false;


        //Variables definited for when queueing up multiple animations.
        private AnimationType internalType;
        private string internalText;
        private int internalInterval;
        private bool internalShouldSkipLine;

        public Animation(AnimationType type, int interval, string text = "", bool skipLine = true) {
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
            while (actualRepeat > 0) {
                for (int i = 0; i < _Characters.Length; i++) {

                    if (Console.KeyAvailable) {
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter) {
                            _Skipped = true;
                        }
                    }

                    var EscapeCharCaught = CheckForImmediateEscapeCharacters(_Characters, i, out var escapeChar);
                    if (_Skipped == false) {
                        if (EscapeCharCaught) {
                            if (escapeChar == "\n") {
                                Console.WriteLine();
                                i++;
                            } else if (escapeChar == "\t") {
                                Console.Write("\t");
                                i++;
                            } else {
                                Console.Write(_Characters[i]);
                            }
                        } else {
                            Console.Write(_Characters[i]);
                        }
                        Thread.Sleep(interval);
                    } else {
                        remainingCharacterIndex = i;
                        break;
                    }


                }

                if (remainingCharacterIndex != -1) {

                    string compiledCharacters = new string(_Characters);

                    if (HasEscapeCharacters(compiledCharacters)) { //Write them out individually to handle escape characters.
                        for (int i = 0; i < compiledCharacters.Length; i++) {
                            var EscapeCharCaught = CheckForImmediateEscapeCharacters(_Characters, i, out var escapeChar);

                            if (escapeChar == "\n") {
                                Console.WriteLine();
                                i++;
                            } else if (escapeChar == "\t") {
                                Console.Write("\t");
                                i++;
                            } else {
                                Console.Write(compiledCharacters[i]);
                            }
                        }
                    } else { //Write the characters out all at once.
                        Console.Write(compiledCharacters.Substring(remainingCharacterIndex));
                    }
                }

                actualRepeat--;
            }


            if (skipLine) {
                Console.WriteLine();
            }

            ClearAnimation();

        }
        private static bool CheckForImmediateEscapeCharacters(char[] characterArray, int startingIndex, out string escapeCharacterDetected) {
            if (startingIndex + 1 < characterArray.Length) {
                if (characterArray[startingIndex] == '\\' && characterArray[startingIndex + 1] == 'n') {
                    escapeCharacterDetected = "\n";
                    return true;
                } else if (characterArray[startingIndex] == '\\' && characterArray[startingIndex + 1] == 't') {
                    escapeCharacterDetected = "\t";
                    return true;
                } else {
                    escapeCharacterDetected = null;
                }
            } else { escapeCharacterDetected = null; }
            return false;
        }
        private static bool HasEscapeCharacters(string str) {
            for (int index = 0; index < str.Length; index++) {
                if (index + 1 < str.Length) {
                    if (str[index] == '\\' && (str[index + 1] == 'n' || str[index + 1] == 't'))
                        return true;
                }
            }
            return false;
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
