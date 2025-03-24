using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AgentScript
{
    public class Unit : MonoBehaviour
    {


        public enum BEHAVIOURS
        {
            WANDERING,
            GOING,
            ATTACKING
        }

        public enum Team
        {
            BLUE,
            RED
        }

        public bool isLeader;
        public int team; // Team team
        protected int maxHp = 0;
        protected int currentHp = 0;
        protected int atk;
        protected int atkSpeed;
        protected int atkReach;
        protected float attackCooldown = 0f;

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

        void Start()
        {
            agent = this.GetComponent<NavMeshAgent>();
            floor = GameObject.Find("floor");
            bnd = floor.GetComponent<Renderer>().bounds;
            Search();

            //leader = GetLeader();
            //isLeader = (leader == this);

            ReceivedMessages = new Queue<Message>();
        }

        void Update()
        {
            if (leader == null)
            {
                leader = GetLeader();
                isLeader = (leader == this);
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
                        Search();
                    }
                    Unit enemy = SeeEnemy();
                    if (enemy != null)
                    {
                        if (enemy.team != this.team)
                        {
                        SendMessage(new SpottedEnemyMessage(this, leader, enemy));
                        }
                    }
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

        void SetRandomDestination()
        {
            float rx = UnityEngine.Random.Range(-50, 50);
            float rz = UnityEngine.Random.Range(-50, 50);
            Vector3 moveto = new Vector3(rx, this.transform.position.y, rz);
            agent.SetDestination(moveto);
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

                    if (unit != null)
                    {

                        Debug.Log("Enemy seen");

                        Debug.Log(raycastInfo.collider.GetComponent<Unit>());
                        return unit;
                    }
                }
            }
            return null;

        }

        void Search()
        {
            if (agent.remainingDistance < 5f)
            {
                SetRandomDestination();
            }

            // Update the animator with the current velocity
            _animator.SetFloat("Velocity", agent.velocity.magnitude);
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
                }
            }

            // Update the animator with the current velocity
            _animator.SetFloat("Velocity", agent.velocity.magnitude);

        }

        void ReduceHp(int dmgTaken)
        {
            this.currentHp -= dmgTaken;
            if (this.currentHp <= 0)
            {
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


        public Unit GetLeader() {
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
        private void HandleRetreat(RetreatMessage retreatMessage) {
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
            else {
                Debug.LogWarning("SpottedEnemyMessage received by a unit that is not a leader");
            } 
        }

        


        // ...

        // fonctions pour le leader ?
        // ...

    }
}

