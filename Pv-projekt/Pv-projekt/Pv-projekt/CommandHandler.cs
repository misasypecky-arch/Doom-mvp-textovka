namespace Pv_projekt;

using System;
using System.Linq;

public class CommandHandler
{
    
    private readonly Action<string> _posliVsem;
    private readonly Action<string, string> _posliDoMistnosti; 

    public CommandHandler(Action<string> posliVsem, Action<string, string> posliDoMistnosti)
    {
        _posliVsem = posliVsem;
        _posliDoMistnosti = posliDoMistnosti;
    }

    public string ProcessCommand(Hrac hrac, string rawCommand)
    {
        if (string.IsNullOrWhiteSpace(rawCommand)) return "";

        var parts = rawCommand.Trim().ToLower().Split(' ');
        var action = parts[0];
        var args = parts.Skip(1).ToArray(); 

        return action switch
        {
            
            "pomoc" => CmdPomoc(),
            "jdi" => CmdJdi(hrac, args),
            "rozhledni" => CmdRozhledni(hrac), 
            
           
            "vezmi" => CmdVezmi(hrac, args),
            "inventar" => CmdInventar(hrac),
            "pouzij" => CmdPouzij(hrac, args),
            
            
            "rekni" => CmdRekni(hrac, args),
            "krik" => CmdKrik(hrac, args),
            
            
            "utoc" => CmdUtoc(hrac, args),
            "kup" => CmdKup(hrac, args),
            
            
            "status" => CmdStatus(hrac),
            
            _ => "Neznámý příkaz. Zkus napsat 'pomoc'."
        };
    }

    private string CmdPomoc()
    {
        return "Dostupné příkazy:\r\n" +
               "- jdi [sever/jih/vychod/zapad]\r\n" +
               "- rozhledni (popíše aktuální místnost)\r\n" +
               "- vezmi [předmět]\r\n" +
               "- pouzij [předmět]\r\n" +
               "- inventar\r\n" +
               "- rekni [zpráva] (slyší jen lidé v místnosti)\r\n" +
               "- krik [zpráva] (slyší celé peklo)\r\n" +
               "- utoc [npc]\r\n" +
               "- kup [předmět]\r\n" +
               "- status (zobrazí HP, Víru a peníze)";
    }

    private string CmdJdi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Kam chceš jít? (např. 'jdi sever')";
        string direction = args[0];
    
        // Najdeme aktuální místnost hráče
        var currentRoom = NajdiMistnostPodleId(hrac.CurrentRoomId);
        if (currentRoom == null) return "Chyba: Nacházíš se v neexistující místnosti!";

        // Zkontrolujeme, zda existuje východ tímto směrem
        if (currentRoom.Exits.ContainsKey(direction))
        {
            string targetRoomId = currentRoom.Exits[direction];
            var targetRoom = NajdiMistnostPodleId(targetRoomId);

            if (targetRoom == null) return "Cesta tam vede, ale cílová místnost neexistuje!";

            // M11 – Zamčené místnosti
            if (targetRoom.IsLocked)
            {
                if (string.IsNullOrEmpty(targetRoom.RequiredKeyId) || !hrac.Inventory.HasItem(targetRoom.RequiredKeyId))
                {
                    return $"Dveře směrem na {direction} jsou zamčené. Potřebuješ klíč: {targetRoom.RequiredKeyId ?? "Neznámý"}";
                }
            }

            // Přesun hráče
            hrac.CurrentRoomId = targetRoomId;
            return $"Vydal ses na {direction}.\n\n" + CmdRozhledni(hrac); // Rovnou vypíše popis nové místnosti
        }

        return "Tímto směrem žádná cesta nevede.";
    }

    private string CmdVezmi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš vzít?";
        return $"Zvedl jsi {args[0]} a dal si ho do inventáře.";
    }

    private string CmdInventar(Hrac hrac)
    {
        if (hrac.Inventory.Items.Count == 0) return "Tvůj inventář je prázdný.";
        return "Neseš: " + string.Join(", ", hrac.Inventory.Items.Select(i => i.Name));
    }

    // CommandHandler.cs - uprav metodu CmdPouzij
    private string CmdPouzij(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš použít?";
        string itemName = args[0].ToLower();

        if (!hrac.Inventory.HasItem(itemName)) return "Tento předmět u sebe nemáš.";

        // M13 - Ruská ruleta / Ďábelský revolver
        if (itemName == "revolver") 
        {
            Random rng = new Random();
            if (rng.Next(1, 7) == 1) // Šance 1 ku 6
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

        return $"Použil jsi {itemName}, ale nic se nestalo.";
    }

    private string CmdRekni(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš říct?";
        string zprava = string.Join(" ", args);
        _posliDoMistnosti?.Invoke(hrac.CurrentRoomId, $"[{hrac.Username} říká]: {zprava}");
        return $"Řekl jsi: '{zprava}'";
    }

    private string CmdKrik(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš křičet?";
        string zprava = string.Join(" ", args);
        _posliVsem?.Invoke($"[KŘIK - {hrac.Username}]: {zprava}");
        return $"Zakřičel jsi na celé peklo: '{zprava}'";
    }
    

    private string CmdUtoc(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Na koho chceš útočit?";
        return $"Zaútočil jsi na {args[0]}! (Zde začne combat loop M2)";
    }

    private string CmdKup(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš koupit?";
        return $"Snažíš se koupit {args[0]}. (Zde se zkontrolují peníze a NPC obchodník M4)";
    }

    private string CmdStatus(Hrac hrac)
    {
        return $"--- STATUS ---\r\nJméno: {hrac.Username}\r\nHP: {hrac.Hp}\r\nÚtok: {hrac.Attack} | Obrana: {hrac.Defense}\r\nVíra (Faith): {hrac.Faith}\r\nDuše (Měna): {hrac.Money}";
    }
    private string CmdRozhledni(Hrac hrac)
    {
        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);

        if (mistnost == null) 
            return "Chyba: Nacházíš se v neexistující místnosti!";

        string info = $"--- {mistnost.Name} ---\r\n";
        info += mistnost.Description + "\r\n";

       
        if (mistnost.Exits != null && mistnost.Exits.Count > 0)
        {
            info += "Východy: " + string.Join(", ", mistnost.Exits.Keys) + "\r\n";
        }

        
        if (mistnost.ItemsOnGround != null && mistnost.ItemsOnGround.Count > 0)
        {
            info += "Na zemi vidíš: " + string.Join(", ", mistnost.ItemsOnGround) + "\r\n";
        }

        return info;
    }
    
    
    private Mistnost NajdiMistnostPodleId(string id)
    {
        
        return HerniSvet.VsechnyMistnosti.FirstOrDefault(m => m.RoomId == id);
    }

}