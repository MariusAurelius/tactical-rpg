using AgentScript;
using UnityEngine;

/// <summary>
/// A message to tell a unit (the recipient) to go help another unit (the friend).
/// </summary>
public class GoHelpMessage : Message
{
    /// <summary>
    /// The friend to go help.
    /// </summary>
    public Unit friend;

    public GoHelpMessage(Unit sender, Unit recipient, Unit friend) : base(sender, recipient)
    {
        this.friend = friend;
        Debug.Log("Message sent from " + sender.debugName + " to " + recipient.debugName + ": Go Help Message, friend to help: " + friend.debugName);
    }
}
