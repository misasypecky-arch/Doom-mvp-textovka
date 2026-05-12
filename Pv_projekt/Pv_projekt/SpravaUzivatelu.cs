namespace Pv_projekt;
using System.Text.Json;
using System.IO;

public static class SpravaUzivatelu
{
    private static string souborHracu = "users.json";
    public static List<Hrac> VsichniUzivatele { get; set; } = new List<Hrac>();

    
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

    
    public static void Uloz()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(VsichniUzivatele, options);
        File.WriteAllText(souborHracu, json);
    }

    
    public static Hrac Autentizace(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var existujici = VsichniUzivatele.FirstOrDefault(u => u.Username == username);

        
        if (existujici != null)
        {
            if (existujici.Password == password)
                return existujici;

            return null; 
        }

        
        var novyHrac = new Hrac(username)
        {
            Password = password
        };

        VsichniUzivatele.Add(novyHrac);
        Uloz();

        return novyHrac;
    }
}
