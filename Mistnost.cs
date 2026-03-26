using System.Collections.Generic;
using System.Linq;

public class Mistnost
{
    public string RoomId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public Dictionary<string, string> Exits { get; set; } = new Dictionary<string, string>();
    
    public List<string> ItemsOnGround { get; set; } = new List<string>(); 
    public List<string> NpcsPresent { get; set; } = new List<string>();   
    
    public bool IsLocked { get; set; } = false;
    public string RequiredKeyId { get; set; } = null;

    
    public Mistnost() { }

    public void AddExit(string direction, string targetRoomId)
    {
        Exits[direction] = targetRoomId;
    }
}