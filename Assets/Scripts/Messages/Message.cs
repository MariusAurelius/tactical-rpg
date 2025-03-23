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
    }
}

// 0: message destine au leader / 1: message destiné aux troupes en proximité / 2: message destiné au leader et aux troupes en proximité
// 3: message du leader destiné à une troupe en particulier / 4: message d'une troupe à une autre troupe en particulier
// ( / 5: message du leader au groupe entier / 6: message du leader destiné à l'ensemble des leaders)
public enum MessageType 
{
    AskForHelp = 3, // demande de l'aide aux nearby troops
    AttackEnemy = 34,
    GoHelp = 3, // va aider une autre troupe
    GoTo = 3, // va quelque part
    NeedHelp = 2,
    Retreat = 34,
    ShareGroupStatus = 6, // toutes les x secondes, le leader envoie un message à tous les leaders pour partager l'état de son groupe
    SharePosition = 0, // toutes les x secondes, les troupes d'un groupe envoient leur position au leader qui decide s'ils sont trop éloignés les uns des autres et doivent se regrouper ou non.
    SpottedEnemy = 0,
    EngagingEnemy = 0, // nécessaire ?
    DisengagingEnemy = 0, // ou RunningAway ; nécessaire ?
}