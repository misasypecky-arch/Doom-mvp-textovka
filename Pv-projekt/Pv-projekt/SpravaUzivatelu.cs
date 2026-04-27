namespace Pv_projekt;
using System.Text.Json;
using System.IO;

public static class SpravaUzivatelu
{
    private static string souborHracu = "users.json";
    public static List<Hrac> VsichniUzivatele { get; set; } = new List<Hrac>();

    // Načte všechny uživatele ze souboru při startu serveru
    public static void NactiUzivatele()
    {
        if (!File.Exists(souborHracu)) return;
        try
        {
            string json = File.ReadAllText(souborHracu);
            VsichniUzivatele = JsonSerializer.Deserialize<List<Hrac>>(json) ?? new List<Hrac>();
        }
        catch { VsichniUzivatele = new List<Hrac>(); }
    }

    // Uloží aktuální stav všech hráčů do JSONu
    public static void Uloz()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(VsichniUzivatele, options);
        File.WriteAllText(souborHracu, json);
    }

    // Najde hráče nebo vytvoří nového (Registrace/Login)
    public static Hrac Autentizace(string jmeno, string heslo)
    {
        var existujici = VsichniUzivatele.FirstOrDefault(u => u.Username.Equals(jmeno, StringComparison.OrdinalIgnoreCase));
        
        if (existujici == null)
        {
            // Registrace nového hráče
            var novy = new Hrac(jmeno) { Password = heslo };
            VsichniUzivatele.Add(novy);
            Uloz();
            return novy;
        }

        // Kontrola hesla pro existujícího hráče
        return existujici.Password == heslo ? existujici : null;
    }
}