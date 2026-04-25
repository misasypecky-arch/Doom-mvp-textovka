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
        // --- PŘIDÁNO: Logování startu ---
        Logger.Zaznamenej("Server startuje...");

        try 
        {
            Console.WriteLine("[System] Načítám herní svět...");
            string json = File.ReadAllText("mistnosti.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            HerniSvet.VsechnyMistnosti = JsonSerializer.Deserialize<List<Mistnost>>(json, options);
            Console.WriteLine($"[System] Úspěšně načteno {HerniSvet.VsechnyMistnosti.Count} místností.");
        }
        catch (Exception ex)
        {
            // --- PŘIDÁNO: Logování kritické chyby ---
            Logger.Zaznamenej($"Kritická chyba při startu: {ex.Message}");
            return; 
        }

        var server = new HerniServer();
        await server.StartAsync(5000); 
    }
}