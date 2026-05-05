namespace Pv_projekt;

public class Item
{
    public string ItemId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ItemType { get; set; } 
    public int Value { get; set; }
    
    // PŘIDÁNO: Proměnná pro ID efektu
    public string EffectId { get; set; }

    public Item(string itemId, string name, string description, string itemType, int value = 0, string effectId = "nic")
    {
        ItemId = itemId;
        Name = name;
        Description = description;
        ItemType = itemType;
        Value = value;
        EffectId = effectId; // Např. "heal_30", "def_3", "revolver_rng"
    }

    // PŘIDÁNO: Funkce, která vykoná efekt přímo podle EffectId
    public string VykonejEfekt(Hrac hrac)
    {
        if (EffectId == "nic") 
            return $"Použil jsi {Name}, ale vůbec nic se nestalo.";

        if (EffectId == "heal_30")
        {
            hrac.Hp = Math.Min(100, hrac.Hp + 30);
            return $"Vypil jsi {Name} a obnovil si 30 HP.";
        }
        
        if (EffectId == "def_3")
        {
            hrac.Defense += 3;
            return $"Použil jsi {Name}. Tvá obrana se dočasně zvýšila (+3).";
        }

        if (EffectId == "revolver_rng")
        {
            Random rng = new Random();
            if (rng.Next(1, 7) == 1) // Ruská ruleta
            {
                hrac.Hp -= 50;
                hrac.Faith -= 20;
                return "CVAK... BUM! Revolver vystřelil. Tvé HP a Víra prudce klesly!";
            }
            else
            {
                hrac.Faith += 5;
                return "CVAK... Měl jsi štěstí. Tvá Víra v přežití roste (+5 Faith).";
            }
        }

        return $"Neznámý efekt předmětu {Name}.";
    }
}