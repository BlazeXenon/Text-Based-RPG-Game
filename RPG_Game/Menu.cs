using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RPG_Game {
    public class Menu {

        public int choice = -1;
        private bool playMenuAnimation = true;

        private static PlayerStats stats;

        public Menu() { }


        public void Start() {
            bool continuePlay = true;
            while (continuePlay) {
                while (choice <= 0) {
                    Console.Title = "RPG Game V" + Program.VersionNumber;
                    LoadScreen(playMenuAnimation);
                    Console.Write(">> ");
                    var input = Console.ReadLine();

                    if (int.TryParse(input, out choice)) {
                        if (choice == 1) {
                            var success = NewGame();
                            if (!success)
                                choice = -1;
                        } else if (choice == 2) {
                            LoadGame();
                        } else if (choice == 99) {
                            System.Diagnostics.Process.Start(Application.ExecutablePath);
                            Environment.Exit(0);
                        } else {
                            Animation.RunAnimation(textToType: "\n\nInvalid Input.\n");
                            Console.ReadKey();
                            choice = -1;
                        }
                    } else if (!input.Trim().Equals("")) {
                        Animation.RunAnimation(textToType: "\n\nInvalid Input.\n");
                        Console.ReadKey();
                        choice = -1;
                    }
                }
                Game game = new Game(stats);
                if (!game.ShouldGameRestart) {
                    continuePlay = false;
                }
            }
            
        }

        private void LoadScreen(bool useAnimation) {
            Console.Clear();
            if (useAnimation) {
                Animation.Queue(new Animation(AnimationType.TextTyping, 80, "\tWelcome to an\n"));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"____/\\\\\\\\\______/\\\\\\\\\\\\\_______/\\\\\\\\\\\\_        "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @" __/\\\///////\\\___\/\\\/////////\\\___/\\\//////////__       "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"  _\/\\\_____\/\\\___\/\\\_______\/\\\__/\\\_____________      "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"   _\/\\\\\\\\\\\/____\/\\\\\\\\\\\\\/__\/\\\____/\\\\\\\_     "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"    _\/\\\//////\\\____\/\\\/////////____\/\\\___\/////\\\_    "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"     _\/\\\____\//\\\___\/\\\_____________\/\\\_______\/\\\_   "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"      _\/\\\_____\//\\\__\/\\\_____________\/\\\_______\/\\\_  "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"       _\/\\\______\//\\\_\/\\\_____________\//\\\\\\\\\\\\/__ "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"        _\///________\///__\///_______________\////////////____\n"));
                Animation.Queue(new Animation(AnimationType.TextTyping, 80, "\t\t\t\t\tGame.\n\n\n"));
                Animation.PlayQueue();
                playMenuAnimation = false;
            } else {
                Console.WriteLine("\tWelcome to an\n");
                Console.WriteLine(@"____/\\\\\\\\\______/\\\\\\\\\\\\\_______/\\\\\\\\\\\\_        ");
                Console.WriteLine(@" __/\\\///////\\\___\/\\\/////////\\\___/\\\//////////__       ");
                Console.WriteLine(@"  _\/\\\_____\/\\\___\/\\\_______\/\\\__/\\\_____________      ");
                Console.WriteLine(@"   _\/\\\\\\\\\\\/____\/\\\\\\\\\\\\\/__\/\\\____/\\\\\\\_     ");
                Console.WriteLine(@"    _\/\\\//////\\\____\/\\\/////////____\/\\\___\/////\\\_    ");
                Console.WriteLine(@"     _\/\\\____\//\\\___\/\\\_____________\/\\\_______\/\\\_   ");
                Console.WriteLine(@"      _\/\\\_____\//\\\__\/\\\_____________\/\\\_______\/\\\_  ");
                Console.WriteLine(@"       _\/\\\______\//\\\_\/\\\_____________\//\\\\\\\\\\\\/__ ");
                Console.WriteLine(@"        _\///________\///__\///_______________\////////////____" + "\n");
                Console.WriteLine("\t\t\t\t\tGame.\n\n\n");
            }
            Console.WriteLine("(1) New Game " + (File.Exists(Directory.GetCurrentDirectory() + "/save.txt") ? "(2) Load Game " : "") + "(99) Exit");
        }

        private bool NewGame() {
            string[] classDescriptions = { "Warrior: What some call a fearsome brute, this class thrives in health and strength.\n",
                                           "Mage: A powerful and deadly force, these people specialize in the art of magic.\n",
                                           "Archer: Swift and nimble, the archer's bow allows them to pick enemies off from a distance.\n"};

            string playerClass = "";
            PlayerClass pc = PlayerClass.Undefined;

            bool animationHasBeenPlayed = false;

            while (playerClass == "") {
                Console.Clear();

                if (!animationHasBeenPlayed) {
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[0] + "\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[1] + "\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[2] + "\n\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, "(use the command \"back\" to return to the previous screen)\n"));
                    Animation.PlayQueue();
                    animationHasBeenPlayed = true;
                } else {
                    Console.WriteLine(classDescriptions[0] + "\n");
                    Console.WriteLine(classDescriptions[1] + "\n");
                    Console.WriteLine(classDescriptions[2] + "\n\n");
                    Console.WriteLine("(use the command \"back\" to return to the previous screen)\n");
                }

                WriteOnBottomLine("Please Choose Your Class:\n", 2);
                WriteOnBottomLine(">> ");
                var pClass = Console.ReadLine();
                if (pClass.Length > 0 && (pClass.ToLowerInvariant() == "warrior" || pClass.ToLowerInvariant() == "mage" || pClass.ToLowerInvariant() == "archer")) {
                    playerClass = pClass.ToLowerInvariant();
                } else if (pClass.ToLowerInvariant().Equals("back")) {
                    return false;
                } else if (!pClass.Trim().Equals("")) {
                    Animation.RunAnimation(textToType: "\n\n\nInvalid Class Selection.\n");
                    Console.ReadKey();
                }
            }
            if (playerClass == "warrior") { pc = PlayerClass.Warrior; } else if (playerClass == "mage") { pc = PlayerClass.Mage; } else if (playerClass == "archer") { pc = PlayerClass.Archer; }
            stats = new PlayerStats(pc);
            return true;
        }

        public static void LoadGame() {

            PlayerStats gameStats = new PlayerStats();

            Dictionary<string, string> saveValues = ParseSaveValues();  //Populate the lists with our save data
            //^ - Create a dictionary of key pair values where
            //keys are the variable name and values in the dictionary are
            //the variable names actual values.

            foreach (string saveAttribute in saveValues.Keys) {
                foreach (string variableName in gameStats.GameVariables.Keys) {
                    if (saveAttribute.Equals(variableName)) {
                        gameStats.GameVariables[variableName] = saveValues[saveAttribute];
                        break;
                    }
                }    
            }
            stats = gameStats;
        }

        /// <summary>
        /// Populates the data of 2 List<string> objects.
        /// </summary>
        /// <param name="v">Values who's index's directly correspond to the attribute list.</param>
        /// <returns>Attribute Names</returns>
        public static Dictionary<string, string> ParseSaveValues() {
            Dictionary<string, string> saveValues = new Dictionary<string, string>();

            using (StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/save.txt")) {
                int counter = 0;
                string line;
                while ((line = sr.ReadLine()) != null) {
                    saveValues[(line.Substring(0, line.IndexOf(' ')))] = line.Substring(line.LastIndexOf(' ') + 1, line.Length - (line.LastIndexOf(' ') + 1));
                    counter++;
                }
            }

            return saveValues;
        }

        public static void WriteOnBottomLine(string text, int offset = 1, bool restoreCursorPosition = false) {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - offset;

            Console.Write(text);
            // Restore previous position
            if (restoreCursorPosition)
                Console.SetCursorPosition(x, y);
        }
    }
}
