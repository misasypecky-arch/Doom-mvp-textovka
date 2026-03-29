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

    public KlientSession(TcpClient client, Guid clientId, Action<Guid> onDisconnect)
    {
        _client = client;
        _clientId = clientId;
        _onDisconnect = onDisconnect;
        
        var stream = _client.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream) { AutoFlush = true }; 
    }

    public async Task ZpracujKomunikaciAsync()
    {
        await PosliZpravuAsync("Vítej v Pekle! Zadej svůj příkaz (např. 'jdi sever'):");

        try
        {
            while (_client.Connected)
            {
                
                string zpravaOdHrace = await _reader.ReadLineAsync();
                
                
                if (zpravaOdHrace == null) break;

                Console.WriteLine($"[Klient {_clientId}] poslal: {zpravaOdHrace}");

                
                 string odpoved = commandHandler.ProcessCommand(hrac, zpravaOdHrace);
                 await PosliZpravuAsync(odpoved);

                
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
            await _writer.WriteLineAsync(zprava);
        }
        catch
        {
            
            UkoniSpojeni();
        }
    }

    private void UkoniSpojeni()
    {
        _client.Close();
        _onDisconnect?.Invoke(_clientId);
    }
}