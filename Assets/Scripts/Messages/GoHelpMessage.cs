using UnityEngine;
using AgentScript;

public class GoHelpMessage : Message
{
    /// <summary>
    /// The friend to go help.
    /// </summary>
    public Unit friend;

    public GoHelpMessage(Unit sender, Unit recipient, Unit friend) : base(sender, recipient)
    {
        this.friend = friend;
    }
}
