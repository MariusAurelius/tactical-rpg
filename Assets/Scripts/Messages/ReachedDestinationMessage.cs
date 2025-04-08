using UnityEngine;
using AgentScript;

/// <summary>
/// A message that tells the recipient (the leader) that a unit (the sender) has reached its destination. 
/// </summary>
public class ReachedDestinationMessage : Message
{
    public ReachedDestinationMessage(Unit sender, Unit recipient) : base(sender, recipient) { 
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Reached Destination Message");
    }
}
