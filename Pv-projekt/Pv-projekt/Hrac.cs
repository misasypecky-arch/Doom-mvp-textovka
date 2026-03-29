namespace Pv_projekt;

using System.Collections.Generic;
using System.Linq;

public class Hrac
{
    public string Username { get; set; }
    public int Hp { get; set; } = 100;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public int Faith { get; set; } = 50; 
    public int Money { get; set; } = 0;  
    
    public Inventory Inventory { get; set; } = new Inventory();
    public string CurrentRoomId { get; set; } = "start";
    public List<string> QuestsCompleted { get; set; } = new List<string>(); 

    public Hrac(string username)
    {
        Username = username;
    }
}