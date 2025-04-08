using UnityEngine;
using AgentScript;

/// <summary>
/// A message that tells a unit (the recipient) to go in the general area of a destination.
/// </summary>
public class GoToAreaMessage : Message
{

    public Vector3 destination;
    public float radius;

    public GoToAreaMessage(Unit sender, Unit recipient, Vector3 destination, float radius = 5f) : base(sender, recipient)
    {
        this.destination = destination;
        this.radius = radius;
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Go To Area Message, destination: " + destination + ", radius: " + radius);
    }
}