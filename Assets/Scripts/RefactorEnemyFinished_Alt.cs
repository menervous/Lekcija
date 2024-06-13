﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefactorEnemyFinished_Alt : MonoBehaviour
{
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

        [Tooltip("How fast the enemy flees from the player (only when idle is false and player health is at maximum).")]
        public float fleeSpeed;
    }

    public Stats enemyStats;

    //The transform that will lock onto the player once the enemy has spotted them.
    public Transform sight;

    //Whether the enemy is idle or not. Once the player is within distance, idle will turn false and the enemy will chase the player.
    private bool idle = true;

    private bool slipping = false;

    private float facing;

    private Rigidbody rb;

    private GameObject player;

    private PatrolFinished patrolBehavior;

    private ProximityExplodeFinished explodeBehavior;

    private GetHit playerHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        patrolBehavior = GetComponent<PatrolFinished>();
        explodeBehavior = GetComponent<ProximityExplodeFinished>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<GetHit>();
    }

    private void Update()
    {
        // changes the enemy's behavior: pacing in circles, chasing the player, or fleeing from the player
        if (idle)
        {
            patrolBehavior.Move(enemyStats.walkSpeed);
        }
        else
        {
            if (playerHealth.health == playerHealth.baseHealth)
            {
                Flee();
            }
            else
            {
                Chase();
            }
            if (explodeBehavior.CheckForExplosion(player.transform))
            {
                idle = true;
            }
        }
        CheckIfSlipping();
    }

    private void CheckIfSlipping()
    {
        // stops enemy from following player up the inaccessible slopes
        if (slipping == true)
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
        }
    }

    private void Chase()
    {
        //Chase the player
        sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(sight);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);
    }

    private void Flee()
    {
        //Flee from the player
        Vector3 directionToFlee = transform.position - player.transform.position;
        directionToFlee.y = 0; // Keep the enemy at the same height
        transform.rotation = Quaternion.LookRotation(directionToFlee);
        transform.position = Vector3.MoveTowards(transform.position, transform.position + directionToFlee, Time.deltaTime * enemyStats.fleeSpeed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 9)
        {
            slipping = true;
        }
        else
        {
            slipping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //start chasing if the player gets close enough
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            idle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //stop chasing if the player gets far enough away
        if (other.gameObject.tag == "Player")
        {
            idle = true;
        }
    }
}