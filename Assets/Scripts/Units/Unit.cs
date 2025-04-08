using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AgentScript
{
    public enum Team : int
    {
        RED = 1,
        BLUE = 2
    }

    public class Unit : MonoBehaviour
    {
        public enum BEHAVIOURS
        {
            WANDERING,
            GOING,
            ATTACKING
        }

        /// <summary>
        /// The unique identifier of this unit.
        /// </summary>
        public int id;

        /// <summary>
        /// Is this unit a sub-team leader?
        /// </summary>
        public bool isLeader;

        /// <summary>
        /// The overall team this unit belongs to. (e.g. RED or BLUE)
        /// </summary>
        public Team team;

        /// <summary>
        /// The starting health points of this unit.
        /// </summary>
        protected int maxHp = 0;

        /// <summary>
        /// The current health points of this unit.
        /// </summary>
        protected int currentHp = 0;

        /// <summary>
        /// The amount of damage this unit inflicts on an enemy.
        /// </summary>
        protected int atk;

        /// <summary>
        /// How fast this unit's attacks are, directly influencing the attack cooldown.
        /// </summary>
        /// <remarks>
        /// The higher the value, the faster the unit attacks.
        /// </remarks>
        protected float atkSpeed;

        /// <summary>
        /// The reach of this unit's attacks, or from what distance it can attack an enemy.
        /// </summary>
        protected int atkReach;

        /// <summary>
        /// The cooldown between attacks, in seconds.
        /// </summary>
        protected float attackCooldown = 0f;

        /// <summary>
        /// The speed at which this unit moves.
        /// </summary>
        protected float movSpeed;

        /// <summary>
        /// The power of this unit, used to determine if it can attack an enemy or not.
        /// </summary>
        protected int power;

        /// <summary>
        /// The power of this unit as perceived by the enemy.
        /// </summary>
        public int perceivedPower = -1; // unknown to enemy

        /// <summary>
        /// Has this unit been attacked by an enemy?
        /// </summary>
        /// <remarks>
        /// If true, then this unit's hp is known to the enemy.
        /// </remarks>
        public bool hasBeenAttacked;

        public Unit leader;

        //Agent
        private NavMeshAgent agent;
        public BEHAVIOURS currentBehaviour = BEHAVIOURS.WANDERING;
        public Unit currentEnemy;

        [Header("Animations")]
        [SerializeField]
        protected Animator _animator;

        public Queue<Message> ReceivedMessages;

        /// <summary>
        /// The unit's color, type, id, and subteam id for logging and debugging.
        /// </summary>
        public string debugName;

        void Start()
        {

            agent = this.GetComponent<NavMeshAgent>();
            Search();
            ReceivedMessages = new Queue<Message>();
        }

        void Update()
        {
            if (isLeader && agent.destination == null)
            {
                Debug.Log("hiyaaaaaaa");
                SetRandomDestination();
            }
            if (this.currentEnemy == null /*&& leader didn't tell us where to go: agent.destination or behaviour wandering*/)
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }
            switch (currentBehaviour)
            {
                case BEHAVIOURS.WANDERING:
                    if (agent.remainingDistance == 0)
                    {
                        agent.speed = movSpeed;
                        Search();
                    }
                    SeeEnemy();
                    break;
                case BEHAVIOURS.GOING:
                    Go(); break;
                case BEHAVIOURS.ATTACKING:
                    Attack(); break;
                default: throw new Exception("Unknown behaviour");
            }
        }

        void Attack()
        {
            if (currentEnemy != null)
            {
                Vector3 directionToEnemy = (currentEnemy.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToEnemy.x, 0, directionToEnemy.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

                if (attackCooldown <= 0f)
                {
                    currentEnemy.ReduceHp(this.atk);
                    attackCooldown = 1f / atkSpeed;  // Cooldown based on attack speed

                    currentEnemy.hasBeenAttacked = true;
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }
            }
            else
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }
        }

        public void SetRandomDestination()
        {
            SetRandomDestination(-50f, 50f, -50f, 50f);
        }

        void SetRandomDestination(float minX, float maxX, float minZ, float maxZ)
        {
            Debug.Log(debugName + ": SetRandomDestination() called");
            if (agent == null)
            {
                Debug.LogError($"{debugName}: NavMeshAgent is null. Cannot set random destination.");
                return;
            }
            if (!agent.isActiveAndEnabled)
            {
                Debug.LogError($"{debugName}: NavMeshAgent is not active or enabled.");
                return;
            }
            
            Vector3 moveto;
            int maxAttempts = 10; // Limit the number of attempts to find a valid destination
            int attempts = 0;

            do
            {
            float rx = UnityEngine.Random.Range(minX, maxX);
            float rz = UnityEngine.Random.Range(minZ, maxZ);
            moveto = new Vector3(rx, transform.position.y, rz);

            NavMeshPath path = new();
            agent.CalculatePath(moveto, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(moveto);
                Debug.Log(debugName + ": Valid destination set to " + moveto);
                return;
            }

            attempts++;
            } while (attempts < maxAttempts);

            Debug.LogWarning(debugName + ": Unable to find a valid destination after " + maxAttempts + " attempts.");
        }

        Unit SeeEnemy()
        {
            RaycastHit raycastInfo;
            Vector3 rayToTarget = agent.destination - agent.transform.position;
            if (Physics.Raycast(this.transform.position, rayToTarget, out raycastInfo))
            {
                if (raycastInfo.collider != null)
                {
                    Unit unit = raycastInfo.collider.GetComponent<Unit>();

                    if (unit != null && IsEnemy(unit) && currentBehaviour != BEHAVIOURS.ATTACKING) // if sees enemy and we aren't already attacking
                    {
                        SendMessage(new SpottedEnemyMessage(this, leader, unit));
                        return unit;
                    }
                }
            }
            return null;

        }

        public bool IsEnemy(Unit unit)
        {
            return unit.team != this.team;
        }

        void Search()
        {
            if (agent.remainingDistance < 5f)
            {
                if (isLeader)
                {
                    Debug.Log("pog");
                    SendMessage(new ReachedDestinationMessage(this, this)); // could del this: called below as well
                    SetRandomDestination();
                }
                else
                {
                    if (leader != null)
                    {
                        if (agent.destination == leader.agent.destination)
                        {
                            // agent.SetDestination(leader.transform.position);
                            // AskLeaderForDestination();
                            SendMessage(new ReachedDestinationMessage(this, leader));
                        }
                        else
                        {
                            agent.SetDestination(leader.agent.destination);
                            // AskLeaderForDestination();
                        }
                    }
                }
            }
        }
        void SetEnemy(Unit enemy)
        {
            currentEnemy = enemy;
            currentBehaviour = BEHAVIOURS.GOING;
        }

        void ClearEnemy()
        {
            currentEnemy = null;
            currentBehaviour = BEHAVIOURS.WANDERING;
        }

        void Retreat()
        {
            ClearEnemy();
            agent.SetDestination(leader.transform.position);
        }

        void Go()
        {
            if (currentEnemy != null)
            {
                agent.SetDestination(currentEnemy.transform.position);

                if (agent.remainingDistance < this.atkReach)
                {
                    if (currentEnemy.currentEnemy == null) // if enemy is not already attacking someone else
                    {
                        currentEnemy.currentEnemy = this;
                    }
                    if (currentEnemy.currentBehaviour != BEHAVIOURS.ATTACKING)
                    {
                        currentEnemy.currentBehaviour = BEHAVIOURS.GOING;
                    }
                    this.currentBehaviour = BEHAVIOURS.ATTACKING;
                }
            }
            else
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }

        }

        void ReduceHp(int dmgTaken)
        {
            this.currentHp -= dmgTaken;
            if (this.currentHp <= 0)
            {
                if (currentEnemy == null)
                {
                    Debug.LogError("currentEnemy is null, cannot update enemy's perceived power");
                }
                else
                {
                    if (currentEnemy.perceivedPower < this.power)
                    {
                        currentEnemy.perceivedPower = this.power;
                    }
                }
                List<Unit> subTeam = SubTeamManager.GetSubTeam(this);
                if (subTeam == null)
                {
                    Debug.LogWarning($"Subteam is null, cannot remove unit {debugName} from subteam list");
                    return;
                }

                Debug.Log($"Before Remove: {string.Join(", ", subTeam)}");
                bool removed = subTeam.Remove(this);
                Debug.Log(removed ? "successfully removed unit from subteam list" : "could not remove unit from subteam list" + $". After Remove: {string.Join(", ", subTeam)}");

                if (isLeader)
                {
                    Debug.Log("Leader " + leader.gameObject.name + " died, trying to find new leader");

                    SubTeamManager.AssignLeader((int)team, subTeam);
                    Debug.Log("Leader died, new leader assigned: " + leader.gameObject.name);
                }
                Destroy(this.gameObject);
                return;
            }
        }

        public int GetHp()
        {
            return currentHp;
        }

        public int GetHPPercentage()
        {
            return (int)((currentHp / maxHp) * 100);
        }

        public int GetEnemyHp(Unit enemy)
        {
            if (enemy.hasBeenAttacked)
            {
                return enemy.GetHp();
            }

            // hp unknown
            return -1;
        }

        public bool IsLowHp()
        {
            return GetHPPercentage() < 30;
        }


        public float GetVelocity()
        {
            // Retourne la vitesse normalisée de l'agent (entre 0 et 1)
            return Mathf.Clamp01(agent.velocity.magnitude / agent.speed);
        }


        public List<Unit> GetFriendlyTroopsNearby()
        {
            List<Unit> units = new();
            float rangeRadius = 5f;

            foreach (Transform child in transform.parent)
            {
                Unit unit = child.GetComponent<Unit>();

                if (unit != null && Vector3.Distance(child.position, transform.position) <= rangeRadius)
                {
                    units.Add(unit);
                }
            }
            // or simpler is to use team field and compare in nearby troops

            return units;
        }

        public int GetPower()
        {
            return power;
        }

        /// <summary>
        /// Decides if this should attack enemy.
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns>
        /// true if this should attack enemy, false otherwise.
        /// </returns>
        public bool ShouldAttackEnemy(Unit enemy)
        {
            if (enemy.currentEnemy != null && !enemy.currentEnemy.IsLowHp() && enemy.currentEnemy.team == team) // if enemy is already attacking a friend that isn't low hp
            {
                return false; // ignore enemy if friend is already attacking it and isn't in trouble
            }
            if (enemy.IsLowHp() || enemy.perceivedPower <= this.power) // if power unknown or enemy is weaker / same power, or if enemy is low hp
            {
                return true; // attack enemy
            }
            return false; // ask for help
        }

        // envoi de messages

        public void SendMessage(Message message)
        {
            if (message.recipient != null)
            {
                message.recipient.ReceivedMessages.Enqueue(message);
                message.recipient.ProcessNextMessage();
            }
        }

        public void ProcessNextMessage()
        {
            if (ReceivedMessages.Count == 0)
            {
                Debug.LogWarning("Message processing called when no new messages to process");
                return;
            }

            Message message = ReceivedMessages.Dequeue();

            switch (message)
            {
                case AskForHelp askForHelp:
                    HandleAskForHelp(askForHelp);
                    break;
                case AttackEnemyMessage attackEnemyMessage:
                    HandleAttackEnemy(attackEnemyMessage);
                    break;
                case GoHelpMessage goHelpMessage:
                    HandleGoHelp(goHelpMessage);
                    break;
                case GoToAreaMessage goToAreaMessage:
                    HandleGoToArea(goToAreaMessage);
                    break;
                case GoToMessage goToMessage:
                    HandleGoTo(goToMessage);
                    break;
                case NeedHelpMessage needHelpMessage:
                    HandleNeedHelp(needHelpMessage);
                    break;
                case ReachedDestinationMessage reachedDestinationMessage:
                    HandleReachedDestination(reachedDestinationMessage);
                    break;
                case RetreatMessage retreatMessage:
                    HandleRetreat(retreatMessage);
                    break;
                case ShareGroupStatusMessage shareGroupStatusMessage:
                    HandleShareGroupStatus(shareGroupStatusMessage);
                    break;
                case SharePositionMessage sharePositionMessage:
                    HandleSharePosition(sharePositionMessage);
                    break;
                case SpottedEnemyMessage spottedEnemyMessage:
                    HandleSpottedEnemy(spottedEnemyMessage);
                    break;
            }

            if (ReceivedMessages.Count > 0)
            {
                ProcessNextMessage();
            }
        }

        /// <summary>
        /// If enough power is available from nearby troops, will attack enemy, else will retreat to leader.
        /// </summary>
        /// <param name="askForHelp"></param>
        private void HandleAskForHelp(AskForHelp askForHelp)
        {
            // gather power of nearby troops
            List<Unit> nearbyTroops = GetFriendlyTroopsNearby();
            List<Unit> availableNearbyTroops = new();
            int combinedPower = 0;
            foreach (Unit unit in nearbyTroops)
            { // instead do it with a message that is a coroutine
                if (unit.currentBehaviour != BEHAVIOURS.ATTACKING || unit.currentEnemy == askForHelp.enemy) // if not currently attacking a different enemy
                {
                    combinedPower += unit.GetPower();
                    availableNearbyTroops.Add(unit);
                }
            }

            if (combinedPower >= askForHelp.enemy.perceivedPower) // if enough power to attack enemy
            {
                foreach (var friend in availableNearbyTroops)
                {
                    SendMessage(new AttackEnemyMessage(this, friend, askForHelp.enemy));
                }
                SetEnemy(askForHelp.enemy);
            }
            else
            {
                Retreat();
            }
        }

        /// <summary>
        /// Sets enemy as the current enemy after receiving the order to attack from the leader.
        /// </summary>
        /// <param name="attackEnemyMessage"></param>
        private void HandleAttackEnemy(AttackEnemyMessage attackEnemyMessage)
        {
            SetEnemy(attackEnemyMessage.enemy);
        }

        /// <summary>
        /// Sets destination to friend's position and sets friend's enemy as the current enemy after receiving the order to go help the friend from the leader.
        /// </summary>
        /// <param name="goHelpMessage"></param>
        private void HandleGoHelp(GoHelpMessage goHelpMessage)
        {
            agent.SetDestination(goHelpMessage.friend.transform.position);
            SetEnemy(goHelpMessage.friend.currentEnemy);
        }

        /// <summary>
        /// Sets destination to a position in the area around goToAreaMessage.destination.
        /// </summary>
        /// <param name="goHelpMessage"></param>
        private void HandleGoToArea(GoToAreaMessage goToAreaMessage)
        {
            int maxIterations = 10; // Safeguard to prevent infinite loops
            int iterationCount = 0;

            do
            {
                SetRandomDestination(goToAreaMessage.destination.x - goToAreaMessage.radius, goToAreaMessage.destination.x + goToAreaMessage.radius,
                    goToAreaMessage.destination.z - goToAreaMessage.radius, goToAreaMessage.destination.z + goToAreaMessage.radius);

                iterationCount++;
                if (iterationCount >= maxIterations)
                {
                    Debug.LogWarning($"{debugName}: Unable to find a valid random destination within {maxIterations} attempts.");
                    break;
                }

            } while (agent.destination == goToAreaMessage.destination); // wait until the destination is different from the center of the area
        }

        /// <summary>
        /// Sets destination to the given position after receiving the order to go there from the leader.
        /// </summary>
        /// <param name="goToMessage"></param>
        private void HandleGoTo(GoToMessage goToMessage)
        {
            agent.SetDestination(goToMessage.destination);
        }

        /// <summary>
        /// If not currently attacking, sets enemy as the current enemy after receiving the ask for help from a nearby friendly troop.
        /// </summary>
        /// <param name="needHelpMessage"></param>
        private void HandleNeedHelp(NeedHelpMessage needHelpMessage)
        {
            if (currentBehaviour != BEHAVIOURS.ATTACKING)
            {
                SetEnemy(needHelpMessage.sender.currentEnemy);
            }
        }

        /// <summary>
        /// Tells the sender to wander around the area of the leader's destination while waiting for all the other troops to arrive.
        /// If all units have arrived, sets a new destination and tells all the other troops to go there.
        /// </summary>
        /// <param name="reachedDestinationMessage"></param>
        private void HandleReachedDestination(ReachedDestinationMessage reachedDestinationMessage)
        {
            if (!isLeader)
            {
                Debug.LogWarning("ReachedDestinationMessage received by a unit that is not a leader");
                return;
            }
            
            bool haveAllArrived = true;
            foreach (var unit in SubTeamManager.GetSubTeam(this))
            {
                if (unit != this && (unit == reachedDestinationMessage.sender || unit.agent.destination != agent.destination)) // unit already reached destination and it's not the leader
                {
                    Debug.Log(unit.debugName + " reached destination, sending message to wander around leader's destination");
                    // wander around leader's destination while waiting for all the other troops
                    SendMessage(new GoToAreaMessage(this, unit, agent.destination));
                }
                else // unit hasn't reached destination yet
                {
                    Debug.Log(unit.debugName + " hasn't reached destination yet, waiting for it to arrive");
                    haveAllArrived = false;
                }
            }
            if (haveAllArrived)
            {
                Debug.Log("all have arrived, setting new destination");
                // all units have reached destination, leader can set a new destination
                SetRandomDestination();
                foreach (var unit in SubTeamManager.GetSubTeam(this))
                {
                    if (unit != this)
                    {
                        SendMessage(new GoToMessage(this, unit, agent.destination));
                    }
                }
            }
        }

        /// <summary>
        /// Retreats to leader after receiving the order to retreat.
        /// </summary>
        /// <param name="retreatMessage"></param>
        private void HandleRetreat(RetreatMessage retreatMessage)
        {
            Retreat();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareGroupStatusMessage"></param>
        private void HandleShareGroupStatus(ShareGroupStatusMessage shareGroupStatusMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Leader decides if units are too far apart and need to move closer to each other.
        /// </summary>
        /// <param name="sharePositionMessage"></param>
        private void HandleSharePosition(SharePositionMessage sharePositionMessage)
        {
            if (!isLeader)
            {
                Debug.LogWarning("position shared to a unit that is not a leader");
                return;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Leader responds to sender, telling them if they should attack or ask for support. If enough support is available, will attack enemy, else will retreat to leader.
        /// </summary>
        /// <param name="spottedEnemyMessage"></param>
        private void HandleSpottedEnemy(SpottedEnemyMessage spottedEnemyMessage)
        {
            if (isLeader)
            {
                // decide if should attack enemy or ask for help before attacking

                if (spottedEnemyMessage.sender.ShouldAttackEnemy(spottedEnemyMessage.enemy))
                {
                    SendMessage(new AttackEnemyMessage(this, spottedEnemyMessage.sender, spottedEnemyMessage.enemy));
                }

                else
                {
                    // then : instead of retreat, tell it to ask for more power from nearby troops before attacking (AskForHelp msg) > 
                    //  sender asks nearby troops if they can potentially help (NOT NeedHelp msg) and gathers combined power of those available
                    //  if combined power is enough, sender attacks enemy
                    //  else, sender retreats
                    SendMessage(new AskForHelp(this, spottedEnemyMessage.sender, spottedEnemyMessage.enemy));
                    // SendMessage(new RetreatMessage(this, spottedEnemyMessage.sender));
                }
            }
            else
            {
                Debug.LogWarning("SpottedEnemyMessage received by a unit that is not a leader");
            }
        }

    }
}

