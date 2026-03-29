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
       
        try 
        {
            Console.WriteLine("[System] Načítám herní svět...");
            
            
            string json = File.ReadAllText("mistnosti.json");
            
            
            HerniSvet.VsechnyMistnosti = JsonSerializer.Deserialize<List<Mistnost>>(json);
            
            Console.WriteLine($"[System] Úspěšně načteno {HerniSvet.VsechnyMistnosti.Count} místností.");
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"[CHYBA] Nepodařilo se načíst mistnosti.json: {ex.Message}");
            return; 
        }

        
        var server = new HerniServer();
        await server.StartAsync(5000); 
    }
}