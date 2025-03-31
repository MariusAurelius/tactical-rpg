using UnityEngine;
using AgentScript;
public class Peasant : Unit
{
    Peasant()
    {
        this.maxHp = 7;
        this.currentHp = this.maxHp;
        this.atk = 2;
        this.atkSpeed = 1;
        this.atkReach = 1;

        this.movSpeed = 2;

        this.power = 5;
    }

     


}
