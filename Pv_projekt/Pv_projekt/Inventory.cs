namespace Pv_projekt;

using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public List<Item> Items { get; private set; } = new List<Item>();

    public void AddItem(Item item)
    {
        Items.Add(item);
    }

    public bool RemoveItem(string itemId)
    {
        var item = Items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            Items.Remove(item);
            return true;
        }
        return false;
    }

    public bool HasItem(string itemId)
    {
        return Items.Any(i => i.ItemId == itemId);
    }
}