using UnityEngine;
using AgentScript;

public class AttackEnnemyMessage : Message
{
    public Vector3 ennemyPos;
    // public Unit ennemy;

    public AttackEnnemyMessage(Unit sender, Unit recipient, Vector3 pos /*, Unit ennemy*/) : base(sender, recipient)
    {
        ennemyPos = pos;
        //this.ennemy = ennemy; 
    }
}
