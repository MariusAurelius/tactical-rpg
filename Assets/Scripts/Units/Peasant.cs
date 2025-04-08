using UnityEngine;
using AgentScript;
public class Peasant : Unit
{
    Peasant()
    {
        this.maxHp = 23;
        this.currentHp = this.maxHp;
        this.atk = 3;
        this.atkSpeed = 1;
        this.atkReach = 6;

        this.movSpeed = 3; // 2.5f

        this.power = 5;
    }

     


}
