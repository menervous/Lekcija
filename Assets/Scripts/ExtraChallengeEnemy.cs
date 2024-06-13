using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraChallengeEnemy : MonoBehaviour
{
    public Stats enemyStats;

    [Tooltip("The transform to which the enemy will pace back and forth to.")]
    public Transform[] patrolPoints;

    private int currentPatrolPoint = 1;  // Changed from float to int

    /// <summary>
    /// Contains tunable parameters to tweak the enemy's movement.
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]

        [Tooltip("How fast the enemy moves.")]
        public float speed;

        [Tooltip("Whether the enemy should move or not")]
        public bool move;
    }

    void Start()
    {
        // Initialize struct fields if necessary
        enemyStats.speed = 10;
        enemyStats.move = false; // or true based on your requirement
    }

    void Update()
    {
        // If the enemy is allowed to move
        if (enemyStats.move == true)  // Changed from assignment to comparison
        {
            Vector3 moveToPoint = patrolPoints[currentPatrolPoint].position;
            transform.position = Vector3.MoveTowards(transform.position, moveToPoint, enemyStats.speed * Time.deltaTime);
           
            if (Vector3.Distance(transform.position, moveToPoint) < 0.01f)  // Fixed Vector3.Distance usage
            {
                currentPatrolPoint++;

                if (currentPatrolPoint >= patrolPoints.Length)  // Fixed array index logic
                {
                    currentPatrolPoint = 0;  // Changed from == to = for assignment
                }
            }
        }
    }
}