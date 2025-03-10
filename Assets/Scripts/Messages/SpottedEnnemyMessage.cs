using UnityEngine;
using AgentScript;

public class SpottedEnnemyMessage : Message
{
    public Vector3 ennemyPos;
    // public Unit ennemy;

    public SpottedEnnemyMessage(Unit sender, Unit recipient, Vector3 pos /*, Unit ennemy*/) : base(sender, recipient)
    {
        ennemyPos = pos;
        //this.ennemy = ennemy;
    }
}
