using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG_Game
{
    [Serializable]
    public class Inventory
    {
        public static Inventory instance;
        private readonly SortedSet<Item> playerItems;

        public Inventory()
        {
            instance = this;
            playerItems = new SortedSet<Item>();
        }

        public void AddItem(Item item)
        {
            if (!playerItems.Add(item))
            {
                // If the item is not added, then it already exists.
                // Update the quantity of the existing item instead.

                if (!playerItems.TryGetValue(item, out Item actualItem))
                    throw new Exception($"Item {item.Name} was not added, but found, and could not be retrieved.");

                if (actualItem != null)
                    actualItem.Quantity += item.Quantity;
            }
        }

        public bool RemoveItem(Item item)
        {
            bool retrievalSuccess = playerItems.TryGetValue(item, out Item actualItem);

            if (retrievalSuccess)
            {
                // Item was found

                // If there is only 1 of the item left, remove it from the players inventory.
                if (actualItem.Quantity == 1)
                {
                    return playerItems.Remove(item);
                }
                else
                {
                    // Otherwise, subtract that amount from the players inventory.

                    if (item.Quantity >= actualItem.Quantity)
                        return playerItems.Remove(item);
                    else
                        actualItem.Quantity -= item.Quantity;
                }
            }
            else
            {
                // Item was not found
                return false;
            }

            return true;
        }

        public List<Item> GetInventory()
        {
            return playerItems.ToList();
        }

        public Item GetItemByIndex(int index)
        {
            return playerItems.ElementAt(index);
        }

        public override bool Equals(object obj)
        {
            if (obj is Inventory otherInv)
            {
                Item[] currentPlayerItems = playerItems.ToArray();
                Item[] otherPlayerItems = otherInv.playerItems.ToArray();

                // If there lengths aren't the same then they must have different contents.
                if (currentPlayerItems.Length != otherPlayerItems.Length)
                    return false;

                // If we get here, this implies that the lengths are equal, therefore we check on a "per item" basis.
                // Note: This is a sorted set, therefore items **should be parallel with each other.

                bool difference = false;
                for (int i = 0; i < currentPlayerItems.Length; i++)
                {
                    if (!currentPlayerItems[i].Equals(otherPlayerItems[i]))
                        difference = true;
                }

                return !difference;
            }

            //return playerItems.SetEquals(otherInv.playerItems);
            return false;
        }
        public override int GetHashCode() => base.GetHashCode();
    }
}
