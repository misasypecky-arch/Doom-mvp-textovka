namespace Pv_projekt;

using System.Collections.Generic;
using System.Linq;

public class Npc
{
    public string NpcId { get; set; }
    public string Name { get; set; }
    public bool IsHostile { get; set; }
    public bool IsTrader { get; set; }
    
    
    public int Hp { get; set; } = 50;
    public int Attack { get; set; } = 8;
    public int Defense { get; set; } = 2;
    
    public List<string> LootTable { get; set; } = new List<string>(); 

    public Npc(string npcId, string name, bool isHostile, bool isTrader)
    {
        NpcId = npcId;
        Name = name;
        IsHostile = isHostile;
        IsTrader = isTrader;
    }
}