namespace Pv_projekt;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class HerniServer
{
    private TcpListener _listener;
    
    
    public ConcurrentDictionary<Guid, KlientSession> PripojeniKlienti { get; } = new();

    public async Task StartAsync(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        Console.WriteLine($"[Server] Peklo otevřelo své brány na portu {port}...");

        while (true)
        {
            
            TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("[Server] Nová duše vstoupila (nový klient připojen)!");

            
            Guid clientId = Guid.NewGuid();
            var session = new KlientSession(tcpClient, clientId, toto_odeber_klienta);
            
            PripojeniKlienti.TryAdd(clientId, session);

            
            _ = Task.Run(() => session.ZpracujKomunikaciAsync());
        }
    }

    
    private void toto_odeber_klienta(Guid clientId)
    {
        PripojeniKlienti.TryRemove(clientId, out _);
        Console.WriteLine($"[Server] Klient {clientId} se odpojil.");
    }
}