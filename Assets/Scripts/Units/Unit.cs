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
        public float maxHp = 0;
        public float currentHp = 0;
        public float atk;
        public float atkSpeed;
        public float atkReach;

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
            if (this.currentHp <= 0)
            {
                Destroy(this);
            }
            if (this.currentEnemy == null)
            {
                currentBehaviour = BEHAVIOURS.WANDERING;
            }
            switch (currentBehaviour)
            {
                case BEHAVIOURS.WANDERING:
                    Search(); break;
                case BEHAVIOURS.GOING:
                    Go(); break;
                case BEHAVIOURS.ATTACKING:
                    Attack(); break;
                default: throw new Exception("Unknown behaviour"); break;
            }

        }

        void Attack()
        {
            this.currentEnemy.currentHp = this.currentEnemy.currentHp - this.atk;
        }

        void Search()
        {
            float rx = UnityEngine.Random.Range(-50, -50);
            float rz = UnityEngine.Random.Range(-50, -50);
            Vector3 moveto = new Vector3(rx, this.transform.position.y, rz);
            agent.SetDestination(moveto);
        }


        void Go()
        {
            if(currentEnemy != null)
            {
                agent.SetDestination(currentEnemy.transform.position);
            }
            if (agent.remainingDistance < this.atkReach)
            {
                this.currentBehaviour = BEHAVIOURS.ATTACKING;
            }

        }
    }
}