using UnityEngine;
using AgentScript;

public class Archer : Unit
{
    Archer() 
    {
        this.maxHp = 21;
        this.currentHp = this.maxHp;
        this.atk = 4;
        this.atkSpeed = 1.15f;
        this.atkReach = 15;

        this.movSpeed = 3;

        this.power = 7;
    }
}
