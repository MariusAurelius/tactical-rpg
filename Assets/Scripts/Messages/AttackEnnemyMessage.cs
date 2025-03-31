using AgentScript;
using UnityEngine;

/// <summary>
/// A message that tells a unit (the recipient) to attack an enemy.
/// </summary>
public class AttackEnemyMessage : Message
{
    // public Vector3 enemyPos; // maybe for later : go to pos then see if enemy there ?
    public Unit enemy;

    public AttackEnemyMessage(Unit sender, Unit recipient, Unit enemy /*Vector3 pos*/) : base(sender, recipient)
    {
        // enemyPos = pos;
        this.enemy = enemy; 
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Attack Enemy Message, enemy: " + enemy.debugName);
    }
}
