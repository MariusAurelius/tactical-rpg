using UnityEngine;
using AgentScript;

public class Archer : Unit
{
    Archer() 
    {
        this.maxHp = 6;
        this.currentHp = this.maxHp;
        this.atk = 2;
        this.atkSpeed = 1;
        this.atkReach = 20;

        this.movSpeed = 6;

        this.power = 5;
    }
}
