namespace Pv_projekt;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class KlientSession
{
    private CommandHandler commandHandler;
    private Hrac hrac;
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
        this.commandHandler = commandHandler; // Toto tam musí být!
    
        var stream = _client.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream) { AutoFlush = true }; 
    }

    public Hrac Hrac;

    public async Task ZpracujKomunikaciAsync()
    {
        // PRO TESTOVÁNÍ: Vytvoříme dočasného hráče
        this.hrac = new Hrac("Poutnik"); 
    
        await PosliZpravuAsync("Vítej v Pekle! Zadej svůj příkaz:");

        try
        {
            while (_client.Connected)
            {
                string zpravaOdHrace = await _reader.ReadLineAsync();
                if (zpravaOdHrace == null) break;

                // --- PŘIDÁNO: Logování příkazu ---
                // Použijeme Username, pokud už je hráč vytvořen, jinak ID
                string jmeno = hrac != null ? hrac.Username : _clientId.ToString();
                Logger.Zaznamenej($"Hráč {jmeno} zadal příkaz: {zpravaOdHrace}");

                // Zpracování příkazu (pokud máš commandHandler inicializovaný)
                if (commandHandler != null && hrac != null)
                {
                    string odpoved = commandHandler.ProcessCommand(hrac, zpravaOdHrace);
                    await PosliZpravuAsync(odpoved);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Chyba] {_clientId}: {ex.Message}");
        }
        finally
        {
            UkoniSpojeni();
        }
    }

    public async Task PosliZpravuAsync(string zprava)
    {
        try
        {
            // Přidáme \r\n na konec každé zprávy, kterou posíláme
            await _writer.WriteAsync(zprava + "\r\n");
            await _writer.FlushAsync(); // Okamžitě odešle data ze bufferu
        }
        catch (Exception ex)
        {
            Logger.Zaznamenej($"Chyba při posílání zprávy: {ex.Message}");
        }
    }

    private void UkoniSpojeni()
    {
        _client.Close();
        _onDisconnect?.Invoke(_clientId);
    }
}