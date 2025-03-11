using UnityEngine;
using AgentScript;

public class GoToMessage : Message
{

    public Vector3 destination;

    public GoToMessage(Unit sender, Unit recipient, Vector3 destination) : base(sender, recipient)
    {
        this.destination = destination;
    }
}
