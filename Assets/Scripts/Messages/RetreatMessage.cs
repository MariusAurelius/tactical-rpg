using AgentScript;

/// <summary>
/// A message that tells a unit (the recipient) to retreat back to the leader.
/// </summary>
public class RetreatMessage : Message
{
    public RetreatMessage(Unit sender, Unit recipient) : base(sender, recipient)
    {
    }
    
}
