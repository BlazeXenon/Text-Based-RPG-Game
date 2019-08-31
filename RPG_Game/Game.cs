using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using System.Windows.Forms;

namespace RPG_Game 
{
    public class Game 
    {

        public static ForestArea forestArea;

        public bool ShouldGameRestart { get => shouldGameRestart; private set => shouldGameRestart = value; }

        private bool shouldGameRestart = false;
        public static PlayerStats ps;

        public Game(PlayerStats playerStats) 
        {
            ps = playerStats;
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
                Program.ConsoleColorWriteLine($"/wExperience:/e ({Math.Round(ps.Experience, 2)}/{ps.MaxExperience})\n");

                Program.ConsoleColorWriteLine($"/RPower:/e {ps.Power}");
                Program.ConsoleColorWriteLine($"/gNimble:/e {ps.Nimble}");
                Program.ConsoleColorWriteLine($"/cMagic:/e {ps.Magic}");
                Program.ConsoleColorWriteLine($"/YCunning:/e {ps.Cunning}\n\n");

                Program.ConsoleColorWriteLine($"/yGold:/e {ps.Gold}");
                Program.ConsoleColorWriteLine($"/wAvailable Skill Points:/e {ps.AvailableSkillPoints}");

                Menu.WriteOnBottomLine("What would you like to do?\n", 4, true);
                Menu.WriteOnBottomLine("/w(1)/e Enter Battle /w(2)/e Purchase Items /w(3)/e Save Game" + ((ps.AvailableSkillPoints > 0) ? " /w(4)/e Spend Skill Points " : " ") + "/w(99)/e Exit", 3, true);
                Menu.WriteOnBottomLine(">> ");
                var x = Console.ReadLine();
                if (int.TryParse(x, out int selection))
                {
                    if (selection == 1)
                    {
                        Battle();
                    }
                    else if (selection == 2)
                    {
                        Shop();
                    }
                    else if (selection == 3)
                    {
                        Animation.RunAnimation(textToType: SaveGame() ? "\nSuccessfully Saved Game!" : "\nUnable to Save Game.");
                        Console.ReadKey();
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
            }
        }

        private void Battle()
        {
            Console.Clear();

            Area currentArea = GetCurrentArea();
            BaseEnemy currentEnemy = currentArea.GenerateEnemy(ps.CurrentDifficultRating());

            int timesPlayerCanAttemptFlee = 1;
            bool playerEscapeStatus = false;

            currentEnemy.InitializeEnemy();

            while (currentEnemy.Health > 0 && ps.Health > 0 && !playerEscapeStatus) 
            {
                bool ShouldEnemyAttackPlayer = true;
                Program.ConsoleColorWriteLine($"\n\n/wYou:/e (/rHealth:/e {ps.Health}/{ps.MaxHealth} /cMana:/e {ps.Mana}/{ps.MaxMana}) ||| /wEnemy:/e (/rHealth:/e {currentEnemy.Health})");
                Program.ConsoleColorWriteLine($"\n/w(1)/e Attack /w(2)/e Items{(timesPlayerCanAttemptFlee > 0 ? " /w(3)/e Flee" : "")}");
                Console.Write(">> ");
                var action = Console.ReadLine().TrimEnd();
                if (action == "1")
                {
                    int damageAmount = 0;
                    if (ps.PlayerClass == PlayerClass.Warrior)
                        damageAmount = Program.InclusiveIntRNG(ps.Power - Program.InclusiveIntRNG(1, 3),
                            ps.Power + Program.InclusiveIntRNG(1, 3));
                    else if (ps.PlayerClass == PlayerClass.Mage)
                        damageAmount = Program.InclusiveIntRNG(ps.Magic - Program.InclusiveIntRNG(1, 3),
                            ps.Magic + Program.InclusiveIntRNG(1, 3));
                    else if (ps.PlayerClass == PlayerClass.Archer)
                        damageAmount = Program.InclusiveIntRNG(ps.Nimble - Program.InclusiveIntRNG(1, 3),
                            ps.Nimble + Program.InclusiveIntRNG(1, 3));

                    Animation.Queue(new Animation(text: $"\nYou have dealt /w{damageAmount} damage/e!\n"));
                    if (damageAmount < currentEnemy.Health)
                        Animation.Queue(new Animation(AnimationType.Dot, 200));
                    currentEnemy.Damage(damageAmount);
                }
                else if (action == "2")
                {
                    Animation.Queue(new Animation(text: "\n-----------------", interval: 15));
                    Animation.Queue(new Animation(text: "Your Consumables:", interval: 15));
                    Animation.Queue(new Animation(text: "-----------------", interval: 15));
                    List<Item> usableItems = WriteUseItems();
                    if (usableItems.Count >= 1)
                    {
                        Animation.Queue(new Animation(text: "\n(Type the number of the consumable you would like to use, or \"/wback/e\")\n>> ", skipLine: false));
                        Animation.PlayQueue();
                        string userInput = Console.ReadLine();
                        if (userInput.ToLowerInvariant().Trim() == "back")
                            continue;
                        if (int.TryParse(userInput, out int index))
                        {
                            index -= 1; // Convert to index instead of gui number.
                            Item itemToRemove = (Item)Activator.CreateInstance(usableItems[index].GetType());
                            itemToRemove.Quantity = 1;

                            Console.WriteLine();
                            Animation.Queue(new Animation(AnimationType.Dot, interval:100));
                            Animation.Queue(new Animation(text: $"\n{itemToRemove.UseAction()}\n"));
                            Animation.Queue(new Animation(AnimationType.Dot, interval: 50));
                            Animation.PlayQueue();

                            Inventory.instance.RemoveItem(itemToRemove);
                        }
                        else
                        {
                            Console.WriteLine("Invalid Number.");
                        }
                    }
                    ShouldEnemyAttackPlayer = false;
                }
                else if (action == "3") 
                {
                    if (timesPlayerCanAttemptFlee > 0) 
                    {
                        timesPlayerCanAttemptFlee--;
                        Animation.Queue(new Animation(text: "\nYou Attempt to Escape\n", skipLine: true));
                        Animation.Queue(new Animation(AnimationType.Dot, 200));
                        Console.WriteLine();
                        if (Program.FloatRNG() > 0.5) 
                        {
                            Animation.Queue(new Animation(text: "\nThe escape attempt was /wsuccessful/e!\n"));
                            playerEscapeStatus = true;
                            ShouldEnemyAttackPlayer = false;

                            Animation.Queue(new Animation(text: "\n/A<Press any key to return to town>/e", skipLine: false));
                            Animation.PlayQueue();
                            Console.ReadKey();
                        } 
                        else 
                        {
                            if (Program.FloatRNG() > 0.5) 
                            {
                                Animation.Queue(new Animation(text: "\nYou attempted escape but while running away you hit a tree and your enemy caught up to you...\n"));
                                Animation.Queue(new Animation(text: "Luckily, you manage to see him just before he swings!"));
                                Animation.PlayQueue();
                                ShouldEnemyAttackPlayer = false;
                            } 
                            else 
                            {
                                int enemyDamage = currentEnemy.Attack();
                                Animation.Queue(new Animation(text: $"\nWhile attempting to escape, your enemy grabbed you and dealt /w{enemyDamage} damage/e to you!"));
                                Animation.PlayQueue();
                                ps.AlterHealth(Operation.Subtract, enemyDamage);
                                ShouldEnemyAttackPlayer = false;
                            }
                        }
                    } 
                    else 
                    {
                        Animation.RunAnimation(AnimationType.TextTyping, 20, "You have no more escape attempts!");
                        ShouldEnemyAttackPlayer = false;
                    }
                } 
                else 
                {
                    Animation.RunAnimation(textToType: "Are you sure you want to skip your turn? (y/N)");
                    Animation.RunAnimation(textToType: ">> ", skipLine: false);
                    if (Console.ReadLine().Trim() != "y")
                        ShouldEnemyAttackPlayer = false;
                    Console.WriteLine();
                }

                if (currentEnemy.Health <= 0) break;

                if (ShouldEnemyAttackPlayer) 
                {
                    int enemyDamage = currentEnemy.Attack();
                    Animation.Queue(new Animation(text: $"\nThe enemy lashes back and deals /w{enemyDamage} damage/e to you!"));
                    ps.AlterHealth(Operation.Subtract, enemyDamage);
                    if (ps.Health > 0)
                        Animation.PlayQueue();
                }
            }

            if (!playerEscapeStatus) 
            {
                float experienceGain = (Program.InclusiveIntRNG((currentEnemy.EnemyDifficulty + 1) * 2, (currentEnemy.EnemyDifficulty + 1) * 4) + Program.FloatRNG());
                experienceGain = (float)Math.Round(experienceGain, 2);

                // If the enemy is killed, and the player is still alive.
                if (ps.Health > 0 && currentEnemy.Health <= 0) 
                {
                    Animation.Queue(new Animation(AnimationType.Dot, 200));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\nCongrats! You have defeated your foe,", false));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 200, " ", false));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, $"and as a result you have gained /w{experienceGain}/e experience!\n"));

                    Animation.Queue(new Animation(interval: 200, text: " "));
                    Animation.Queue(new Animation(interval: 20, text: TextVariations.GetRandomGoldText(currentEnemy.GetEnemyGoldYield()) + "\n"));

                    ps.AlterExperience(Operation.Add, experienceGain);
                    ps.AlterGold(Operation.Add, currentEnemy.GetEnemyGoldYield());

                    if ((ps.Level < ps.MAX_LEVEL) && (ps.Experience >= ps.MaxExperience)) 
                    {
                        int level_up_amount = 0;

                        // Calculate a multi-level scenario
                        while (ps.Experience >= ps.MaxExperience) 
                        {
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

                        Animation.Queue(new Animation(AnimationType.Dot, 200));
                        Animation.Queue(new Animation(text: $"\nYour player has leveled up!!! You now have /w{ps.AvailableSkillPoints}/e skill points to spend!\n"));
                    } 
                    else 
                    {
                        if ((Program.FloatRNG() > 0.4) && (ps.Health != ps.MaxHealth) && (ps.Health > 0))
                        {
                            int healthRecovered = Program.InclusiveIntRNG(1,
                                (ps.MaxHealth / 2 >= 2) ? (ps.MaxHealth / 2) + 3 : (ps.MaxHealth / 2));
                            if (ps.Health + healthRecovered > ps.MaxHealth)
                                ps.AlterHealth(Operation.Set, ps.MaxHealth);
                            else
                                ps.AlterHealth(Operation.Add, healthRecovered);
                            Animation.Queue(new Animation(AnimationType.Dot, 200));
                            Animation.Queue(new Animation(text: $"\nYou have managed to recover some health! (/w{healthRecovered}/e health).\n"));
                        }
                    }

                    Animation.Queue(new Animation(text: "\n/A<Press any key to return to town>/e", skipLine: false));
                    Animation.PlayQueue();
                    Console.ReadKey();
                } 
                else if (ps.Health <= 0) // If the player is killed.
                {
                    Animation.Queue(new Animation(interval: 1));
                    Animation.Queue(new Animation(AnimationType.Dot, 100));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\nYou have died in combat\n"));
                    Animation.Queue(new Animation(AnimationType.Dot, 100));
                    Animation.Queue(new Animation(AnimationType.TextTyping, 20, "\nYou can begin a new game or start from your previous save.\n"));
                    Animation.PlayQueue();

                    Animation.RunAnimation(textToType: "\n/wStart from previous save? (Y/n)/e\n>> ", skipLine: false);
                    var choice = Console.ReadLine();
                    if (choice.ToLowerInvariant() == "n") 
                    {
                        System.Diagnostics.Process.Start(Application.ExecutablePath);
                        Environment.Exit(0);
                    } 
                    else 
                    {
                        Menu.LoadGame(true);
                    }
                }
            }
        }

        private void AssignSkillPoints() 
        {
            bool isSpendingSkillPoints = true;
            string[] approvedSkillsToUpgrade = { "health", "mana", "power", "nimble", "magic", "cunning" };
            string[] greetingStrings = { "Come to train your skills, have you?", "What can I do for you?", "What are you looking to improve upon today?"};
            Console.Clear();
            if (!ps.HasPreviouslyAssignedSkillPoints) 
            {
                Animation.Queue(new Animation(AnimationType.Dot, 400));
                Animation.Queue(new Animation(AnimationType.TextTyping, 50, "\n*You hear a voice off to the side*\n"));
                Animation.Queue(new Animation(AnimationType.Dot, 300));
                Animation.PlayQueue();

                Animation.RunAnimation(AnimationType.TextTyping, 50, "\n/wElise/e: Hello Adventurer! I am /wElise/e, the training guide in this town.\n" + 
                                                                     "/wElise/e: It seems as though you have a prospect to get stronger!\n\n" +
                                                                     "\t*You are surprised but nod in agreement*\n\n" +
                                                                     "/wElise/e: Before we continue, tell me, what is your name adventurer?\n\n" + 
                                                                     $"\t*You tell Elise that your name is /w{ps.Name}/e*\n\n" + 
                                                                     "/wElise/e: Ah, a wonderful name indeed!\n/wElise/e: I'm sure we'll be fantastic friends for many years to come!\n" +
                                                                     "\n/wElise/e: Now walk over here with me and I'll show you how to get stronger.");
                ps.HasPreviouslyAssignedSkillPoints = true;
                Console.ReadKey();
            }

            int gsn = Program.IntRNG(0, greetingStrings.Length);

            bool firstLoop = true;
            while (isSpendingSkillPoints) 
            {
                Console.Clear();

                // Used to cancel animation for the subsequent times the player enters the menu AFTER the first time.
                // Basically used to prettify the skill menu.

                if (firstLoop) 
                {
                    Animation.RunAnimation(textToType: $"/wElise/e: Greetings /w{ps.Name}/e! {greetingStrings[gsn]}\n");
                    firstLoop = false;
                } 
                else 
                    Program.ConsoleColorWriteLine($"/wElise/e: Greetings /w{ps.Name}/e! {greetingStrings[gsn]}\n");

                Program.ConsoleColorWriteLine($"\n * You can spend skill points by /Rtyping the stat/e you would like to spend them on\n   then /wtyping the amount you would like to spend/e.");
                Program.ConsoleColorWriteLine("   Once you are finished type \"/wback/e\" to reenter the town.");
                Program.ConsoleColorWriteLine("   (E.g.: /Rpower/e /w3/e)\n");
                Program.ConsoleColorWriteLine($"Your Current Stats:\n\n\t/rMax Health/e: {ps.MaxHealth}\n\t/cMax Mana/e: {ps.MaxMana}\n\n\t/RPower/e: {ps.Power}\n\t/gNimble/e: {ps.Nimble}\n\t/cMagic/e: {ps.Magic}\n\t/YCunning/e: {ps.Cunning}\n");
                Console.WriteLine("The available stats are as follows:\n");
                Program.ConsoleColorWriteLine("\t/rhealth/e, /cmana/e, /Rpower/e, /gnimble/e, /cmagic/e, /Ycunning/e\n");
                Program.ConsoleColorWriteLine($"\nYou have /w{ps.AvailableSkillPoints}/e skill points to spend.\n");
                Console.Write(">> ");

                var userInput = Console.ReadLine();
                string[] parameters = userInput.Split(new char[] { ' ' });
                if (parameters.Length == 1 || parameters.Length >= 3) 
                {
                    if (!parameters[0].ToLowerInvariant().Equals("back")) 
                    {
                        Animation.RunAnimation(textToType: "Invalid Syntax.");
                        Console.ReadKey();
                    } 
                    else 
                        isSpendingSkillPoints = false;
                } 
                else 
                {
                    int indexOfSkillToUpgrade = -1;
                    for (int i = 0; i < approvedSkillsToUpgrade.Length; i++) 
                        if (parameters[0].ToLowerInvariant().Equals(approvedSkillsToUpgrade[i])) 
                            indexOfSkillToUpgrade = i;

                    if (indexOfSkillToUpgrade == -1) //Check to see if the parameter the user input was an actual stat
                    { 
                        Animation.RunAnimation(textToType: "\nThat is not a skill you can upgrade!");
                        Console.ReadKey();
                    } 
                    else 
                    {
                        if (!int.TryParse(parameters[1], out int amount)) 
                        { //Check to see that the amount the user input was a number
                            Animation.RunAnimation(textToType: "\nThe amount you wish to upgrade the skill by is not a valid number.");
                            Console.ReadKey();
                        } 
                        else 
                        {
                            int amountToUpgradeBy = int.Parse(parameters[1]);

                            //Check to see that the amount input was not over the total amount available.
                            if (amountToUpgradeBy > ps.AvailableSkillPoints) 
                            { 
                                Animation.RunAnimation(textToType: "\nThe amount you wish to upgrade the skill by exceeds the amount of skill points\navailable to you!");
                                Console.ReadKey();
                            } 
                            else //If all checks pass then search for and upgrade the skill
                            { 
                                foreach (string statToUpgrade in ps.GameVariables.Keys) // loops through all available game variables
                                {
                                    if (statToUpgrade.ToLowerInvariant().Equals(parameters[0].ToLowerInvariant())) //if it finds one that matches (at this point it definitely should)
                                    { 
                                        if (statToUpgrade.ToLowerInvariant().Equals("health")) 
                                        {
                                            PlayerStats.StringToInt(ps.GameVariables["maxHealth"].ToString(), out var currentValueOfStat);
                                            ps.GameVariables["maxHealth"] = currentValueOfStat + amountToUpgradeBy; //then add the desired amount to that skill.
                                            PlayerStats.StringToInt(ps.GameVariables["health"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["health"] = currentValueOfStat + amountToUpgradeBy;
                                            break;
                                        } 
                                        else if (statToUpgrade.ToLowerInvariant().Equals("mana")) 
                                        {
                                            PlayerStats.StringToInt(ps.GameVariables["maxMana"].ToString(), out var currentValueOfStat);
                                            ps.GameVariables["maxMana"] = currentValueOfStat + amountToUpgradeBy; //then add the desired amount to that skill.
                                            PlayerStats.StringToInt(ps.GameVariables["mana"].ToString(), out currentValueOfStat);
                                            ps.GameVariables["mana"] = currentValueOfStat + amountToUpgradeBy;
                                            break;
                                        } 
                                        else 
                                        {
                                            PlayerStats.StringToInt(ps.GameVariables[statToUpgrade].ToString(), out var currentValueOfStat);
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
        public bool SaveGame()
        {
            DateTime currentTime = DateTime.Now;

            bool successfulSave = DatabaseHelper.instance.UpdateMultipleGameData(
                new []{ Tables.Saves, Tables.Inventory }, 
                new []{ Menu.GameSaveId, Menu.GameSaveId }, 
                new []{ currentTime, currentTime }, 
                new List<byte[]> { Program.Serialize(ps), Program.Serialize(Inventory.instance)}
                );

            return successfulSave;
            //DatabaseHelper.instance.UpdateGameData(Tables.Saves, Menu.GameSaveId, currentTime, Program.Serialize(ps));
            //DatabaseHelper.instance.UpdateGameData(Tables.Inventory, Menu.GameSaveId, currentTime, Program.Serialize(Inventory.instance));



            /* Old Method
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
                              + "promptSkillPoints = " + ps.HasPreviouslyAssignedSkillPoints + Environment.NewLine
                              + "promptShopKeeper1 = " + ps.HasTalkedToShopKeeper1;

            File.WriteAllText(Directory.GetCurrentDirectory() + "/save.txt", gamesave);*/
        }

        private bool DoesSaveDataMatchCurrentData() 
        {
            Inventory savedInventory = Program.Deserialize(DatabaseHelper.instance.RetrieveData(Tables.Inventory, Menu.GameSaveId)) as Inventory;
            PlayerStats savedStats = Program.Deserialize(DatabaseHelper.instance.RetrieveData(Tables.Saves, Menu.GameSaveId)) as PlayerStats;

            if (savedInventory != null && savedStats != null)
            {
                // If the inventory saved in the database DOES NOT equal the current inventory OR if the player stats saved in the database DOES NOT equal the current player stats.
                bool inventorySame = savedInventory.Equals(Inventory.instance);
                bool statsSame = savedStats.Equals(ps);
                if (inventorySame && statsSame) 
                    return true;
            }
            return false;

            /* Old Method
            Dictionary<string, string> currentSaveValues = Menu.ParseSaveValues();

            foreach (string saveKey in currentSaveValues.Keys) 
            {
                foreach (string actualKey in ps.GameVariables.Keys) 
                {
                    if (saveKey.Equals(actualKey)) 
                    {
                        if (currentSaveValues[saveKey] != (string)ps.GameVariables[actualKey].ToString()) 
                        {
                            return false;
                        }
                    }
                }
            }*/
            //return true;
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

        private void Shop()
        {
            Console.Clear();

            if (!ps.HasTalkedToShopKeeper1)
            {
                // Intro Sequence
                Animation.Queue(new Animation(text: "*You head towards the town shop and you are greeted by the shop keeper*\n", interval: 70));
                Animation.Queue(new Animation(AnimationType.Dot, interval: 100));
                Animation.Queue(new Animation(text: "\n/w???/e: Hello there traveler! What brings you to this fine establishment?\n", interval: 70));
                Animation.Queue(new Animation(text: "\t*You tell the shop keeper that you would like to see what he offers*\n", interval: 70));
                Animation.Queue(new Animation(text: "/w???/e: Ah yes, you are free to browse as you please traveler! Speaking of which...\n", interval: 70));
                Animation.Queue(new Animation(text: "\t*He reaches out to shake your hand*\n", interval: 70));
                Animation.Queue(new Animation(text: "/wDwari/e: I haven't properly introduced myself, I'm /wDwari/e, the town's shopkeeper!\n", interval: 70));
                Animation.Queue(new Animation(text: $"\t*You tell the shop keeper your name is /w{ps.Name}/e*\n", interval: 70));
                Animation.Queue(new Animation(text: $"/wDwari/e: Ah /w{ps.Name}/e! What a fine name indeed, I can't wait to do business with you!\n", interval: 70));

                Animation.PlayQueue();

                ps.HasTalkedToShopKeeper1 = true;
                Console.ReadKey();
            }

            OpenShopKeeper();
        }
        private void OpenShopKeeper()
        {
            Console.Clear();
            string[] greetingStrings = { "Come to see my wares?", "What bring you here at this hour?", "What can I do for you friend?", "Are you in need of a new weapon perhaps?", "Come to buy some potions? They are fresh in stock!", "You've come to do business I see!", "Here to sell some items?", "Buying or selling today?" };
            string greetingString = greetingStrings[Program.IntRNG(0, greetingStrings.Length)];

            // Make sure there isn't more the 26 items.
            List<Item> itemsInStock = new List<Item>
            {
                new SmallHealthPotion(),
                new SmallManaPotion()
            };

            while (true)
            {
                Console.Clear();
                Program.ConsoleColorWriteLine($"/wDwari/e: Ahoy there /w{ps.Name}/e! {greetingString}\n");
                Program.ConsoleColorWriteLine(" * To buy items, type /wbuy/e then type the /cnumber/e next to the item.");
                Program.ConsoleColorWriteLine(" * To sell items, type /wsell/e then type the /cnumber/e next to the item.");
                Program.ConsoleColorWriteLine(" * To add a quantity, add the /ramount/e you want to purchase/sell after the first number.\n");
                Program.ConsoleColorWriteLine("(e.g. /wbuy/e /c5/e)");
                Program.ConsoleColorWriteLine("(e.g. /wbuy/e /c2/e /r5/e)");
                Program.ConsoleColorWriteLine("(e.g. /wsell/e /c8/e /r3/e)\n");
                
                Program.ConsoleColorWriteLine("To return back to town type /w\"back\"/e.\n");

                Console.WriteLine(" --------");
                Console.WriteLine(" | Buy: |");
                Console.WriteLine(" --------");

                WriteCurrentShopInventory(itemsInStock);

                Console.WriteLine(" ---------");
                Console.WriteLine(" | Sell: |");
                Console.WriteLine(" ---------");

                WritePlayerInventory();

                Program.ConsoleColorWriteLine($"Your Current /yGold/e: /w{ps.Gold}/e\n");

                Console.Write(">> ");
                string userInput = Console.ReadLine();
                userInput = userInput.ToLowerInvariant().Trim();

                if (userInput == "back")
                    break;

                if (ValidShopInput(userInput))
                {
                    bool successfulParse = ParseUserInput(userInput, out string type, out int itemNum, out uint itemAmount);

                    if (successfulParse)
                    {
                        if (type.ToLowerInvariant().Trim() == "buy")
                        {
                            if (itemNum > 0 && itemNum <= itemsInStock.Count) // If the user's input for item choice is one of the options available.
                            {
                                itemNum -= 1; // Reduce the user input by one to convert to index input.
                                Item newItem = (Item)Activator.CreateInstance(itemsInStock[itemNum].GetType());

                                newItem.Quantity = itemAmount;

                                if (ps.Gold >= newItem.BuyPrice * (itemAmount > 1 ? itemAmount : 1))
                                {
                                    ps.AlterGold(Operation.Subtract, (uint)newItem.BuyPrice * itemAmount);
                                    Inventory.instance.AddItem(newItem);

                                    Program.ConsoleColorWriteLine($"\n/wDwari/e: That will cost you /w{newItem.BuyPrice * itemAmount}/e /ygold/e /w{ps.Name}/e. Enjoy your /w{itemAmount}/e new /w{newItem.Name}{(itemAmount > 1 ? "s" : "")}!/e");
                                    Console.ReadKey();
                                }
                                else
                                {
                                    Program.ConsoleColorWriteLine($"\n/wDwari/e: Er, sorry /w{ps.Name}/e... Unfortunately you don't have enough /ygold/e to buy {(itemAmount > 1 ? "those" : "that")} item{(itemAmount > 1 ? "s" : "")}. You are /w{Math.Abs(ps.Gold - newItem.BuyPrice * itemAmount)}/e /ygold/e short.\n");
                                    Console.ReadKey();
                                }
                            }
                        }
                        else if (type.ToLowerInvariant().Trim() == "sell")
                        {
                            if (itemNum > 0 && itemNum <= Inventory.instance.GetInventory().Count)
                            {
                                itemNum -= 1;

                                Item itemToSell = (Item)Activator.CreateInstance(Inventory.instance.GetItemByIndex(itemNum).GetType());
                                itemToSell.Quantity = itemAmount;

                                bool success = Inventory.instance.RemoveItem(itemToSell);
                                ps.AlterGold(Operation.Add, (uint)itemToSell.SellPrice * itemAmount);   

                                if (success)
                                {
                                    Program.ConsoleColorWriteLine($"\nThank you for your business /w{ps.Name}/e! Here is the /w{(uint)itemToSell.SellPrice * itemAmount}/e /ygold/e I owe you.");
                                }
                                else
                                {
                                    Program.ConsoleColorWriteLine("\nError: Item could not be removed.");
                                }
                                Console.ReadKey();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("An error occurred with the input you provided.");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("\nError: That is not valid formatting. Please refer to the above text for the correct format.");
                    Console.ReadKey();
                }
            }
        }

        private void WriteCurrentShopInventory(List<Item> itemsInStock)
        {
            int itemNum = 1;
            foreach (Item item in itemsInStock)
            {
                Program.ConsoleColorWriteLine($"\n\t/w(/c{itemNum}/w)/e {item.Name}\n\t\tBuy:\t/w{item.BuyPrice}/e /ygold/e each.\n\t\tSell:\t/w{item.SellPrice}/e /ygold/e each.\n");
                itemNum += 1;
            }
        }
        private void WritePlayerInventory()
        {
            List<Item> playerInventory = Inventory.instance.GetInventory();
            if (playerInventory.Count < 1)
                Console.WriteLine("\n\t(Your pack seems to be empty...)");
            else
            {
                int counter = 1;
                foreach (Item currItem in playerInventory)
                {
                    Program.ConsoleColorWriteLine($"\n\t/w(/c{counter}/w)/e {currItem.Name} - {currItem.Description} (x /r{currItem.Quantity}/e)");
                    counter++;
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Writes out all the consumable items that is in the players inventory.
        /// </summary>
        /// <returns>The amount of consumable items in the players inventory.</returns>
        private List<Item> WriteUseItems()
        {
            List<Item> playerInventory = Inventory.instance.GetInventory();
            List<Item> consumablesInInventory = new List<Item>();

            if (playerInventory.Count < 1)
            {
                Animation.Queue(new Animation(text: "\n\t(Your pack seems to be empty...)", interval: 15));
                Animation.PlayQueue();
                Console.ReadKey();
            }
            else
            {
                foreach (Item currItem in playerInventory)
                {
                    if (currItem.Type == ItemType.Consumable)
                    {
                        consumablesInInventory.Add(currItem);
                    }
                }

                if (consumablesInInventory.Count < 1)
                {
                    Animation.RunAnimation(textToType: "\n\t(Your pack seems to be empty...)", interval: 15);
                }
                else
                {
                    int counter = 1;
                    foreach (Item currItem in playerInventory)
                    {
                        Animation.Queue(new Animation(text: $"\n\t/w(/c{counter}/w)/e {currItem.Name} (x /r{currItem.Quantity}/e)\n\t\t* {currItem.Description}", interval: 15));
                        counter++;
                    }
                }
            }

            Console.WriteLine();
            return consumablesInInventory;
        }
        private bool ValidShopInput(string str)
        {
            return new Regex(@"(buy|sell) \d+\s*\d*", RegexOptions.IgnoreCase).IsMatch(str);
        }

        /// <summary>
        /// Parses a users input and returns the trade command type, the index of item to trade, and the amount to trade.
        /// </summary>
        /// <param name="userInput">Raw, Unparsed string</param>
        /// <param name="type">Out -> Trade Command (buy/sell)</param>
        /// <param name="item">Out -> User Item Index (non-zero based)</param>
        /// <param name="amount">Out -> Amount of Item of Trade (if 0 or negative value, returns 1)</param>
        /// <returns></returns>
        private bool ParseUserInput(string userInput, out string type, out int item, out uint amount)
        {
            string[] inputs = userInput.Split(' ');

            amount = 1;
            type = inputs[0];

            if (!int.TryParse(inputs[1], out item))
                return false;

            if (inputs.Length > 2)
            {
                if (int.TryParse(inputs[2], out int intVal))
                {
                    if (intVal > 0)
                    {
                        if (!uint.TryParse(inputs[2], out amount))
                            return false;
                    }
                    else
                    {
                        amount = 1;
                    }
                }
            }

            return true;
        }
    }
}
