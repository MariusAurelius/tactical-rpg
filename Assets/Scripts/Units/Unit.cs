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
        public float maxHp = 0;
        public float currentHp = 0;
        public float atk;
        public float atkSpeed;
        public float atkReach;
        private float attackCooldown = 0f;

        public float movSpeed;

        public float power;

        //Agent
        private NavMeshAgent agent;

        public BEHAVIOURS currentBehaviour = BEHAVIOURS.WANDERING;
        public GameObject goal;
        public Unit currentEnemy;

        // Start is called before the first frame update
        void Start()
        {

            agent = this.GetComponent<NavMeshAgent>();
            Search();
        }

        // Update is called once per frame
        void Update()
        {

            if (this.currentEnemy == null)
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }
            Debug.Log(currentBehaviour);
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
                        currentEnemy = enemy;
                        currentBehaviour = BEHAVIOURS.GOING;
                    }
                    break;
                case BEHAVIOURS.GOING:
                    Go(); break;
                case BEHAVIOURS.ATTACKING:
                    Attack(); break;

                default: throw new Exception("Unknown behaviour"); break;
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

        void Search()
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
                        return unit;
                    }
                }
            }
            return null;

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

        }

        void ReduceHp(float dmgTaken)
        {
            this.currentHp -= dmgTaken;
            if (this.currentHp <= 0)
            {
                Destroy(this.gameObject);
                return;
            }
        }
    }
}