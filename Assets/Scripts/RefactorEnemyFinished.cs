using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefactorEnemyFinished : MonoBehaviour
{
    public Stats enemyStats;

    [Tooltip("The transform that will lock onto the player once the enemy has spotted them.")]
    public Transform sight;

    [Tooltip("The transform to which the enemy will pace back and forth to.")]
    public Transform[] patrolPoints;

    [Tooltip("Blue explosion particles")]
    public GameObject enemyExplosionParticles;

    public int currentPatrolPoint = 0;

    public bool slipping = false;
   
    public float facing;
    
    public Rigidbody rb;

    private GameObject player;

    /// <summary>
    /// Contains tunable parameters to tweak the enemy's movement and behavior.
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]
        [Tooltip("How fast the enemy walks (only when idle is true).")]
        public float walkSpeed;

        [Tooltip("How fast the enemy turns in circles as they're walking (only when idle is true).")]
        public float rotateSpeed;

        [Tooltip("How fast the enemy runs after the player (only when idle is false).")]
        public float chaseSpeed;

        [Tooltip("Whether the enemy is idle or not. Once the player is within distance, idle will turn false and the enemy will chase the player.")]
        public bool idle;

        [Tooltip("How close the enemy needs to be to explode")]
        public float explodeDist;

    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (enemyStats.idle)
        {
            Patrol();
        }
        else
        {
            ChasePlayer();
        }

        if (slipping)
        {
            SlipBack();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            Debug.LogError("Patrol points array is empty.");
            return;
        }

        Vector3 moveToPoint = patrolPoints[currentPatrolPoint].position;
        transform.position = Vector3.MoveTowards(transform.position, moveToPoint, enemyStats.walkSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, moveToPoint) < 0.01f)
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(sight);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);

            if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.explodeDist)
            {
                StartCoroutine(Explode());
                enemyStats.idle = true;
            }
        }
    }

    private void SlipBack()
    {
        transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
    }
    private void OnCollisionEnter(Collision other)
    {
        slipping = other.gameObject.layer == 9;
    }


   private void OnTriggerEnter(Collider other)
    {
        //start chasing if the player gets close enough
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            enemyStats.idle = false;
        }
    }

   private void OnTriggerExit(Collider other)
    {
        //stop chasing if the player gets far enough away
        if (other.gameObject.tag == "Player")
        {
            enemyStats.idle = true;      
        }
    }

    private IEnumerator Explode()
    {
        GameObject particles = Instantiate(enemyExplosionParticles, transform.position, new Quaternion());
        yield return new WaitForSeconds(0.2f);
        Destroy(transform.parent.gameObject);
    }


}