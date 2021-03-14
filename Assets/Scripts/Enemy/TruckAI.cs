using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TruckAI : IEnemy
{
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    Transform player;
    
    public float health;

    // PATROL
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // ATTACKING
    public float timeBetweenAttacks; // this is how long the truck will be driving away from the player so that it can then turn and charge the player.
    bool hitPlayer; // used for making the truck go out a bit after hitting the player before attacking again (to perform more of a charging action).

    // STATES
    public float sightRange;
    bool playerInSightRange;

    public override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Invoke(nameof(DestroyEnemy), .5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // called every frame
    private void Patrol()
    {
        if (!walkPointSet)
        {
            Invoke(nameof(SearchWalkPoint), 1f);
        }
        if (walkPointSet)
        {   
            // IMPLEMENT CODE FROM THE ATTACKPLAYER METHOD WHEN IT WORKS
            //agent.SetDestination(walkPoint);
        }

        // check if we have reached the walkPoint
        Vector3 distToWalkPoint = transform.position - walkPoint;
        if (distToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // create the point to go to
        float randZ = Random.Range(-walkPointRange, walkPointRange);
        float randX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        // check if the point to walk to is on the ground
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    // called every frame
    private void AttackPlayer()
    {
        // turn the wheels toward the player
        // IF THE ANGLE IS WITHIN A CERTAIN DEGREE, THEN THE TRUCK CAN ACCELERATE. 
        // IF IT IS OUTSIDE THAT RANGE, THE TRUCK SHOULD TURN ITS WHEELS SHARP BUT HAVE A MAXIMUM TURN DEGREE AND NOT ACCELERATE BUT GO AT A CONSTANT SPEED.
        float angle = Vector3.Angle(transform.forward, player.position); // changed walkPoint to player.position
        if (angle > 45)
        {
            // turn wheels sharply and set speed to something slower

        }
        
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        float str = Mathf.Min(.5f * Time.deltaTime, 1);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);

        // apply the appropriate speed to the wheels
        //agent.SetDestination(player.position);

        //transform.LookAt(player); // WILL NEED TO CHANGE THIS. Make it so it only follows the up axis.

        if (!hitPlayer) // WILL NEED TO REFACTOR THIS STATEMENT TO CHECK IF THE TRUCK ACTUALLY HIT THE PLAYER
        {

            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }

    private void ResetAttack()
    {
        // add code here for going away from the player to then be able to charge the player !!!


        hitPlayer = false;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


}
