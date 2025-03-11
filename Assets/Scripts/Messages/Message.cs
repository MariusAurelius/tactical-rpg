using System.Collections;
using System.Collections.Generic;
using AgentScript;

public abstract class Message
{
    public Unit sender;
    public Unit recipient;

    protected Message(Unit sender, Unit recipient)
    {
        this.sender = sender;
        this.recipient = recipient;
        //this.destination = destination;
        //this.destination = recipient.transform.position;
    }
}


// 0: message destiné au leader / 1: message destiné aux troupes en proximité / 2: message destiné au leader et aux troupes en proximité
// 3: message du leader destiné à une troupe en particulier
// ( / 4: message du leader au groupe entier / 5: message du leader destiné à l'ensemble des leaders)
public enum MessageType 
{
    SharePosition = 0, // toutes les x secondes, les troupes d'un groupe envoient leur position au leader quiide s'ils sont trop éloignés les uns des autres et doivent spprocher ou non.
    SpottedEnnemy = 2,
    AttackEnnemy = 3,
    EngagingEnnemy = 0, // nécessaire ?
    DisengagingEnnemy = 0, // ou RunningAway ; nécessaire ?
    NeedHelp = 2,
    GoTo = 3, // va quelque part
    GoHelp = 3, // va aider une autre troupe
}