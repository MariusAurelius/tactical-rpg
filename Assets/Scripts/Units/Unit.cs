using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.AI;
using static Unity.VisualScripting.Metadata;

namespace AgentScript
{
    public enum Team:int
    {
        RED = 1,
        BLUE = 2
    }
    public class Unit : MonoBehaviour
    {
        private static bool USE_MINIMAX = true;
        private static int MINIMAX_DEPTH = 3;
        public enum BEHAVIOURS
        {
            WANDERING,
            GOING,
            ATTACKING
        }

        public int id;

        public bool isLeader;
        public Team team;
        protected int maxHp = 0;
        protected int currentHp = 0;
        protected int atk;
        protected int atkSpeed;
        protected int atkReach;
        protected float attackCooldown = 0f;
        protected bool hasCalledForHelp = false;

        [SerializeField] private float visionAngle = 60f; // Angle de vision en degrés
        [SerializeField] private float visionRange = 100f; // Portée de vision

        protected float movSpeed;

        protected int power;

        public int perceivedPower = -1; // unknown to enemy

        public bool hasBeenAttacked;

        public Unit leader;

        //Agent
        private NavMeshAgent agent;
        private GameObject floor = null;
        private Bounds bnd;
        public BEHAVIOURS currentBehaviour = BEHAVIOURS.WANDERING;
        public GameObject goal;
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
            floor = GameObject.Find("floor");
            bnd = floor.GetComponent<Renderer>().bounds;
            Search();
            ReceivedMessages = new Queue<Message>();
        }

