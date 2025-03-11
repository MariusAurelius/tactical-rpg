using UnityEngine;
using AgentScript;

public class SharePositionMessage : Message
{
    public Vector3 position;

    public SharePositionMessage(Unit sender, Unit recipient) : base(sender, recipient)
    {
        position = sender.gameObject.transform.position;
    }
}
