using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Transactions;
using System.Windows.Forms;

namespace RPG_Game 
{
    public class Menu
    {
        public static int GameSaveId = -1;

        public int choice = -1;
        private bool playMenuAnimation = true;

        private static PlayerStats stats;
        private bool hasGameSave = false;

        public Menu() { }

        public void Start() 
        {
            // Cache whether or not the player has a saved game.
            string[] timestamps = DatabaseHelper.instance.GetLoadScreenData();
            if (timestamps.Length > 0) hasGameSave = true;

            bool continuePlay = true;
            while (continuePlay) 
            {
                while (choice <= 0) 
                {
                    Console.Title = "RPG Game V" + Program.VersionNumber;
                    LoadScreen(playMenuAnimation);
                    Console.Write(">> ");
                    var input = Console.ReadLine();

                    if (int.TryParse(input, out choice)) 
                    {
                        if (choice == 1) 
                        {
                            var success = NewGame();
                            if (!success)
                                choice = -1;
                        }
                        else if (choice == 2 && hasGameSave)
                        {
                            bool successfulLoad = LoadGame();
                            if (!successfulLoad)
                                choice = -1;
                        }
                        else if (choice == 99) 
                        {
                            Environment.Exit(0);
                        }
                        else 
                        {
                            Animation.RunAnimation(textToType: "\n\nInvalid Input.\n");
                            Console.ReadKey();
                            choice = -1;
                        }
                    }
                    else if (!input.Trim().Equals("")) 
                    {
                        Animation.RunAnimation(textToType: "\n\nInvalid Input.\n");
                        Console.ReadKey();
                        choice = -1;
                    }
                }
                Game game = new Game(stats);
                if (!game.ShouldGameRestart)
                    continuePlay = false;
            }
        }

        private void LoadScreen(bool useAnimation) 
        {
            Console.Clear();
            if (useAnimation) 
            {
                Animation.Queue(new Animation(AnimationType.TextTyping, 80, "\t/wWelcome to an/e\n"));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"____/\\\\\\\\\______/\\\\\\\\\\\\\_______/\\\\\\\\\\\\_        "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @" __/\\\///////\\\___\/\\\/////////\\\___/\\\//////////__       "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"  _\/\\\_____\/\\\___\/\\\_______\/\\\__/\\\_____________      "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"   _\/\\\\\\\\\\\/____\/\\\\\\\\\\\\\/__\/\\\____/\\\\\\\_     "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"    _\/\\\//////\\\____\/\\\/////////____\/\\\___\/////\\\_    "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"     _\/\\\____\//\\\___\/\\\_____________\/\\\_______\/\\\_   "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"      _\/\\\_____\//\\\__\/\\\_____________\/\\\_______\/\\\_  "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"       _\/\\\______\//\\\_\/\\\_____________\//\\\\\\\\\\\\/__ "));
                Animation.Queue(new Animation(AnimationType.TextTyping, 2, @"        _\///________\///__\///_______________\////////////____\n"));
                Animation.Queue(new Animation(AnimationType.TextTyping, 80, "\t\t\t\t\t/wGame./e\n\n\n"));
                Animation.PlayQueue();
                playMenuAnimation = false;
            } 
            else 
            {
                Program.ConsoleColorWriteLine("\t/wWelcome to an/e\n");
                Console.WriteLine(@"____/\\\\\\\\\______/\\\\\\\\\\\\\_______/\\\\\\\\\\\\_        ");
                Console.WriteLine(@" __/\\\///////\\\___\/\\\/////////\\\___/\\\//////////__       ");
                Console.WriteLine(@"  _\/\\\_____\/\\\___\/\\\_______\/\\\__/\\\_____________      ");
                Console.WriteLine(@"   _\/\\\\\\\\\\\/____\/\\\\\\\\\\\\\/__\/\\\____/\\\\\\\_     ");
                Console.WriteLine(@"    _\/\\\//////\\\____\/\\\/////////____\/\\\___\/////\\\_    ");
                Console.WriteLine(@"     _\/\\\____\//\\\___\/\\\_____________\/\\\_______\/\\\_   ");
                Console.WriteLine(@"      _\/\\\_____\//\\\__\/\\\_____________\/\\\_______\/\\\_  ");
                Console.WriteLine(@"       _\/\\\______\//\\\_\/\\\_____________\//\\\\\\\\\\\\/__ ");
                Console.WriteLine(@"        _\///________\///__\///_______________\////////////____" + "\n");
                Program.ConsoleColorWriteLine("\t\t\t\t\t/wGame./e\n\n\n");
            }

