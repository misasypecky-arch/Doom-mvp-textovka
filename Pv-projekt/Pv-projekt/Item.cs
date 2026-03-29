namespace Pv_projekt;

using System.Collections.Generic;
using System.Linq;

public class Item
{
    public string ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ItemType { get; set; } 
    public int Value { get; set; }

    public Item(string itemId, string name, string description, string itemType, int value = 0)
    {
        ItemId = itemId;
        Name = name;
        Description = description;
        ItemType = itemType;
        Value = value;
    }
}

