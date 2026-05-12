namespace Pv_projekt;
using System;
using System.Collections.Generic;


using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class HerniServer
{
    private TcpListener _listener;
    
    
    
    public static ConcurrentDictionary<Guid, KlientSession> PripojeniKlienti { get; } = new();

    

    public async Task StartAsync(int port)
    {
        
        
        var commandHandler = new CommandHandler(PosliVsem, PosliDoMistnosti);

        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();

        while (true)
        {
            TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
            Guid clientId = Guid.NewGuid();
        
            
            var session = new KlientSession(tcpClient, clientId, toto_odeber_klienta, commandHandler);
            PripojeniKlienti.TryAdd(clientId, session);
            _ = Task.Run(() => session.ZpracujKomunikaciAsync());
        }
    }

    
    public void PosliVsem(string zprava)
    {
        foreach (var klient in PripojeniKlienti.Values)
        {
            _ = klient.PosliZpravuAsync($"[GLOBÁLNÍ] {zprava}");
        }
    }

    public void PosliDoMistnosti(string roomId, string zprava)
    {
        foreach (var klient in PripojeniKlienti.Values)
        {
            if (klient.Hrac?.CurrentRoomId == roomId)
            {
                _ = klient.PosliZpravuAsync(zprava);
            }
        }
    }

    private void toto_odeber_klienta(Guid clientId)
    {
        PripojeniKlienti.TryRemove(clientId, out _);
    
        
        Logger.Zaznamenej($"Hráč s ID {clientId} se odpojil.");
    }

    
    
}
