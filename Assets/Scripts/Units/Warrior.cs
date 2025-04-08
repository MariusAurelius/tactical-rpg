using UnityEngine;
using AgentScript;

public class Warrior : Unit
{
    Warrior()
    {
        this.maxHp = 10;
        this.currentHp = this.maxHp;
        this.atk = 3;
        this.atkSpeed = 1;
        this.atkReach = 2;

        this.movSpeed = 5;

        this.power = 7;
    }
}