        void Update()
        {
            if (currentBehaviour == BEHAVIOURS.ATTACKING)
        {
            agent.isStopped = true; // Empêche le mouvement pendant l'attaque
        }
        else
        {
            agent.isStopped = false; // Autorise le mouvement dans les autres comportements
        }
          
            if (this.currentEnemy == null /*&& leader didn't tell us where to go: agent.destination or behaviour wandering*/)
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }
            switch (currentBehaviour)
            {
                case BEHAVIOURS.WANDERING:
                    if (agent.remainingDistance == 0.5f)
                    {
                        Search();
                    }
                    Unit enemy = SeeEnemy();
                    break;
                case BEHAVIOURS.GOING:
                    Go(); break;
                case BEHAVIOURS.ATTACKING:
                    Attack(); break;
                default: throw new Exception("Unknown behaviour"); //break;
            }
        }

        void Attack()
        {
            if (currentEnemy != null)
            {
                agent.isStopped = true;
                
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
                agent.isStopped = false;
            }
        }

        void SetRandomDestination()
        {
            float rx = UnityEngine.Random.Range(-50, 50);
            float rz = UnityEngine.Random.Range(-50, 50);
            Vector3 moveto = new Vector3(rx, this.transform.position.y, rz);
            agent.SetDestination(moveto);
        }

        // Unit SeeEnemy()
        // {
        //     RaycastHit raycastInfo;
        //     Vector3 rayToTarget = agent.destination - agent.transform.position;
        //     if (Physics.Raycast(this.transform.position, rayToTarget, out raycastInfo))
        //     {
        //         if (raycastInfo.collider != null)
        //         {
        //             Unit unit = raycastInfo.collider.GetComponent<Unit>();

        //             if (unit != null && IsEnemy(unit))
        //             {
        //                 Debug.Log("Enemy seen: " + unit);
        //                 SendMessage(new SpottedEnemyMessage(this, leader, unit));
        //                 return unit;
        //             }
        //         }
        //     }
        //     return null;

        // }

        Unit SeeEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, visionRange); // Détecte tous les objets dans la portée
            foreach (Collider collider in colliders)
            {
                Unit unit = collider.GetComponent<Unit>();
                if (unit != null && IsEnemy(unit))
                {
                    // Vérifie si l'unité est dans le champ de vision
                    Vector3 directionToUnit = (unit.transform.position - transform.position).normalized;
                    float angleToUnit = Vector3.Angle(transform.forward, directionToUnit);

                    if (angleToUnit <= visionAngle / 2) // Si l'unité est dans l'angle de vision
                    {
                        // Vérifie si un obstacle bloque la ligne de vue
                        Ray ray = new Ray(transform.position, directionToUnit);
                        Debug.DrawRay(transform.position, directionToUnit * visionRange, Color.red, 0.1f); // Visualise le raycast
                        if (Physics.Raycast(ray, out RaycastHit hit, visionRange))
                        {
                            if (hit.collider.gameObject == unit.gameObject) // Si le raycast touche l'unité
                            {
                                Debug.DrawRay(transform.position, directionToUnit * visionRange, Color.green, 0.1f); // Rayon vert si l'ennemi est visible
                                Debug.Log("Enemy seen: " + unit);
                                SendMessage(new SpottedEnemyMessage(this, leader, unit));
                                return unit;
                            }
                            else
                            {
                                Debug.DrawRay(transform.position, directionToUnit * visionRange, Color.red, 0.1f); // Rayon rouge si un obstacle bloque la vue
                                Debug.Log("Enemy blocked by obstacle: " + hit.collider.gameObject.name);
                            }
                        }
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Draws the vision range and angle in the editor.
        /// </summary>
        // private void OnDrawGizmos()
        // {
        //     // Dessine la portée de vision
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, visionRange);

        //     // Dessine le cône de vision
        //     Vector3 forward = transform.forward * visionRange;
        //     Quaternion leftRayRotation = Quaternion.Euler(0, -visionAngle / 2, 0);
        //     Quaternion rightRayRotation = Quaternion.Euler(0, visionAngle / 2, 0);

        //     Vector3 leftRayDirection = leftRayRotation * forward;
        //     Vector3 rightRayDirection = rightRayRotation * forward;

        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawRay(transform.position, leftRayDirection);
        //     Gizmos.DrawRay(transform.position, rightRayDirection);
        // }

        public bool IsEnemy(Unit unit)
        {
            return unit.team != this.team;
        }

        void Search()
        {
            if (agent.remainingDistance < 5f)
            {
                SetRandomDestination();
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

                // Debug.Log(agent.remainingDistance < this.atkReach);
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
                    agent.isStopped = true;
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
            if(isLowHp() && !hasCalledForHelp)
            {
                chooseBestAction(this, currentEnemy);
                hasCalledForHelp = true;
            }
            if (this.currentHp <= 0)
            {
                List<Unit> subTeam = SubTeamManager.GetSubTeam(this); // should assign this to subTeams[this subteam]
                subTeam.Remove(this);
                currentEnemy.perceivedPower = Math.Max(this.power,currentEnemy.perceivedPower);
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

        public bool isLowHp()
        {
            return GetHPPercentage() < 30;
        }


        public Unit GetLeader()
        {
            Debug.Log(transform.parent.name);
            foreach (Transform child in transform.parent)
            {
                Debug.Log(child.name);
                if (child.CompareTag("Leader"))
                {
                    Debug.LogWarning("Leader found");
                    return child.GetComponent<Unit>();
                }
            }
            Debug.LogWarning("No leader found");
            return null;

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
            if (enemy.currentEnemy != null && !enemy.currentEnemy.isLowHp() && enemy.currentEnemy.team == team) // if enemy is already attacking a friend that isn't low hp
            {
                return false;
            }
            if (enemy.isLowHp() || enemy.perceivedPower <= this.power) // if power unknown or enemy is weaker / same power, or if enemy is low hp
            {
                return true;
            }
            return false;
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
                case GoToMessage goToMessage:
                    HandleGoTo(goToMessage);
                    break;
                case NeedHelpMessage needHelpMessage:
                    HandleNeedHelp(needHelpMessage);
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
                // decide if should attack enemy or ask for help before attacking with score based by minimax
                if (USE_MINIMAX)
                {
                    chooseBestAction(spottedEnemyMessage.sender, spottedEnemyMessage.enemy);
                }
                else
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
            }
            else
            {
                Debug.LogWarning("SpottedEnemyMessage received by a unit that is not a leader");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackingUnit"></param>
        /// <param name="defendingUnit"></param>
        public void chooseBestAction(Unit attackingUnit, Unit defendingUnit)
        {
            Node origin = new Node(MINIMAX_DEPTH, attackingUnit, defendingUnit);
            Node bestMove = origin.children.OrderByDescending(child => child.score).FirstOrDefault();
            switch (bestMove.associatedState)
            {
                case Node.ENCOUNTER_STATES.ALLIES_CALLED:
                    SendMessage(new AskForHelp(this, attackingUnit, defendingUnit));
                    break;
                case Node.ENCOUNTER_STATES.ENNEMY_RETREAT_CALLED:
                    SendMessage(new AttackEnemyMessage(this, attackingUnit, defendingUnit));
                    break;
                case Node.ENCOUNTER_STATES.RETREAT_CALLED:
                case Node.ENCOUNTER_STATES.ENNEMIES_CALLED:
                    SendMessage(new RetreatMessage(this, attackingUnit));
                    break;
                default:
                    SendMessage(new RetreatMessage(this, attackingUnit));
                    break;

            }
        }




        /// <summary>
        /// Used to determine the action an unit should perform
        /// </summary>
        /// <param name="node">Screenshot of all the informations the attacking unit has about its environment</param>
        /// <param name="depth">Maximum depth search</param>
        /// <param name="maximizingPlayer">true if attacking unit, false if defending unit</param>
        /// <returns></returns>
        private static int Minimax(Node node, int depth, bool maximizingPlayer)
        {
            int value;
            if (depth == 0 || node.children.Count == 0)
            {
                return node.Evaluate();
            }

            if (maximizingPlayer)
            {
                value = Int32.MinValue;
                foreach (Node child in node.children)
                {
                    value = Math.Max(value, Minimax(child, depth - 1, true));
                }
            }
            else
            {
                value = Int32.MaxValue;
                foreach (Node child in node.children)
                {
                    value = Math.Min(value, Minimax(child, depth - 1, false));
                }
            }

            return value;



        }


        public class Node {
            public List<Node> children = new List<Node>();
            private Unit attackingUnit;
            private Unit defendingUnit;
            private List<Unit> nearbyAllies;
            private List<Unit> nearbyEnnemies;
            private bool alliesCalled = false;
            private bool enemiesCalled = false;
            private bool retreatCalled = false;
            private bool enemyRetreatCalled = false;
            public int score;
            public ENCOUNTER_STATES associatedState = ENCOUNTER_STATES.ALLIES_CALLED;
            public enum ENCOUNTER_STATES
            {
                ALLIES_CALLED, //The attacking unit is calling for help
                ENNEMIES_CALLED, //The defending unit is calling for help
                RETREAT_CALLED, //The attacking unit is retreating
                ENNEMY_RETREAT_CALLED //The defending unit is retreating
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="desiredDepth">Depth of the tree that will be built</param>
            /// <param name="_attackingUnit"></param>
            /// <param name="_defendingUnit"></param>
            public Node(int desiredDepth, Unit _attackingUnit, Unit _defendingUnit)
            {
                attackingUnit = _attackingUnit;
                defendingUnit = _defendingUnit;
                nearbyAllies = attackingUnit.GetFriendlyTroopsNearby();
                nearbyEnnemies = defendingUnit.GetFriendlyTroopsNearby();
                if (desiredDepth > 0)
                {

                    foreach (ENCOUNTER_STATES state in Enum.GetValues(typeof(ENCOUNTER_STATES)))
                    {
                        children.Add(new Node(desiredDepth - 1, state, this, true));
                    }
                }
                this.score = Minimax(this, desiredDepth, true);
            }
            private Node(int desiredDepth, ENCOUNTER_STATES stateToApply, Node parent, bool maximizingPlayer)
            {
                attackingUnit = parent.attackingUnit;
                defendingUnit = parent.defendingUnit;
                nearbyAllies = parent.nearbyAllies;
                nearbyEnnemies = parent.nearbyEnnemies;
                alliesCalled = parent.alliesCalled;
                enemiesCalled = parent.enemiesCalled;
                retreatCalled = parent.retreatCalled;
                enemyRetreatCalled = parent.enemyRetreatCalled;
                this.ApplyState(stateToApply);
                this.associatedState = stateToApply;
                if (desiredDepth > 0 && (!enemyRetreatCalled && !retreatCalled)) //A unit calling a retreat will prune the rest of the branch and make this node a terminal node
                {
                    foreach(ENCOUNTER_STATES state in Enum.GetValues(typeof(ENCOUNTER_STATES))){
                        children.Add(new Node(desiredDepth -  1, state, this, !maximizingPlayer));
                    }
                }
                this.score = Minimax(this, desiredDepth, maximizingPlayer); ;

            }

            public void ApplyState(ENCOUNTER_STATES stateToApply)
            {
                switch (stateToApply)
                {
                    case ENCOUNTER_STATES.ALLIES_CALLED:
                        alliesCalled = true; break;
                    case ENCOUNTER_STATES.ENNEMIES_CALLED:
                        enemyRetreatCalled = true; break;
                    case ENCOUNTER_STATES.RETREAT_CALLED:
                        retreatCalled = true; break;
                    case ENCOUNTER_STATES.ENNEMY_RETREAT_CALLED:
                        enemyRetreatCalled = true; break;
                    default:
                        throw new Exception("Unknown state");
                }
            }
            //TODO
            public int Evaluate()
            {
                int score = 0;
                if (!defendingUnit.hasBeenAttacked)
                {
                    score += 5;
                }
                if (defendingUnit.isLowHp())
                {
                    score += 10;
                }
                if (retreatCalled)
                {
                    score -= 5;
                    return score;
                }
                if (enemyRetreatCalled)
                {
                    score += 5;
                    return score;
                }
                if (defendingUnit.perceivedPower > attackingUnit.power)
                {
                    score -= 5;
                }
                else
                {
                    score += 5;
                }
                if (alliesCalled)
                {
                    foreach (Unit ally in nearbyAllies)
                    {
                        score += ally.power;
                    }

                }
                if (enemiesCalled)
                {
                    foreach(Unit ennemy in nearbyEnnemies)
                    {
                        score -= ennemy.perceivedPower;
                    }
                }
                return score;
            }
        }
    }
}

