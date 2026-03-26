using System.Collections.Generic;
using System.Linq;

public class CommandHandler
{
    

    public string ProcessCommand(Hrac hrac, string rawCommand)
    {
        if (string.IsNullOrWhiteSpace(rawCommand)) return "";

        var parts = rawCommand.Trim().ToLower().Split(' ');
        var action = parts[0];
        var args = parts.Skip(1).ToArray(); 

        return action switch
        {
            "jdi" => CmdJdi(hrac, args),
            "vezmi" => "Zatím neimplementováno.", 
            "inventar" => CmdInventar(hrac),
            _ => "Neznámý příkaz. Zkus napsat 'pomoc'."
        };
    }

    private string CmdJdi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Kam chceš jít? (např. 'jdi sever')";
        
        string direction = args[0];
        
        
         var currentRoom = _gameState.GetRoom(hrac.CurrentRoomId);
        
        
        
        if (currentRoom.Exits.ContainsKey(direction))
        {
            var targetRoomId = currentRoom.Exits[direction];
            var targetRoom = _gameState.GetRoom(targetRoomId);
            
            if (targetRoom.IsLocked && !hrac.Inventory.HasItem(targetRoom.RequiredKeyId))
            {
                return "Dveře jsou zamčené. Potřebuješ klíč.";
            }
            
            hrac.CurrentRoomId = targetRoomId;
            return $"Přesunul ses do: {targetRoom.Name}\n{targetRoom.Description}";
        }
        return "Tímto směrem jít nemůžeš.";
        
        
        return $"Zkusil jsi jít na {direction}. (Logika pro přesun připravena v komentáři)";
    }

    private string CmdInventar(Hrac hrac)
    {
        if (hrac.Inventory.Items.Count == 0) return "Tvůj inventář je prázdný.";
        
        var itemNames = hrac.Inventory.Items.Select(i => i.Name);
        return "Neseš: " + string.Join(", ", itemNames);
    }
}