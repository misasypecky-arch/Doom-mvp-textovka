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
            "blackjack" => CmdBlackjack(hrac, args),
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
               "- utoc [npc] = zahájí souboj\r\n" +
               "- utoc utok | utoc blok | utoc utec | utoc item [předmět] = tah v souboji\r\n" +
               "- kup [předmět]\r\n" +
               "- blackjack - Zahraje si blackjack o duše (pouze v Kasinu)\r\n" +
               "- ukonci - Bezpečně uloží hru a odpojí tě ze serveru\r\n" +
               "- status (zobrazí HP, Víru a peníze)";
    }

    private string CmdJdi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Kam chceš jít? (např. 'jdi sever')";
        string direction = args[0];
    
        var currentRoom = NajdiMistnostPodleId(hrac.CurrentRoomId);
        if (currentRoom == null) return "Chyba: Nacházíš se v neexistující místnosti!";

        if (currentRoom.Exits.ContainsKey(direction))
        {
            string targetRoomId = currentRoom.Exits[direction];
            var targetRoom = NajdiMistnostPodleId(targetRoomId);

            if (targetRoom == null) return "Cesta tam vede, ale cílová místnost neexistuje!";

            if (targetRoom.IsLocked)
            {
                if (string.IsNullOrEmpty(targetRoom.RequiredKeyId) || !hrac.Inventory.HasItem(targetRoom.RequiredKeyId))
                {
                    return $"Dveře směrem na {direction} jsou zamčené. Potřebuješ klíč: {targetRoom.RequiredKeyId ?? "Neznámý"}";
                }
            }

            hrac.CurrentRoomId = targetRoomId;
            return $"Vydal ses na {direction}.\n\n" + CmdRozhledni(hrac); 
        }

        return "Tímto směrem žádná cesta nevede.";
    }

    private string CmdVezmi(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš vzít?";
        string itemName = string.Join("_", args).ToLower(); 

        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);
        if (mistnost == null) return "Chyba: Nacházíš se v neexistující místnosti!";

        string foundItemId = mistnost.ItemsOnGround
            .FirstOrDefault(i => i.ToLower() == itemName);

        if (foundItemId == null)
            return $"Žádný předmět '{string.Join(" ", args)}' tu nevidíš.";

        mistnost.ItemsOnGround.Remove(foundItemId);
        hrac.Inventory.AddItem(new Item(foundItemId, foundItemId.Replace("_", " "), "", "misc"));

        return $"Zvedl jsi {foundItemId.Replace("_", " ")} a dal sis ho do inventáře.";
    }

    private string CmdInventar(Hrac hrac)
    {
        if (hrac.Inventory.Items.Count == 0) return "Tvůj inventář je prázdný.";
        return "Neseš: " + string.Join(", ", hrac.Inventory.Items.Select(i => i.Name));
    }

    private string CmdPouzij(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš použít?";

        return PouzijItem(hrac, args[0]);
    }

    private string PouzijItem(Hrac hrac, string itemNameRaw)
    {
        string itemName = (itemNameRaw ?? "").Trim().ToLower();
        if (string.IsNullOrWhiteSpace(itemName)) return "Co chceš použít?";

        string resolvedItemId = itemName switch
        {
            "revolver" => "dabelsky_revolver",
            "lektvar" => "lektvar_zdravi",
            "stit" => "pekelny_stit",
            _ => itemName
        };

        if (!hrac.Inventory.HasItem(resolvedItemId))
            return "Tento předmět u sebe nemáš.";

        if (resolvedItemId == "dabelsky_revolver")
        {
            Random rng = new Random();
            if (rng.Next(1, 7) == 1)
            {
                hrac.Hp -= 50;
                hrac.Faith -= 20;
                hrac.Inventory.RemoveItem(resolvedItemId);
                return "CVAK... BUM! Revolver vystřelil. Tvé HP a Víra prudce klesly!";
            }
            else
            {
                hrac.Faith += 5;
                return "CVAK... Měl jsi štěstí. Tvá Víra v přežití roste (+5 Faith).";
            }
        }

        if (resolvedItemId == "lektvar_zdravi")
        {
            hrac.Hp = Math.Min(100, hrac.Hp + 30);
            hrac.Inventory.RemoveItem(resolvedItemId);
            return "Vypil jsi léčivý lektvar a obnovil si 30 HP.";
        }

        if (resolvedItemId == "pekelny_stit")
        {
            hrac.Defense += 3;
            hrac.Inventory.RemoveItem(resolvedItemId);
            return "Použil jsi pekelný štít. Tvá obrana se dočasně zvýšila.";
        }

        return $"Použil jsi {itemName}, ale nic se nestalo.";
    }

    private string CmdRekni(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš říct?";
        string text = string.Join(" ", args);
        _posliDoMistnosti(hrac.CurrentRoomId, $"[Místnost] {hrac.Username}: {text}");
        return $"Řekl jsi: {text}";
    }

    private string CmdKrik(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš křičet?";
        string text = string.Join(" ", args);
        _posliVsem($"[KŘIK] {hrac.Username}: {text}");
        return "Vykřikl jsi do celého světa.";
    }
    
    // --------------------------------------------------------
    // M2 - SOUBOJOVÝ SYSTÉM
    // --------------------------------------------------------
    private string CmdUtoc(Hrac hrac, string[] args)
    {
        if (!hrac.IsInCombat)
        {
            if (args.Length == 0)
                return "Na koho chceš útočit? (např. 'utoc nizsi_demon')";

            string targetId = args[0].ToLower();

            var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);
            if (mistnost == null) return "Zde nemůžeš bojovat.";

            if (!mistnost.NpcsPresent.Contains(targetId))
                return "Nikdo takový tu není.";

            Npc npc = NajdiNpcPodleId(targetId);

            hrac.IsInCombat = true;
            hrac.CombatNpcId = targetId;
            hrac.CombatNpcName = npc.Name;
            hrac.CombatNpcHp = npc.Hp;
            hrac.CombatNpcAttack = npc.Attack;
            hrac.CombatNpcDefense = npc.Defense;

            return $"Přiblížil ses ke {npc.Name}.\r\n" +
                   "Souboj začal. Zadej jednu z voleb:\r\n" +
                   "- utoc utok\r\n" +
                   "- utoc blok\r\n" +
                   "- utoc item [předmět]\r\n" +
                   "- utoc utec";
        }

        if (args.Length == 0)
        {
            return $"Jsi v souboji s {hrac.CombatNpcName} (HP nepřítele: {hrac.CombatNpcHp}).\r\n" +
                   "Volby: utoc utok | utoc blok | utoc item [předmět] | utoc utec";
        }

        string choice = args[0].ToLower();

        if (choice == "utok" || choice == "útok" || choice == "attack")
        {
            return ProvedSoubojovyTah(hrac, "utok", null);
        }

        if (choice == "blok" || choice == "block")
        {
            return ProvedSoubojovyTah(hrac, "blok", null);
        }

        if (choice == "utec" || choice == "uteč" || choice == "flee" || choice == "run")
        {
            return ProvedSoubojovyTah(hrac, "utec", null);
        }

        if (choice == "item" || choice == "pouzij" || choice == "use")
        {
            if (args.Length < 2)
                return "Jaký předmět chceš použít? (např. 'utoc item lektvar_zdravi')";

            string itemName = string.Join("_", args.Skip(1));
            return ProvedSoubojovyTah(hrac, "item", itemName);
        }

        return "Neznámá volba v souboji. Použij: utoc utok | utoc blok | utoc item [předmět] | utoc utec";
    }

    private string ProvedSoubojovyTah(Hrac hrac, string akce, string itemName)
    {
        if (!hrac.IsInCombat)
            return "Momentálně nejsi v souboji.";

        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);
        if (mistnost == null)
        {
            KonecSouboje(hrac);
            return "Souboj se ztratil v prázdnotě. Jsi zpět bez protivníka.";
        }

        if (!mistnost.NpcsPresent.Contains(hrac.CombatNpcId))
        {
            KonecSouboje(hrac);
            return "Protivník už tu není.";
        }

        bool playerBlocked = false;
        string vysledek = "";

        if (akce == "utec")
        {
            Random rng = new Random();
            if (rng.Next(1, 101) <= 60)
            {
                KonecSouboje(hrac);
                return $"Podařilo se ti uprchnout před {hrac.CombatNpcName}.";
            }

            vysledek += $"Útěk se nezdařil. {hrac.CombatNpcName} tě dohání!\r\n";
        }
        else if (akce == "item")
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return "Jaký předmět chceš použít?";

            vysledek += PouzijItem(hrac, itemName) + "\r\n";
        }
        else if (akce == "blok")
        {
            playerBlocked = true;
            vysledek += "Zvedl jsi obranu a připravil se na zásah.\r\n";
        }
        else if (akce == "utok")
        {
            int dmgToNpc = Math.Max(1, hrac.Attack - hrac.CombatNpcDefense);
            hrac.CombatNpcHp -= dmgToNpc;
            vysledek += $"Zaútočil jsi na {hrac.CombatNpcName} a způsobil jsi {dmgToNpc} poškození. (Zbývá mu {Math.Max(0, hrac.CombatNpcHp)} HP)\r\n";

            if (hrac.CombatNpcHp <= 0)
            {
                vysledek += $"** {hrac.CombatNpcName} padl mrtev k zemi! Získal jsi 25 Duší. **";
                hrac.Money += 25;
                mistnost.NpcsPresent.Remove(hrac.CombatNpcId);
                KonecSouboje(hrac);
                return vysledek;
            }
        }

        int dmgToHrac = Math.Max(1, hrac.CombatNpcAttack - hrac.Defense);
        if (playerBlocked)
            dmgToHrac = Math.Max(1, (int)Math.Ceiling(dmgToHrac * 0.5));

        hrac.Hp -= dmgToHrac;
        vysledek += $"{hrac.CombatNpcName} tě zasáhl za {dmgToHrac} poškození. (Zbývá ti {Math.Max(0, hrac.Hp)} HP)";

        if (hrac.Hp <= 0)
        {
            vysledek += "\r\n--- ZEMŘEL JSI! ---\r\nTvá duše se vrací na začátek pekla...";
            hrac.Hp = 100;
            hrac.Money /= 2;
            hrac.CurrentRoomId = "start";
            KonecSouboje(hrac);
            return vysledek;
        }

        return vysledek + "\r\n" +
               "Další tah: utoc utok | utoc blok | utoc item [předmět] | utoc utec";
    }

    private void KonecSouboje(Hrac hrac)
    {
        hrac.IsInCombat = false;
        hrac.CombatNpcId = null;
        hrac.CombatNpcName = null;
        hrac.CombatNpcHp = 0;
        hrac.CombatNpcAttack = 0;
        hrac.CombatNpcDefense = 0;
    }

    // --------------------------------------------------------
    // M4 - OBCHODOVÁNÍ
    // --------------------------------------------------------
    private string CmdKup(Hrac hrac, string[] args)
    {
        if (args.Length == 0) return "Co chceš koupit? (Zkus 'kup lektvar' nebo 'kup stit')";
        string itemName = args[0].ToLower();

        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);
        
        // Zkontrolujeme, jestli je v místnosti obchodník
        if (!mistnost.NpcsPresent.Contains("obchodnik_dusi"))
            return "Není tu nikdo, kdo by s tebou chtěl obchodovat.";

        int cena = 0;
        string itemId = "";

        // Jednoduchý ceník napevno (Zde si můžete přidat další věci)
        switch (itemName)
        {
            case "lektvar": cena = 20; itemId = "lektvar_zdravi"; break;
            case "stit": cena = 50; itemId = "pekelny_stit"; break;
            default: return "Obchodník vrčí: 'Tohle nevedu. Mám jen lektvar (20 duší) a stit (50 duší).'";
        }

        if (hrac.Money < cena)
            return $"Obchodník se chraplavě zasmál: 'Nemáš dost duší, ubožáku! Stojí to {cena}.'";

        // Provedení platby a přidání předmětu
        hrac.Money -= cena;
        hrac.Inventory.AddItem(new Item(itemId, itemId.Replace("_", " "), "Koupený předmět", "vybaveni", cena));
        
        return $"Úspěšně jsi koupil {itemName} za {cena} duší. Zbývá ti {hrac.Money} duší.";
    }

    private string CmdStatus(Hrac hrac)
    {
        return $"--- STATUS ---\r\nJméno: {hrac.Username}\r\nHP: {hrac.Hp}\r\nÚtok: {hrac.Attack} | Obrana: {hrac.Defense}\r\nVíra (Faith): {hrac.Faith}\r\nDuše (Měna): {hrac.Money}";
    }

    private string CmdRozhledni(Hrac hrac)
    {
        var mistnost = NajdiMistnostPodleId(hrac.CurrentRoomId);
        if (mistnost == null) return "Chyba: Nacházíš se v prázdnotě!";

        string info = $"--- {mistnost.Name} ---\r\n";
        info += mistnost.Description + "\r\n";

        var ostatniHraci = HerniServer.PripojeniKlienti.Values
            .Where(s => s.Hrac != null && s.Hrac.CurrentRoomId == hrac.CurrentRoomId && s.Hrac != hrac)
            .Select(s => s.Hrac.Username)
            .ToList();

        if (ostatniHraci.Any())
            info += "Vidíš zde další duše: " + string.Join(", ", ostatniHraci) + "\n";

        if (mistnost.ItemsOnGround != null && mistnost.ItemsOnGround.Any())
        {
            var itemy = mistnost.ItemsOnGround.Select(i => i.Replace("_", " "));
            info += "Na zemi leží: " + string.Join(", ", itemy) + "\n";
        }

        if (mistnost.NpcsPresent != null && mistnost.NpcsPresent.Any())
        {
            var npcText = mistnost.NpcsPresent
                .Select(id => NajdiNpcPodleId(id))
                .Where(npc => npc != null)
                .Select(npc => npc.IsTrader
                    ? $"{npc.Name} (obchodník)"
                    : $"{npc.Name} {(npc.IsHostile ? "(nepřátelský)" : "(neutrální)")}");

            if (npcText.Any())
                info += "Přítomné bytosti: " + string.Join(", ", npcText) + "\n";
        }

        return info;
    }
    // A samotná metoda:
    private string CmdBlackjack(Hrac hrac, string[] args)
    {
        if (!hrac.CurrentRoomId.Contains("kasino"))
            return "Tady není žádný stůl na Blackjack. Jdi do kasina!";

        if (hrac.Money < 10)
            return "Nemáš dost duší (min. 10) na sázku!";

        Random rng = new Random();
    
        // Jednoduchá simulace: Hráč vs Dealer
        int hracKarty = rng.Next(15, 26); // Simulujeme součet karet
        int dealerKarty = rng.Next(17, 24);

        string vysledek = $"--- BLACKJACK ---\nTvůj součet: {hracKarty}\nSoučet dealera: {dealerKarty}\n";

        if (hracKarty > 21) {
            hrac.Money -= 10;
            return vysledek + "Přetáhl jsi! Prohráváš 10 duší.";
        }
        if (dealerKarty > 21 || hracKarty > dealerKarty) {
            hrac.Money += 10;
            return vysledek + "Vyhrál jsi! Získáváš 10 duší.";
        }
        if (hracKarty == dealerKarty) {
            return vysledek + "Remíza. Duše ti zůstávají.";
        }

        hrac.Money -= 10;
        return vysledek + "Dealer vyhrál. Prohráváš 10 duší.";
    }
    
    private Mistnost NajdiMistnostPodleId(string id)
    {
        return HerniSvet.VsechnyMistnosti.FirstOrDefault(m => m.RoomId == id);
    }

    // Pomocná metoda pro simulaci databáze NPC (M2/M4)
    private Npc NajdiNpcPodleId(string id)
    {
        // Tyto staty se obvykle načítají ze samostatného souboru npc.json, 
        // ale pro MVP tu máme hardcoded hodnoty podle ID z mistnosti.json
        if (id == "obchodnik_dusi") return new Npc(id, "Obchodník Duší", false, true) { Hp = 100, Attack = 5, Defense = 5 };
        if (id == "nizsi_demon") return new Npc(id, "Nižší Démon", true, false) { Hp = 30, Attack = 12, Defense = 2 };
        if (id == "krupier_satan") return new Npc(id, "Krupiér Satan", true, false) { Hp = 150, Attack = 25, Defense = 8 };
        
        // Defaultní fallback, pokud ID neznáme
        return new Npc(id, "Neznámá bytost", true, false) { Hp = 20, Attack = 5, Defense = 1 };
    }
}