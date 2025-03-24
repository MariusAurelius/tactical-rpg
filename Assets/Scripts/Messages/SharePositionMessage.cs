using UnityEngine;
using AgentScript;

/// <summary>
/// A message that shares the sender's position with the recipient (the leader). 
/// </summary>
public class SharePositionMessage : Message
{
    public Vector3 position;

    public SharePositionMessage(Unit sender, Unit recipient) : base(sender, recipient)
    {
        position = sender.gameObject.transform.position;
    }
}
