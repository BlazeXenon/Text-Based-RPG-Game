using System;
using System.Collections.Generic;

namespace RPG_Game {
    public class PlayerStats
    {
        //Game Variable Getters/Setters
        public string Name { get { return GameVariables["name"].ToString(); } private set { GameVariables["name"] = value; } }

        public int Health { get { return (StringToInt(GameVariables["health"].ToString(), out int x) ? x : -1); } private set { GameVariables["health"] = value; } }
        public int MaxHealth { get { return (StringToInt(GameVariables["maxHealth"].ToString(), out int x) ? x : -1); } private set { GameVariables["maxHealth"] = value; } }

        public int Mana { get { return (StringToInt(GameVariables["mana"].ToString(), out int x) ? x : -1); } private set { GameVariables["mana"] = value; } }
        public int MaxMana { get { return (StringToInt(GameVariables["maxMana"].ToString(), out int x) ? x : -1); } private set { GameVariables["maxMana"] = value; } }

        public int Level { get { return (StringToInt(GameVariables["level"].ToString(), out int x) ? x : -1); } private set { GameVariables["level"] = value; } }
        public int MAX_LEVEL { get { return max_level; } }
        public int AvailableSkillPoints { get { return (StringToInt(GameVariables["availableSkillPoints"].ToString(), out int x) ? x : -1); } private set { GameVariables["availableSkillPoints"] = value; } }
        public bool HasPreviouslyAssignedSkillPoints { get { return (StringToBool(GameVariables["promptSkillPoints"].ToString(), out bool x) ? x : false); } set { GameVariables["promptSkillPoints"] = value; } }


        public float Experience { get { return (StringToFloat(GameVariables["experience"].ToString(), out float x) ? x : -1f); } private set { GameVariables["experience"] = value; } }
        public int MaxExperience { get { return (StringToInt(GameVariables["maxExperience"].ToString(), out int x) ? x : -1); } private set { GameVariables["maxExperience"] = value; } }

        public uint Gold { get { return StringToUInt(GameVariables["availableSkillPoints"].ToString(), out uint x) ? x : 0; } private set { GameVariables["gold"] = value; } }

        public int Power { get { return (StringToInt(GameVariables["power"].ToString(), out int x) ? x : -1); } private set { GameVariables["power"] = value; } }
        public int Nimble { get { return (StringToInt(GameVariables["nimble"].ToString(), out int x) ? x : -1); } private set { GameVariables["nimble"] = value; } }
        public int Magic { get { return (StringToInt(GameVariables["magic"].ToString(), out int x) ? x : -1); } private set { GameVariables["magic"] = value; } }
        public int Cunning { get { return (StringToInt(GameVariables["cunning"].ToString(), out int x) ? x : -1); } private set { GameVariables["cunning"] = value; } }

        public PlayerClass PlayerClass {
            get {
                if (Enum.TryParse(GameVariables["class"].ToString(), out PlayerClass pClass)) {
                    return pClass;
                }
                return PlayerClass.Undefined;
            }
            private set { GameVariables["class"] = value; }
        }
        

        //Internal Variable Getters/Setters
        public Dictionary<string, object> GameVariables { get { return gameVariables; } private set { gameVariables = value; } }
        private const int max_level = 50;


        //Declare Variables
        Dictionary<string, object> gameVariables = new Dictionary<string, object>();

        public PlayerStats() {
            AddDefaultVariables();
        }

        public PlayerStats(PlayerClass pc, string playerName) {
            GameVariables.Add("name", playerName);
            GameVariables.Add("class", pc);
            GameVariables.Add("level", 1);
            if (pc == PlayerClass.Warrior) {
                GameVariables.Add("health", Program.InclusiveIntRNG(10, 13));
                GameVariables.Add("mana", Program.InclusiveIntRNG(3, 6));
                GameVariables.Add("power", Program.InclusiveIntRNG(7, 10));
                GameVariables.Add("nimble", Program.InclusiveIntRNG(1, 5));
                GameVariables.Add("magic", Program.InclusiveIntRNG(1, 5));
                GameVariables.Add("cunning", Program.InclusiveIntRNG(1, 5));
            } else if (pc == PlayerClass.Mage) {
                GameVariables.Add("health", Program.InclusiveIntRNG(5, 7));
                GameVariables.Add("mana", Program.InclusiveIntRNG(8, 13));
                GameVariables.Add("power", Program.InclusiveIntRNG(1, 5));
                GameVariables.Add("nimble", Program.InclusiveIntRNG(1, 5));
                GameVariables.Add("magic", Program.InclusiveIntRNG(9, 14));
                GameVariables.Add("cunning", Program.InclusiveIntRNG(1, 5));
            } else if (pc == PlayerClass.Archer) {
                GameVariables.Add("health", Program.InclusiveIntRNG(6, 9));
                GameVariables.Add("mana", Program.InclusiveIntRNG(3, 6));
                GameVariables.Add("power", Program.InclusiveIntRNG(2, 6));
                GameVariables.Add("nimble", Program.InclusiveIntRNG(8, 13));
                GameVariables.Add("magic", Program.InclusiveIntRNG(1, 5));
                GameVariables.Add("cunning", Program.InclusiveIntRNG(1, 5));
            }
            GameVariables.Add("maxHealth", Health);
            GameVariables.Add("maxMana", Mana);
            GameVariables.Add("availableSkillPoints", 0);
            GameVariables.Add("experience", 0.0f);
            GameVariables.Add("maxExperience", (int)(Math.Pow(2, (1.0f / 8.0f) * Level) * 10));
            GameVariables.Add("gold", 0);
            GameVariables.Add("promptSkillPoints", false);
        }

