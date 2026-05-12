namespace Pv_projekt;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class KlientSession
{
    private CommandHandler _commandHandler;
    public Hrac Hrac { get; private set; } 
    private readonly TcpClient _client;
    private readonly Guid _clientId;
    private readonly Action<Guid> _onDisconnect;
    private StreamReader _reader;
    private StreamWriter _writer;

    public KlientSession(TcpClient client, Guid clientId, Action<Guid> onDisconnect, CommandHandler commandHandler)
    {
        _client = client;
        _clientId = clientId;
        _onDisconnect = onDisconnect;
        _commandHandler = commandHandler;
        
        var stream = _client.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream) { AutoFlush = true }; 
    }

    public async Task ZpracujKomunikaciAsync()
    {
        try
        {
            
            
            while (this.Hrac == null)
            {
                await PosliZpravuAsync("Vítej v Pekle! Zadej své jméno:");
                string jmeno = (await _reader.ReadLineAsync())?.Trim();

                await PosliZpravuAsync("Zadej své heslo:");
                string heslo = (await _reader.ReadLineAsync())?.Trim();

                if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(heslo))
                {
                    await PosliZpravuAsync("Jméno i heslo musí být vyplněné. Zkus to znovu.\n");
                    continue; 
                }

                this.Hrac = SpravaUzivatelu.Autentizace(jmeno, heslo);

                if (this.Hrac == null)
                {
                    await PosliZpravuAsync("Chybné heslo pro existující účet. Zkus to znovu.\n");
                }
            }

            await PosliZpravuAsync($"\n=== Vítej zpět, {Hrac.Username}! ===\n");
            Logger.Zaznamenej($"Hráč {Hrac.Username} se přihlásil.");

            
            await Task.Delay(1500);

            
            string pomocText = _commandHandler.ProcessCommand(this.Hrac, "pomoc");
            string rozhledniText = _commandHandler.ProcessCommand(this.Hrac, "rozhledni");
            await PosliZpravuAsync($"{pomocText}\n\n{rozhledniText}");

            
            while (_client.Connected)
            {
                string prikaz = await _reader.ReadLineAsync();

                if (prikaz == null || prikaz.Trim().ToLower() == "ukonci") 
                {
                    await PosliZpravuAsync("Ukládám tvou duši, inventář i pozici... Sbohem v Pekle!");
                    break; 
                }

                Logger.Zaznamenej($"[Příkaz] {Hrac.Username}: {prikaz}");
                string odpoved = _commandHandler.ProcessCommand(this.Hrac, prikaz);
                await PosliZpravuAsync(odpoved);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Chyba] {_clientId}: {ex.Message}");
        }
        finally
        {
            SpravaUzivatelu.Uloz();
            _onDisconnect(_clientId);
            _client.Close();
        }
    }

    public async Task PosliZpravuAsync(string zprava)
    {
        try {
            await _writer.WriteLineAsync(zprava);
        } catch { }
    }
}