            // New
            Program.ConsoleColorWriteLine($"/w(1)/e New Game {(hasGameSave ? "/w(2)/e Load Game " : "")}/w(99)/e Exit");

            // Old
            //Program.ConsoleColorWriteLine("/w(1)/e New Game " + (File.Exists(Directory.GetCurrentDirectory() + "/save.txt") ? "/w(2)/e Load Game " : "") + "/w(99)/e Exit");
        }

        private bool NewGame()
        {
            string[] classDescriptions =
            {
                "/rWarrior/e: What some call a fearsome brute, this class thrives in health and strength.\n",
                "/cMage/e: A powerful and deadly force, these people specialize in the art of magic.\n",
                "/gArcher/e: Swift and nimble, the archer's bow allows them to pick enemies off from a distance.\n"
            };
            string[] adjectives = {"Fantastic", "Marvelous", "Superb", "Sensational"};

            string playerClass = "";
            string playerName = "";
            PlayerClass pc = PlayerClass.Undefined;

            bool firstAnimationHasBeenPlayed = false,
                secondAnimationHasBeenPlayed = false;

            while (playerClass == "")
            {
                Console.Clear();

                if (!firstAnimationHasBeenPlayed)
                {
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[0] + "\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[1] + "\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8, classDescriptions[2] + "\n\n"));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8,
                        "(use the command \"/wback/e\" to return to the previous screen)\n"));
                    Animation.PlayQueue();
                    firstAnimationHasBeenPlayed = true;
                }
                else
                {
                    Program.ConsoleColorWriteLine(classDescriptions[0] + "\n");
                    Program.ConsoleColorWriteLine(classDescriptions[1] + "\n");
                    Program.ConsoleColorWriteLine(classDescriptions[2] + "\n\n");
                    Program.ConsoleColorWriteLine("(use the command \"/wback/e\" to return to the previous screen)\n");
                }

                WriteOnBottomLine("Please Choose Your Class:\n", 2);
                WriteOnBottomLine(">> ");
                var pClass = Console.ReadLine();
                if (pClass.Length > 0 && (pClass.ToLowerInvariant() == "warrior" ||
                                          pClass.ToLowerInvariant() == "mage" || pClass.ToLowerInvariant() == "archer"))
                {
                    playerClass = pClass.ToLowerInvariant();
                }
                else if (pClass.ToLowerInvariant().Equals("back"))
                {
                    return false;
                }
                else if (!pClass.Trim().Equals(""))
                {
                    Animation.RunAnimation(textToType: "\n\n\nInvalid Class Selection.\n");
                    Console.ReadKey();
                }
            }

            int adjectiveChosen = Program.IntRNG(0, adjectives.Length);
            string greetingStr = "";
            if (playerClass == "warrior")
            {
                pc = PlayerClass.Warrior;
                greetingStr = "/rWarrior/e";
            }
            else if (playerClass == "mage")
            {
                pc = PlayerClass.Mage;
                greetingStr = "/cMage/e";
            }
            else if (playerClass == "archer")
            {
                pc = PlayerClass.Archer;
                greetingStr = "/gArcher/e";
            }

            while (playerName == "")
            {
                Console.Clear();

                if (!secondAnimationHasBeenPlayed)
                {
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8,
                        $"Greetings Adventurer! Ah, so you've decided to be a {greetingStr}, huh? {adjectives[adjectiveChosen]} Choice Adventurer!\n"));
                    Animation.Queue(new Animation(AnimationType.Dot, 20));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 8,
                        "\nSpeaking of which, what is your /wname/e adventurer?\n"));
                    Animation.PlayQueue();
                    secondAnimationHasBeenPlayed = true;
                }
                else
                {
                    Program.ConsoleColorWriteLine(
                        $"Greetings Adventurer! Ah, so you've decided to be a {greetingStr}, huh? {adjectives[adjectiveChosen]} Choice Adventurer!\n\n. . . \n");
                    Program.ConsoleColorWriteLine("Speaking of which, what is your /wname/e adventurer?\n");
                }

                WriteOnBottomLine("Character Name:\n", 2);
                WriteOnBottomLine(">> ");
                string pName = Console.ReadLine();

                if (pName == string.Empty || pName.Length < 2)
                    continue;

                Console.Clear();
                Program.ConsoleColorWriteLine($"Are you sure you want to name your character \"/w{pName}/e\"? (y/N)");
                WriteOnBottomLine(">> ");
                string choice = Console.ReadLine();
                if (choice.ToLowerInvariant() == "y")
                {
                    playerName = pName;
                    break;
                }
            }


            new Inventory();
            stats = new PlayerStats(pc, playerName);

            DateTime currentTime = DateTime.Now;

            bool successfulTransaction = DatabaseHelper.instance.StoreMultipleGameData(
                new []{Tables.Saves, Tables.Inventory},
                new []{ currentTime, currentTime }, 
                new List<byte[]> { Program.Serialize(stats), Program.Serialize(Inventory.instance) },
                out int lastId
                );

            //DatabaseHelper.instance.StoreGameData(Tables.Saves, currentTime, Program.Serialize(stats));
            //GameSaveId = DatabaseHelper.instance.StoreGameData(Tables.Inventory, currentTime, Program.Serialize(Inventory.instance));
            if (lastId != -1)
                GameSaveId = lastId;

            return successfulTransaction;
        }

        public static bool LoadGame(bool reload = false)
        {
            string[] saves = DatabaseHelper.instance.GetLoadScreenData();

            while (true)
            {
                Console.Clear();
                Program.ConsoleColorWriteLine($"Please choose which save you would like to load{(!reload ? " (or \"/wback/e\")" : "")}:\n");

                for (int i = 0; i < saves.Length; i++)
                    Program.ConsoleColorWriteLine($"\t({(i + 1)}) {saves[i]}");

                Console.Write("\n>> ");
                string userChoice = Console.ReadLine();
                Console.WriteLine();

                if (userChoice != null)
                {
                    if (userChoice.ToLowerInvariant().Trim() == "back" && reload == false)
                        return false;

                    if (int.TryParse(userChoice, out int parsedChoice))
                    {
                        // Store table id for both entries (parallel data entries for inventory and saves table)

                        // Load PlayerStats and Inventory into Memory from database.
                        byte[] serializedInventoryData = DatabaseHelper.instance.RetrieveData(Tables.Inventory, parsedChoice);
                        byte[] serializedStatData = DatabaseHelper.instance.RetrieveData(Tables.Saves, parsedChoice);

                        if (serializedInventoryData == null || serializedStatData == null)
                        {
                            Console.WriteLine("Invalid Input: That save does not exist!");
                            Console.ReadKey();
                            continue;
                        }

                        GameSaveId = parsedChoice;
                        /* 
                         *
                         * SQL SERVER WAY
                         *
                         *
                        parsedChoice -= 1; // Convert from user number to array index value.
                        
                        // Store table id for both entries (parallel data entries for inventory and saves table)
                        GameSaveId = parsedChoice;

                        // Load PlayerStats and Inventory into Memory from database.
                        byte[] serializedInventoryData = DatabaseHelper.instance.RetrieveData(Tables.Inventory, parsedChoice);
                        byte[] serializedStatData = DatabaseHelper.instance.RetrieveData(Tables.Saves, parsedChoice);*/

                        Inventory.instance = Program.Deserialize(serializedInventoryData) as Inventory;
                        stats = Program.Deserialize(serializedStatData) as PlayerStats;

                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input: That is not a number.");
                        Console.ReadKey();
                    }
                }

            }

            /* Old Method
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
            stats = gameStats;*/
        }

        /// <summary>
        /// Populates the data of 2 List<string> objects.
        /// </summary>
        /// <param name="v">Values who's index's directly correspond to the attribute list.</param>
        /// <returns>Attribute Names</returns>
        public static Dictionary<string, string> ParseSaveValues() // DEPRECATED
        {
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

        public static void WriteOnBottomLine(string text, int offset = 1, bool restoreCursorPosition = false) 
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - offset;

            Program.ConsoleColorWrite(text);
            // Restore previous position
            if (restoreCursorPosition)
                Console.SetCursorPosition(x, y);
        }
    }
}
