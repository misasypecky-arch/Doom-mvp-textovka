namespace Pv_projekt;

using System;
using System.IO;

public static class Logger
{
    private static string logFilePath = "server_log.txt";
    private static readonly object _zamek = new object();

    public static void Zaznamenej(string zprava)
    {
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
                Console.WriteLine($"[CHYBA LOGOVÁNÍ] {ex.Message}");
            }
        }
    }
}