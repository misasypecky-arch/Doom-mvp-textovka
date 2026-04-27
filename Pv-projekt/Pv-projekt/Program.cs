namespace Pv_projekt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json; 
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Logger.Zaznamenej("=== Server startuje ===");

        try 
        {
            // Načtení světa
            string json = File.ReadAllText("mistnosti.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            HerniSvet.VsechnyMistnosti = JsonSerializer.Deserialize<List<Mistnost>>(json, options);
            Logger.Zaznamenej($"[System] Načteno {HerniSvet.VsechnyMistnosti.Count} místností.");

            // Načtení uživatelů
            SpravaUzivatelu.NactiUzivatele();
        }
        catch (Exception ex)
        {
            Logger.Zaznamenej($"[KRITICKÁ CHYBA] {ex.Message}");
            return; 
        }

        var server = new HerniServer();
        await server.StartAsync(5000); 
    }
}