        public void AlterHealth(Operation o, int value) {
            if (o == Operation.Add) Health += value;
            else if (o == Operation.Subtract) Health -= value;
            else if (o == Operation.Set) Health = value;
        }

        public void AlterMana(Operation o, int value) {
            if (o == Operation.Add)  Mana += value;
            else if (o == Operation.Subtract)  Mana -= value;
            else if (o == Operation.Set) Mana = value;
        }

        public void AlterExperience(Operation o, float value) {
            if (o == Operation.Add) Experience += value;
            else if (o == Operation.Subtract) {
                Experience -= value;
                Experience = (float)Math.Round(Experience, 2);
            }
            else if (o == Operation.Set) Experience = value;
        }

        public void AlterMaxExperience(Operation o, int value) {
            if (o == Operation.Add) MaxExperience += value;
            else if (o == Operation.Subtract) MaxExperience -= value;
            else if (o == Operation.Set) MaxExperience = value;
        }

        public void AlterSkillPoints(Operation o, int value) {
            if (o == Operation.Add) AvailableSkillPoints += value;
            else if (o == Operation.Subtract) AvailableSkillPoints -= value;
            else if (o == Operation.Set) AvailableSkillPoints = value;
        }
        public void AlterLevel(Operation o, int value)
        {
            if (o == Operation.Add) Level += value;
            else if (o == Operation.Subtract) Level -= value;
            else if (o == Operation.Set) Level = value;
        }

        public void AlterGold(Operation o, uint value)
        {
            if (o == Operation.Add) Gold += value;
            else if (o == Operation.Subtract) Gold -= value;
            else if (o == Operation.Set) Gold = value;
        }

        public void AddStats(Stat stat, int value)
        {
            if (stat == Stat.Power) Power += value; 
            else if (stat == Stat.Nimble) Nimble += value; 
            else if (stat == Stat.Magic) Magic += value; 
            else if (stat == Stat.Cunning) Cunning += value; 
        }

        public int CurrentDifficultRating()
        {
            return (Power + Nimble + Magic) / 12;
        }

        private void AddDefaultVariables()
        {
            GameVariables.Add("name", string.Empty);
            GameVariables.Add("class", PlayerClass.Undefined);
            GameVariables.Add("health", -1);
            GameVariables.Add("maxHealth", -1);
            GameVariables.Add("mana", -1);
            GameVariables.Add("maxMana", -1);
            GameVariables.Add("level", -1);
            GameVariables.Add("availableSkillPoints", -1);
            GameVariables.Add("power", -1);
            GameVariables.Add("nimble", -1);
            GameVariables.Add("magic", -1);
            GameVariables.Add("cunning", -1);
            GameVariables.Add("experience", -1.0f);
            GameVariables.Add("maxExperience", -1);
            GameVariables.Add("gold", 0);
            GameVariables.Add("promptSkillPoints", false);
        }

        public static bool StringToInt(string str, out int x)
        {
            if (int.TryParse(str, out x))
                return true;
            return false;
        }

        public static bool StringToUInt(string str, out uint x)
        {
            if (uint.TryParse(str, out x))
                return true;
            return false;
        }

        public static bool StringToFloat(string str, out float x)
        {
            if (float.TryParse(str, out x)) 
                return true;
            return false;
        }

        public static bool StringToBool(string str, out bool x)
        {
            if (bool.TryParse(str, out x)) 
                return true;
            return false;
        }
    }

    public enum Operation {
        Add,
        Subtract,
        Set
    }

    public enum PlayerClass {
        Undefined,
        Warrior,
        Mage,
        Archer
    }

    public enum Stat { //power = warriors, nimble = archer, magic = mage, cunning = luck
        Power,
        Nimble,
        Magic,
        Cunning
    }
}
