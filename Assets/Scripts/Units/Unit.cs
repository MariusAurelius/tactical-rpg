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

        public bool isLeader;
        public int team;
        public int maxHp = 0;
        public int currentHp = 0;
        public int atk;
        public int atkSpeed;
        public int atkReach;
        private float attackCooldown = 0f;

        public float movSpeed;

        public int power;

        public bool hasBeenAttacked;

        public Unit leader;

        //Agent
        private NavMeshAgent agent;
        private GameObject floor = null;
        private Bounds bnd;

        [Header("Animations")]
        [SerializeField]
        protected Animator _animator;

        public BEHAVIOURS currentBehaviour = BEHAVIOURS.WANDERING;
        public GameObject goal;
        public Unit currentEnemy;

        public Queue<Message> ReceivedMessages;

        void Start()
        {

            agent = this.GetComponent<NavMeshAgent>();
            floor = GameObject.Find("floor");
            bnd = floor.GetComponent<Renderer>().bounds;
            Search();

            leader = GetLeader();
            isLeader = (leader == this);
        }

        void Update()
        {

            if (this.currentEnemy == null)
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
                            currentEnemy = enemy;
                            currentBehaviour = BEHAVIOURS.GOING;
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
                if (attackCooldown <= 0f)
                {
                    currentEnemy.ReduceHp(this.atk);
                    attackCooldown = 1f / atkSpeed;  // Cooldown based on attack speed
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

            // Update the animator with the current speed
            _animator.SetFloat("speed", agent.velocity.magnitude);
        }

        void Go()
        {
            if (currentEnemy != null)
            {
                agent.SetDestination(currentEnemy.transform.position);

                Debug.Log(agent.remainingDistance < this.atkReach);
                if (agent.remainingDistance < this.atkReach)
                {

                    currentEnemy.currentEnemy = this;
                    if (currentEnemy.currentBehaviour != BEHAVIOURS.ATTACKING)
                    {

                        currentEnemy.currentBehaviour = BEHAVIOURS.GOING;
                    }
                    this.currentBehaviour = BEHAVIOURS.ATTACKING;
                }
            }

            // Update the animator with the current speed
            _animator.SetFloat("speed", agent.velocity.magnitude);

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
            if (hasBeenAttacked)
            {
                return currentHp;
            }

            // hp unknown
            return -1;
        }

        public int GetEnnemyHp(Unit ennemy)
        {
            return ennemy.GetHp();
        }


        public Unit GetLeader() {
         
            foreach (Transform child in transform.parent)
            {
                if (child.CompareTag("leader"))
                {
                    return child.GetComponent<Unit>();
                }
            }
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

            return units;
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
                case SharePositionMessage sharePositionMessage:
                    HandleSharePosition(sharePositionMessage);
                    break;
                case SpottedEnnemyMessage spottedEnnemyMessage:
                    HandleSpottedEnemy(spottedEnnemyMessage);
                    break;
                case AttackEnnemyMessage attackEnemyMessage:
                    HandleAttackEnemy(attackEnemyMessage);
                    break;
                case NeedHelpMessage needHelpMessage:
                    HandleNeedHelp(needHelpMessage);
                    break;
                case GoToMessage goToMessage:
                    HandleGoTo(goToMessage);
                    break;
                case GoHelpMessage goHelpMessage:
                    HandleGoHelp(goHelpMessage);
                    break;
            }

            if (ReceivedMessages.Count > 0)
            {
                ProcessNextMessage();
            }
        }

        private void HandleGoHelp(GoHelpMessage goHelpMessage)
        {
            agent.SetDestination(goHelpMessage.friend.transform.position);
        }

        private void HandleGoTo(GoToMessage goToMessage)
        {
            agent.SetDestination(goToMessage.destination);
        }

        private void HandleNeedHelp(NeedHelpMessage needHelpMessage)
        {
            throw new NotImplementedException();
        }

        private void HandleAttackEnemy(AttackEnnemyMessage attackEnemyMessage)
        {
            throw new NotImplementedException();
        }

        private void HandleSpottedEnemy(SpottedEnnemyMessage spottedEnnemyMessage)
        {
            throw new NotImplementedException();
        }

        private void HandleSharePosition(SharePositionMessage sharePositionMessage)
        {
            throw new NotImplementedException();
        }


        // ...

        // fonctions pour le leader
        // ...

    }
}