using UnityEngine;

using AgentScript;

public class Archer : Unit
{
    Archer()
    {
        this.maxHp = 7;
        this.currentHp = this.maxHp;
        this.atk = 2;
        this.atkSpeed = 1;
        this.atkReach = 5;

        this.movSpeed = 2;

        this.power = 5;
    }
}
