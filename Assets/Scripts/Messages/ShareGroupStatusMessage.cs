using AgentScript;
using UnityEngine;

/// <summary>
/// A message from a group leader that tells the team general the status of its group.
/// </summary>
public class ShareGroupStatusMessage : Message
{
    public int groupSize;
    public int initialGroupSize;
    public int groupHealth;
    public int groupMaxHealth;
    public Vector3 groupCenter;
    public Vector3 groupDirection;
    public ShareGroupStatusMessage(Unit sender, Unit recipient) : base(sender, recipient) { }
}
