using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    [Serializable]
    public abstract class Item : IComparable
    {
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract uint Quantity { get; set; }
        public abstract int BuyPrice { get; set; }
        public abstract int SellPrice { get; set; }
        public abstract ItemType Type { get; set; }

        public abstract string UseAction();

        public int CompareTo(object obj)
        {
            Item item = obj as Item;

            /*
            int result = Name.CompareTo(item.Name);
            if (result != 0) return result;

            result = Quantity.CompareTo(item.Quantity);
            if (result != 0) return result;

            return 0;
            */

            return Name.CompareTo(item.Name);
        }

        public override bool Equals(object obj)
        {
            Item newItem = obj as Item;
            return (Name.Equals(newItem.Name) && Quantity.Equals(newItem.Quantity));
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    [Serializable]
    public class SmallHealthPotion : Item
    {
        private string name = "Small Health Potion";
        private string description = "Restores 3 health.";
        private uint quantity = 1;
        private int buyPrice = 50;
        private int sellPrice = 40;
        private ItemType type = ItemType.Consumable;

        public override string UseAction()
        {
            int amountOfHealthToAdd = Game.ps.MaxHealth - Game.ps.Health;
            if (amountOfHealthToAdd > 3) amountOfHealthToAdd = 3;
            Game.ps.AlterHealth(Operation.Add, amountOfHealthToAdd);
            return $"You consume a {name} and restored {amountOfHealthToAdd} health. You currently have ({Game.ps.Health}/{Game.ps.MaxHealth}) /rhealth/e.";
        }
        public override string Name
        {
            get => name;

            set => name = value;
        }
        public override string Description
        {
            get => description;

            set => description = value;
        }
        public override uint Quantity
        {
            get => quantity;

            set => quantity = value;
        }
        public override int BuyPrice
        {
            get => buyPrice;

            set => buyPrice = value;
        }
        public override int SellPrice
        {
            get => sellPrice;

            set => sellPrice = value;
        }
        public override ItemType Type
        {
            get => type;
            set => type = value;
        }
    }

    [Serializable]
    public class SmallManaPotion : Item
    {
        private string name = "Small Mana Potion";
        private string description = "Restores 3 Mana.";
        private uint quantity = 1;
        private int buyPrice = 40;
        private int sellPrice = 35;
        private ItemType type = ItemType.Consumable;

        public override string UseAction()
        {
            int amountOfHealthToAdd = Game.ps.MaxHealth - Game.ps.Health;
            if (amountOfHealthToAdd > 3) amountOfHealthToAdd = 3;
            Game.ps.AlterMana(Operation.Add, amountOfHealthToAdd);
            return $"You consume a {name} and restored {amountOfHealthToAdd} mana. You currently have ({Game.ps.Mana}/{Game.ps.MaxMana}) /cmana/e.";
        }
        public override string Name
        {
            get => name;

            set => name = value;
        }
        public override string Description
        {
            get => description;

            set => description = value; 
        }
        public override uint Quantity
        {
            get => quantity;

            set => quantity = value;
        }
        public override int BuyPrice
        {
            get => buyPrice;

            set => buyPrice = value;
        }
        public override int SellPrice
        {
            get => sellPrice;

            set => sellPrice = value;
        }
        public override ItemType Type
        {
            get => type;
            set => type = value;
        }
    }

    public enum ItemType
    {
        Consumable,
        Default
    }
}
