using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpiderAI : IEnemy 
{
    public NavMeshAgent agent;
    
    public LayerMask whatIsGround, whatIsPlayer;
    public List<SpiderIKSolver> legs1;
    public List<SpiderIKlegsSecond> legs2; 
    public List<SpiderIKlegsBack> legs3;
    public float health;

    Transform player;
    GameObject playerObj;

    // PATROL
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // ATTACKING
    public float timeBetweenAttacks;
    bool hitPlayer;
    bool hitEnemy;

    // STATES
    public float sightRange;
    bool playerInSightRange;

    private GameObject healthBar;
    public override void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Spider Health: " + health);
        healthBar.GetComponent<Image>().fillAmount -= (0.01f * damage * 100/health);
        if (health <= 0)
            Invoke(nameof(OnDeath), .1f);
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        //give acceleraction a random value between a range (15, 50) for different difficulties.
        int randInt = Random.Range(15, 51);
        agent.acceleration = randInt;
        healthBar = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();
    }

    /// <summary>
    /// Handles the patrolling mode for the Spider. Changes speed of Spider and procedural legs and goes to walk point.
    /// </summary>
    private void Patrol()
    {
        // Update to patrol speeds
        agent.speed = 10;
        foreach (SpiderIKSolver leg in legs1)
            leg.SpiderPatrolSpeed();
        foreach (SpiderIKlegsSecond leg in legs2)
            leg.SpiderPatrolSpeed();
        foreach (SpiderIKlegsBack leg in legs3)
            leg.SpiderPatrolSpeed();

        if (!walkPointSet)
        {
            Invoke(nameof(SearchWalkPoint), 1);
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        // check if we have reached the walkPoint
        Vector3 distToWalkPoint = transform.position - walkPoint;
        if (distToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    /// <summary>
    /// Searches for a random point for the Spider to walk to within walkPointRange and goes there if it is a valid point.
    /// </summary>
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

    /// <summary>
    /// Handles the attacking mode for the Spider, changes the speed of the spider and legs, and looks toward the player if the angle between the two gets too big.
    /// </summary>
    private void AttackPlayer()
    {
        // Update to attack speeds
        agent.speed = 30;
        foreach (SpiderIKSolver leg in legs1)
            leg.SpiderChaseSpeed();
        foreach (SpiderIKlegsSecond leg in legs2)
            leg.SpiderChaseSpeed();
        foreach (SpiderIKlegsBack leg in legs3)
            leg.SpiderChaseSpeed();

        // look toward the player
        float angle = Vector3.Angle(transform.forward, player.position);

        if (angle > 20)
        {
            Vector3 targetPosition = new Vector3(player.position.x, this.transform.position.y, player.position.z);
            transform.LookAt(targetPosition);
        }

        agent.SetDestination(player.position);
    }

    /// <summary>
    /// When the spider's attack colliders collide with something, checks if it is something that can take damage and call the appropriate takeDamage function.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //Spider can damage either the truck of the player
        if (other.gameObject.tag == "Player" && !hitPlayer)
        {
            playerObj.GetComponent<PlayerController>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
        if (other.gameObject.tag == "Enemy_Truck" && !hitEnemy)
        {
            other.gameObject.GetComponent<TruckAI>().TakeDamage(10);
            hitEnemy = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }
    private void ResetAttack()
    {
        hitPlayer = false;
        hitEnemy = false;
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the Spider
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
