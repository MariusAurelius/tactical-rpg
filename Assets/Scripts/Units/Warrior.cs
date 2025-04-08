using UnityEngine;
using AgentScript;

public class Warrior : Unit
{
    Warrior()
    {
        this.maxHp = 45;
        this.currentHp = this.maxHp;
        this.atk = 7;
        this.atkSpeed = 0.85f;
        this.atkReach = 5;

        this.movSpeed = 3; // 2

        this.power = 10;
    }
}
