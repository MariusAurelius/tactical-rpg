using UnityEngine;
using AgentScript;

/// <summary>
/// A message that tells the recipient (the leader) that the sender has spotted an enemy.
/// </summary>
public class SpottedEnemyMessage : Message
{
    public Unit enemy;

    public SpottedEnemyMessage(Unit sender, Unit recipient, Unit enemy) : base(sender, recipient)
    {
        this.enemy = enemy;
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Spotted Enemy Message, enemy: " + enemy.debugName);
    }
}
