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

    // Přihlášení existujícího hráče (už nevytváří účet automaticky)
    public static Hrac Autentizace(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var existujici = VsichniUzivatele.FirstOrDefault(u => u.Username == username);

        // Když účet existuje, zkontroluj heslo
        if (existujici != null)
        {
            if (existujici.Password == password)
                return existujici;

            return null; // špatné heslo
        }

        // Když účet neexistuje, vytvoř nový
        var novyHrac = new Hrac(username)
        {
            Password = password
        };

        VsichniUzivatele.Add(novyHrac);
        Uloz();

        return novyHrac;
    }
}