namespace Pv_projekt;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class KlientSession
{
    private CommandHandler _commandHandler;
    public Hrac Hrac { get; private set; } // Změněno na property
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
            // --- FÁZE PŘIHLÁŠENÍ ---
            await PosliZpravuAsync("Vítej v Pekle! Zadej své jméno:");
            string jmeno = (await _reader.ReadLineAsync())?.Trim();

            await PosliZpravuAsync("Zadej své heslo:");
            string heslo = (await _reader.ReadLineAsync())?.Trim();

            if (string.IsNullOrWhiteSpace(jmeno) || string.IsNullOrWhiteSpace(heslo))
            {
                await PosliZpravuAsync("Jméno i heslo musí být vyplněné. Odpojování...");
                return;
            }

            this.Hrac = SpravaUzivatelu.Autentizace(jmeno, heslo);

            if (this.Hrac == null)
            {
                await PosliZpravuAsync("Chybné jméno nebo heslo. Odpojování...");
                return;
            }
            await PosliZpravuAsync($"Vítej zpět, {Hrac.Username}!");
            Logger.Zaznamenej($"Hráč {Hrac.Username} se přihlásil.");

            // --- HERNÍ SMYČKA ---
            while (_client.Connected)
            {
                string prikaz = await _reader.ReadLineAsync();
    
                // Pokud klient spadne nebo napíše "ukonci", přerušíme smyčku
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
            SpravaUzivatelu.Uloz(); // Uložíme data při odpojení
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