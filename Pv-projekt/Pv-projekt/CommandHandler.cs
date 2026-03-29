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
        return "Dostupné příkazy:\n" +
               "- jdi [sever/jih/vychod/zapad]\n" +
               "- rozhledni (popíše aktuální místnost)\n" +
               "- vezmi [předmět]\n" +
               "- pouzij [předmět]\n" +
               "- inventar\n" +
               "- rekni [zpráva] (slyší jen lidé v místnosti)\n" +
               "- krik [zpráva] (slyší celé peklo)\n" +
               "- utoc [npc]\n" +
               "- kup [předmět]\n" +
               "- status (zobrazí HP, Víru a peníze)";
    }

    private string CmdJdi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Kam chceš jít? (např. 'jdi sever')";
        string direction = args[0];
        
       
        return $"Vydal ses na {direction}... (Logika pro načtení místnosti a kontrolu zamčených dveří M11)";
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

    private string CmdPouzij(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš použít?";
        
        return $"Použil jsi {args[0]}. (Zde se aplikuje efekt na hráče)";
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
        return $"--- STATUS ---\nJméno: {hrac.Username}\nHP: {hrac.Hp}\nÚtok: {hrac.Attack} | Obrana: {hrac.Defense}\nVíra (Faith): {hrac.Faith}\nDuše (Měna): {hrac.Money}";
    }
    private string CmdRozhledni(Hrac hrac)
    {
        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);

        if (mistnost == null) 
            return "Chyba: Nacházíš se v neexistující místnosti!";

        string info = $"--- {mistnost.Name} ---\n";
        info += mistnost.Description + "\n";

       
        if (mistnost.Exits != null && mistnost.Exits.Count > 0)
        {
            info += "Východy: " + string.Join(", ", mistnost.Exits.Keys) + "\n";
        }

        
        if (mistnost.ItemsOnGround != null && mistnost.ItemsOnGround.Count > 0)
        {
            info += "Na zemi vidíš: " + string.Join(", ", mistnost.ItemsOnGround) + "\n";
        }

        return info;
    }
    private Mistnost NajdiMistnostPodleId(string id)
    {
        
        return HerniSvet.VsechnyMistnosti.FirstOrDefault(m => m.RoomId == id);
    }

}