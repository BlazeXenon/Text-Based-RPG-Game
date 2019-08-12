using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Game
{
    public abstract class Item : IComparable
    {
        public abstract string Name { get; set; }
        public abstract int Quantity { get; set; }
        public abstract int BuyPrice { get; set; }
        public abstract int SellPrice { get; set; }

        public int CompareTo(object obj)
        {
            Item item = (Item) obj;
            return Name.CompareTo(item.Name);
        }

        public override bool Equals(object obj)
        {
            Item newItem = (Item) obj;
            return Name.Equals(newItem.Name);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public class SmallHealthPotion : Item
    {
        private string name = "Small Health Potion";
        private int quantity = 1;
        private int buyPrice = 10;
        private int sellPrice = 8;

        public override string Name
        {
            get => name;

            set => name = value;
        }
        public override int Quantity
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
    }

    public class SmallManaPotion : Item
    {
        private string name = "Small Mana Potion";
        private int quantity = 1;
        private int buyPrice = 10;
        private int sellPrice = 8;

        public override string Name
        {
            get => name;

            set => name = value;
        }
        public override int Quantity
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
    }
}
