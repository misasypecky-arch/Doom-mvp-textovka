using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Pv_projekt_klient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "MUD Klient - Peklo";
            Console.WriteLine("=== Vítejte v klientském rozhraní pro MUD Peklo ===");
            Console.Write("Zadej IP adresu serveru (nebo stiskni Enter pro lokální server 127.0.0.1): ");
            
            string ip = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = "127.0.0.1";
            }

            using TcpClient client = new TcpClient();
            try
            {
                Console.WriteLine($"Připojuji se na {ip}:8888 ...");
                await client.ConnectAsync(ip, 8888);
                Console.WriteLine("Úspěšně připojeno k serveru!\n");

                var stream = client.GetStream();
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream) { AutoFlush = true };

                // Background Task pro asynchronní čtení zpráv ze serveru
                _ = Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {
                            string message = await reader.ReadLineAsync();
                            if (message == null) 
                            {
                                break; // Server uzavřel spojení
                            }
                            Console.WriteLine(message);
                        }
                    }
                    catch 
                    { 
                        // Ignorujeme chyby při pádu sítě v tomto bloku
                    }
                    finally 
                    { 
                        Console.WriteLine("\n[Spojení se serverem bylo ukončeno. Stiskněte Enter pro zavření klienta]");
                        Environment.Exit(0); 
                    }
                });

                // Hlavní smyčka pro čtení uživatelského vstupu z klávesnice a odesílání na server
                while (true)
                {
                    string input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        await writer.WriteLineAsync(input);
                        
                        if (input.Trim().ToLower() == "ukonci")
                        {
                            await Task.Delay(500); // Počkáme chvilku, ať server stihne poslat hlášku o uložení
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CHYBA PŘIPOJENÍ]: {ex.Message}");
                Console.ReadLine(); // Aby se okno hned nezavřelo
            }
        }
    }
}