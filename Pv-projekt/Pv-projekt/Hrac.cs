namespace Pv_projekt;

using System.Collections.Generic;
using System.Linq;

public class Hrac
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int Hp { get; set; } = 100;
    public int Attack { get; set; } = 10;
    public int Defense { get; set; } = 5;
    public int Faith { get; set; } = 50;
    public int Money { get; set; } = 0;

    public Inventory Inventory { get; set; } = new Inventory();
    public string CurrentRoomId { get; set; } = "start";
    public List<string> QuestsCompleted { get; set; } = new List<string>();

    public bool IsInCombat { get; set; } = false;
    public string CombatNpcId { get; set; } = null;
    public string CombatNpcName { get; set; } = null;
    public int CombatNpcHp { get; set; } = 0;
    public int CombatNpcAttack { get; set; } = 0;
    public int CombatNpcDefense { get; set; } = 0;

    public Hrac(string username)
    {
        Username = username;
    }
}