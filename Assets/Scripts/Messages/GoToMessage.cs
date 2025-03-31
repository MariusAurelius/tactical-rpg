using UnityEngine;
using AgentScript;

/// <summary>
/// A message that tells a unit (the recipient) to go to a specific destination.
/// </summary>
public class GoToMessage : Message
{

    public Vector3 destination;

    public GoToMessage(Unit sender, Unit recipient, Vector3 destination) : base(sender, recipient)
    {
        this.destination = destination;
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Go To Message, destination: " + destination);
    }
}
