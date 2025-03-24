using AgentScript;

/// <summary>
/// A message that tells a unit (the recipient) that the sender is currently in combat and needs help.
/// </summary>
public class NeedHelpMessage : Message
{
    // public int powerNeeded; //in this one ??
    public NeedHelpMessage(Unit sender, Unit recipient) : base(sender, recipient)
    {
    }
}
