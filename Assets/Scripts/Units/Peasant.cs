using UnityEngine;
using AgentScript;
public class Peasant : Unit
{
    Peasant()
    {
        this.maxHp = 4;
        this.currentHp = this.maxHp;
        this.atk = 1;
        this.atkSpeed = 1;
        this.atkReach = 1;

        this.movSpeed = 8;

        this.power = 3;
    }

     


}
