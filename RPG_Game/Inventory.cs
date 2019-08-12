using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG_Game
{
    public class Inventory
    {
        public static Inventory instance;
        private SortedSet<Item> playerItems;

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
    }
}
