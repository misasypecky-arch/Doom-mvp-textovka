namespace Pv_projekt;

using System;
using System.IO;

public static class Logger
{
    private static string logFilePath = "server_log.txt";
    // Tady je ten klíč – zámek
    private static readonly object _zamek = new object();

    public static void Zaznamenej(string zprava)
    {
        // lock zajistí, že pokud jedno vlákno zapisuje, 
        // ostatní počkají ve frontě, dokud nedopíše.
        lock (_zamek)
        {
            try
            {
                string casovaZnacka = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string formatovanyZaznam = $"[{casovaZnacka}] {zprava}";

                Console.WriteLine(formatovanyZaznam);
                File.AppendAllText(logFilePath, formatovanyZaznam + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KRITICKÁ CHYBA LOGU] {ex.Message}");
            }
        }
    }
}