using AgentScript;

/// <summary>
/// A message from the leader that tells a unit (the recipient) to ask for help from nearby friendlies with attacking an enemy.
/// </summary>
public class AskForHelp : Message
{
    /// <summary>
    /// The enemy that the recipient needs help with attacking.
    /// </summary>
    public Unit enemy;
    public AskForHelp(Unit sender, Unit recipient, Unit enemy) : base(sender, recipient)
    {
        this.enemy = enemy;
    }
    
}
