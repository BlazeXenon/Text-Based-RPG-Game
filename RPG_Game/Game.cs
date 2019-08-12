using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RPG_Game 
{
    public class Game 
    {

        public static ForestArea forestArea;

        public bool ShouldGameRestart { get { return shouldGameRestart; } private set { shouldGameRestart = value; } }

        private bool shouldGameRestart = false;
        private PlayerStats ps;

        public Game(PlayerStats ps) 
        {
            this.ps = ps;
            SaveGame();
            forestArea = new ForestArea();

            shouldGameRestart = BeginGame();
        }

        private bool BeginGame() 
        {
            while (true)
            {
                if (ps.Health <= 0)
                    return true;
                Console.Clear();

                Console.WriteLine("\nCurrent Player Stats:\n\n");

                Program.ConsoleColorWriteLine($"/wName:/e {ps.Name}");
                Program.ConsoleColorWriteLine($"/wClass:/e {ps.PlayerClass}\n");


                Program.ConsoleColorWriteLine($"/rHealth:/e ({ps.Health}/{ps.MaxHealth})");
                Program.ConsoleColorWriteLine($"/cMana:/e ({ps.Mana}/{ps.MaxMana})\n");
                Program.ConsoleColorWriteLine($"/yLevel:/e {ps.Level + (ps.Level == 50 ? " (Max Level)" : "")}");
                Program.ConsoleColorWriteLine($"/wExperience:/e ({ps.Experience}/{ps.MaxExperience})\n");

                Program.ConsoleColorWriteLine($"/yGold:/e {ps.Gold}\n");

                Program.ConsoleColorWriteLine($"/RPower:/e {ps.Power}");
                Program.ConsoleColorWriteLine($"/gNimble:/e {ps.Nimble}");
                Program.ConsoleColorWriteLine($"/cMagic:/e {ps.Magic}");
                Program.ConsoleColorWriteLine($"/YCunning:/e {ps.Cunning}\n\n");

                Program.ConsoleColorWriteLine($"/wAvailable Skill Points:/e {ps.AvailableSkillPoints}");

                Menu.WriteOnBottomLine("What would you like to do?\n", 4, true);
                Menu.WriteOnBottomLine("/w(1)/e Enter Battle /w(2)/e Purchase Items /w(3)/e Save Game" + ((ps.AvailableSkillPoints > 0) ? " /w(4)/e Spend Skill Points " : " ") + "/w(99)/e Exit", 3, true);
                Menu.WriteOnBottomLine(">> ");
                var x = Console.ReadLine();
                int selection;
                if (int.TryParse(x, out selection))
                {
                    if (selection == 1)
                    {
                        Battle();
                    }
                    else if (selection == 2)
                    {

                    }
                    else if (selection == 3)
                    {
                        SaveGame();
                    }
                    else if (selection == 4)
                    {
                        if (ps.AvailableSkillPoints > 0)
                        {
                            AssignSkillPoints();
                        }
                    }
                    else if (selection == 99)
                    {
                        if (DoesSaveDataMatchCurrentData())
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine("Are you sure you want to quit? (y/N)\nNote: Your save data doesn't match your current stats.");
                            Console.Write(">> ");
                            if (Console.ReadLine().ToLowerInvariant() == "y")
                                Environment.Exit(0);
                        }
                    }
                    selection = -1;
                }
                else
                {
                    Console.WriteLine("\nInvalid Selection!\n");
                    Console.ReadKey();
                }
            }
        }

        private void Battle()
        {
            Console.Clear();

            Area currentArea = GetCurrentArea();
            BaseEnemy currentEnemy = currentArea.GenerateEnemy(ps.CurrentDifficultRating());


            //Enemy e = new Enemy(ps.CurrentDifficultRating());
            int timesPlayerCanAttemptFlee = 1;
            bool playerEscapeStatus = false;

            currentEnemy.InitializeEnemy();

            while (currentEnemy.Health > 0 && ps.Health > 0 && !playerEscapeStatus) 
            {
                bool ShouldEnemyAttackPlayer = true;
                Program.ConsoleColorWriteLine($"\n\n/wYou:/e (/rHealth:/e {ps.Health} /cMana:/e {ps.Mana}) ||| /wEnemy:/e (/rHealth:/e {currentEnemy.Health})");
                Program.ConsoleColorWriteLine("\n/w(1)/e Attack" + ((timesPlayerCanAttemptFlee > 0) ? " /w(2)/e Flee" : ""));
                Console.Write(">> ");
                var action = Console.ReadLine().TrimEnd();
                if (action == "1") {
                    int damageAmount = 0;
                    if (ps.PlayerClass == PlayerClass.Warrior) {
                        damageAmount = Program.InclusiveIntRNG(ps.Power - Program.InclusiveIntRNG(1, 3), ps.Power + Program.InclusiveIntRNG(1, 3));
                    } else if (ps.PlayerClass == PlayerClass.Mage) {
                        damageAmount = Program.InclusiveIntRNG(ps.Magic - Program.InclusiveIntRNG(1, 3), ps.Magic + Program.InclusiveIntRNG(1, 3));
                    } else if (ps.PlayerClass == PlayerClass.Archer) {
                        damageAmount = Program.InclusiveIntRNG(ps.Nimble - Program.InclusiveIntRNG(1, 3), ps.Nimble + Program.InclusiveIntRNG(1, 3));
                    }
                    Animation.RunAnimation(textToType: $"\nYou have dealt /w{damageAmount} damage/e!\n");
                    if (damageAmount < currentEnemy.Health)
                        Animation.RunAnimation(AnimationType.Dot, 200);
                    currentEnemy.Damage(damageAmount);
                } else if (action == "2") {
                    if (timesPlayerCanAttemptFlee > 0) {
                        timesPlayerCanAttemptFlee--;
                        Animation.RunAnimation(textToType: "\nYou Attempt to Escape\n", skipLine: false);
                        Animation.RunAnimation(AnimationType.Dot, 200);
                        Console.WriteLine();
                        if (Program.FloatRNG() > 0.5) {
                            Animation.RunAnimation(textToType: "The escape attempt was /wsuccessful/e!\n");
                            playerEscapeStatus = true;
                            ShouldEnemyAttackPlayer = false;
                        } else {
                            if (Program.FloatRNG() > 0.5) {
                                Animation.RunAnimation(textToType: "You attempted escape but while running away you hit a tree and your enemy caught up to you...\n");
                                Animation.RunAnimation(textToType: "Luckily, you manage to see him just before he swings!");
                                ShouldEnemyAttackPlayer = false;
                            } else {
                                int enemyDamage = currentEnemy.Attack();
                                Animation.RunAnimation(textToType: $"While attempting to escape, your enemy grabbed you and dealt /w{enemyDamage} damage/e to you!");
                                ps.AlterHealth(Operation.Subtract, enemyDamage);
                                ShouldEnemyAttackPlayer = false;
                            }
                        }
                    } else {
                        Animation.RunAnimation(AnimationType.TextTyping, 20, "You have no more escape attempts!");
                        ShouldEnemyAttackPlayer = false;
                    }
                } else {
                    Animation.RunAnimation(textToType: "Are you sure you want to skip your turn? (y/n)");
                    Animation.RunAnimation(textToType: ">> ", skipLine: false);
                    if (Console.ReadLine().Trim() != "y")
                        ShouldEnemyAttackPlayer = false;
                    Console.WriteLine();
                }

                if (currentEnemy.Health <= 0) break;

                if (ShouldEnemyAttackPlayer) {
                    int enemyDamage = currentEnemy.Attack();
                    Animation.RunAnimation(textToType: $"\nThe enemy lashes back and deals /w{enemyDamage} damage/e to you!");
                    ps.AlterHealth(Operation.Subtract, enemyDamage);
                }
            }

            if (!playerEscapeStatus) {
                float experienceGain = (Program.InclusiveIntRNG((currentEnemy.EnemyDifficulty + 1) * 2, (currentEnemy.EnemyDifficulty + 1) * 4) + Program.FloatRNG());
                experienceGain = (float)Math.Round(experienceGain, 2);

                if (ps.Health > 0 && currentEnemy.Health <= 0) {

                    Animation.Queue(new Animation(AnimationType.Dot, 200));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\nCongrats! You have defeated your foe,", false));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 200, " ", false));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, $"and as a result you have gained /w{experienceGain}/e experience!\n"));

                    Animation.Queue(new Animation(interval: 200, text: " "));
                    Animation.Queue(new Animation(interval: 20, text: TextVariations.GetRandomGoldText(currentEnemy.GetEnemyGoldYield()) + "\n"));

                    Animation.PlayQueue();
                    ps.AlterExperience(Operation.Add, experienceGain);
                    ps.AlterGold(Operation.Add, currentEnemy.GetEnemyGoldYield());
                } else if (ps.Health <= 0) {

                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\n\nYou have died in combat\n"));
                    Animation.Queue(new Animation(AnimationType.Dot, 100));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\nYou can begin a new game or start from your previous save.\n"));
                    Animation.PlayQueue();

                    Animation.RunAnimation(textToType: "\nStart from previous save? (Y/n)\n>> ", skipLine: false);
                    var choice = Console.ReadLine();
                    if (choice.ToLowerInvariant() == "n") {
                        System.Diagnostics.Process.Start(Application.ExecutablePath);
                        Environment.Exit(0);
                    } else {
                        Menu.LoadGame();
                    }
                }

                if ((ps.Level < ps.MAX_LEVEL) && (ps.Experience >= ps.MaxExperience)) {

                    int level_up_amount = 0;

                    // Calculate a multi-level scenerio
                    while (ps.Experience >= ps.MaxExperience) {
                        // Subtract the max amount of experience from the current experience
                        // i.e. Level 1 - 19.6/10 => Level 1 - 9.6/10
                        ps.AlterExperience(Operation.Subtract, ps.MaxExperience);

                        // Increase the Level by 1
                        // i.e. Level 1 - 9.6/10 => Level 2 - 9.6/10
                        ps.AlterLevel(Operation.Add, 1);

                        // Increase the Max Experience by calculation
                        // i.e. Level 2 - 9.6/10 => Level 2 - 9.6/11
                        ps.AlterMaxExperience(Operation.Set, (int)(Math.Pow(2, (1.0f / 8.0f) * ps.Level) * 10));

                        // Repeat while current experience is still greater then max experience.
                        level_up_amount += 1;
                    }

                    ps.AlterHealth(Operation.Set, ps.MaxHealth);
                    ps.AlterMana(Operation.Set, ps.MaxMana);
                    ps.AlterSkillPoints(Operation.Add, 3 * level_up_amount);

                    Animation.RunAnimation(AnimationType.Dot, 200);
                    Program.ConsoleColorWriteLine($"\nYour player has leveled up!!! You now have /w{ps.AvailableSkillPoints}/e skill points to spend!\n");
                } else {
                    if ((Program.FloatRNG() > 0.4) && (ps.Health != ps.MaxHealth) && (ps.Health > 0))
                    {
                        int healthRecovered = Program.InclusiveIntRNG(1,
                            (ps.MaxHealth / 2 >= 2) ? (ps.MaxHealth / 2) + 3 : (ps.MaxHealth / 2));
                        if (ps.Health + healthRecovered > ps.MaxHealth)
                            ps.AlterHealth(Operation.Set, ps.MaxHealth);
                        else
                            ps.AlterHealth(Operation.Add, healthRecovered);
                        Animation.RunAnimation(AnimationType.Dot, 200);
                        Animation.RunAnimation(textToType: $"\nYou have managed to recover some health! (/w{healthRecovered}/e health).\n");
                    }
                }

                Animation.RunAnimation(textToType: "\n/A<Press any key to return to town>/e", skipLine: false);
                Console.ReadKey();
            }
        }

        private void AssignSkillPoints() {
            bool isSpendingSkillPoints = true;
            string[] approvedSkillsToUpgrade = { "health", "mana", "power", "nimble", "magic", "cunning" };
            string[] greetingStrings = { "Come to train your skills, have you?", "What can I do for you?", "What are you looking to improve upon today?"};
            Console.Clear();
            if (!ps.HasPreviouslyAssignedSkillPoints) {
                Animation.RunAnimation(AnimationType.Dot, 400);
                Animation.RunAnimation(AnimationType.TextTyping, 50, "\n*You hear a voice off to the side*\n");
                Animation.RunAnimation(AnimationType.Dot, 300);
                Animation.RunAnimation(AnimationType.TextTyping, 50, "\nHello Adventurer! I am Elise, the training guide in this town.\n" + 
                                                                     "It seems as though you have a prospect to get stronger!\n\n" +
                                                                     "\t*You are surprised but nod in agreement*\n\n" +
                                                                     "Before we continue, tell me, what is your name adventurer?\n\n" + 
                                                                     $"\t*You tell Elise that your name is /w{ps.Name}/e*\n\n" + 
                                                                     "Ah, a wonderful name indeed!\nI'm sure we'll be fantastic friends for many years to come!\n" +
                                                                     "\nNow walk over here with me and I'll show you how to get stronger.");
                ps.HasPreviouslyAssignedSkillPoints = true;
                Console.ReadKey();
            }

            int gsn = Program.IntRNG(0, greetingStrings.Length);

            bool firstLoop = true;
            while (isSpendingSkillPoints) {
                Console.Clear();

                // Used to cancel animation for the subsequent times the player enters the menu AFTER the first time.
                // Basically used to prettify the skill menu.

                if (firstLoop) {
                    Animation.RunAnimation(textToType: $"/wElise: Greetings {ps.Name}! {greetingStrings[gsn]}/e\n");
                    firstLoop = false;
                } else {
                    Program.ConsoleColorWriteLine($"/wElise: Greetings {ps.Name}! {greetingStrings[gsn]}/e\n");
                }

                Program.ConsoleColorWriteLine($"You have /w{ps.AvailableSkillPoints}/e skill points to spend.\n\nYou can spend them by /wtyping the stat/e you would like to spend them on\nthen /wtyping the amount you would like to spend/e.");
                Program.ConsoleColorWriteLine("Once you are finished type \"/wback/e\" to reenter the town.");
                /*Console.Write("You have " + ps.AvailableSkillPoints + " skill points to spend.\n\nYou can spend them by ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("typing the chosen attribute ");
                Console.ResetColor();
                Console.Write("you would like to spend them\non then ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("typing the amount you would like to spend");
                Console.ResetColor();
                Console.WriteLine(", then once you are finished\n type \"back\" to reenter to the town.");*/
                Program.ConsoleColorWriteLine("(E.g.: /Rpower/e /w3/e)\n");
                Console.WriteLine("The available stats are as follows:\n");
                Program.ConsoleColorWriteLine("\t/rhealth/e, /cmana/e, /Rpower/e, /gnimble/e, /cmagic/e, /Ycunning/e\n");
                Program.ConsoleColorWriteLine($"Your Current Stats:\n\n\t/rMax Health/e: {ps.MaxHealth}\n\t/cMax Mana/e: {ps.MaxMana}\n\n\t/RPower/e: {ps.Power}\n\t/gNimble/e: {ps.Nimble}\n\t/cMagic/e: {ps.Magic}\n\t/YCunning/e: {ps.Cunning}\n\t");
                Console.Write(">> ");

                var userInput = Console.ReadLine();
                string[] parameters = userInput.Split(new char[] { ' ' });
                if (parameters.Length == 1 || parameters.Length >= 3) {
                    if (!parameters[0].ToLowerInvariant().Equals("back")) {
                        Animation.RunAnimation(textToType: "Invalid Syntax.");
                        Console.ReadKey();
                    } else {
                        isSpendingSkillPoints = false;
                    }
                } else {
                    int indexOfSkillToUpgrade = -1;
                    for (int i = 0; i < approvedSkillsToUpgrade.Length; i++) {
                        if (parameters[0].ToLowerInvariant().Equals(approvedSkillsToUpgrade[i])) {
                            indexOfSkillToUpgrade = i;
                        }
                    }

                    if (indexOfSkillToUpgrade == -1) { //Check to see if the parameter the user input was an actual stat
                        Animation.RunAnimation(textToType: "\nThat is not a skill you can upgrade!");
                        Console.ReadKey();
                    } else {
                        if (!int.TryParse(parameters[1], out int amount)) { //Check to see that the amount the user input was a number
                            Animation.RunAnimation(textToType: "\nThe amount you wish to upgrade the skill by is not a valid number.");
                            Console.ReadKey();
                        } else {
                            int amountToUpgradeBy = int.Parse(parameters[1]);
                            if (amountToUpgradeBy > ps.AvailableSkillPoints) { //Check to see that the amount input was not over the total amount available.
                                Animation.RunAnimation(textToType: "\nThe amount you wish to upgrade the skill by exceeds the amount of skill points\navailable to you!");
                                Console.ReadKey();
                            } else { //If all checks pass then search for and upgrade the skill
                                foreach (string statToUpgrade in ps.GameVariables.Keys) { // loops through all available game variables
                                    if (statToUpgrade.ToLowerInvariant().Equals(parameters[0].ToLowerInvariant())) { //if it finds one that matches (at this point it definitely should)
                                        if (statToUpgrade.ToLowerInvariant().Equals("health")) {
                                            int currentValueOfStat = -1;
                                            PlayerStats.StringToInt(ps.GameVariables["maxHealth"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["maxHealth"] = currentValueOfStat + amountToUpgradeBy; //then add the desired amount to that skill.
                                            PlayerStats.StringToInt(ps.GameVariables["health"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["health"] = currentValueOfStat + amountToUpgradeBy;
                                            break;
                                        } else if (statToUpgrade.ToLowerInvariant().Equals("mana")) {
                                            int currentValueOfStat = -1;
                                            PlayerStats.StringToInt(ps.GameVariables["maxMana"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["maxMana"] = currentValueOfStat + amountToUpgradeBy; //then add the desired amount to that skill.
                                            PlayerStats.StringToInt(ps.GameVariables["mana"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["mana"] = currentValueOfStat + amountToUpgradeBy;
                                            break;
                                        } else {
                                            int currentValueOfStat = -1;
                                            PlayerStats.StringToInt(ps.GameVariables[statToUpgrade].ToString(), out currentValueOfStat);
                                            ps.GameVariables[statToUpgrade] = currentValueOfStat + amountToUpgradeBy; //then add the desired amount to that skill.
                                            break;
                                        }
                                        
                                    }
                                }
                                ps.AlterSkillPoints(Operation.Subtract, amountToUpgradeBy);
                                Animation.RunAnimation(textToType: "\nUpgrade Successful!");
                                Console.ReadKey();
                            }
                        }
                    }
                }
            }
            
        }
        public void SaveGame()
        {
            string gamesave = "name = " + ps.Name + Environment.NewLine
                              + "class = " + ps.PlayerClass + Environment.NewLine
                              + "health = " + ps.Health + Environment.NewLine
                              + "maxHealth = " + ps.MaxHealth + Environment.NewLine
                              + "mana = " + ps.Mana + Environment.NewLine
                              + "maxMana = " + ps.MaxMana + Environment.NewLine
                              + "power = " + ps.Power + Environment.NewLine
                              + "nimble = " + ps.Nimble + Environment.NewLine
                              + "magic = " + ps.Magic + Environment.NewLine
                              + "cunning = " + ps.Cunning + Environment.NewLine
                              + "level = " + ps.Level + Environment.NewLine
                              + "experience = " + ps.Experience + Environment.NewLine
                              + "maxExperience = " + ps.MaxExperience + Environment.NewLine
                              + "gold = " + ps.Gold + Environment.NewLine
                              + "availableSkillPoints = " + ps.AvailableSkillPoints + Environment.NewLine
                              + "promptSkillPoints = " + ps.HasPreviouslyAssignedSkillPoints;

            File.WriteAllText(Directory.GetCurrentDirectory() + "/save.txt", gamesave);
        }

        private bool DoesSaveDataMatchCurrentData() {
            Dictionary<string, string> currentSaveValues = Menu.ParseSaveValues();

            foreach (string saveKey in currentSaveValues.Keys) {
                foreach (string actualKey in ps.GameVariables.Keys) {
                    if (saveKey.Equals(actualKey)) {
                        if (currentSaveValues[saveKey] != (string)ps.GameVariables[actualKey].ToString()) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private Area GetCurrentArea()
        {
            if (ps.Level >= 1 && ps.Level < 10) // 1 - 9
            {
                return forestArea;
            }
            else if (ps.Level >= 10 && ps.Level < 20) // 10 - 19
            {
                return forestArea;
            }
            else if (ps.Level >= 20 && ps.Level < 30) // 20 - 29
            {
                return forestArea;
            }
            else if (ps.Level >= 30 && ps.Level < 40) // 30 - 39
            {
                return forestArea;
            }
            else if (ps.Level >= 40 && ps.Level <= 50) // 40 - 50
            {
                return forestArea;
            }
            else
            {
                Console.WriteLine($"Level: {ps.Level}");
                throw new NullReferenceException("Your level is not within 1-50! Corrupt data or Cheating.");
            }
        }
    }
}
