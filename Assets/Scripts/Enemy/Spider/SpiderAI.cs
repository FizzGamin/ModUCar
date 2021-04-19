using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public float timeBetweenAttacks; // this is how long the truck will be driving away from the player so that it can then turn and charge the player.
    bool hitPlayer; // used for making the truck go out a bit after hitting the player before attacking again (to perform more of a charging action).

    // STATES
    public float sightRange;
    bool playerInSightRange;

    public override void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Invoke(nameof(OnDeath), .5f);
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
        //give acceleraction a random value between a range (15, 50)
        int randInt = Random.Range(15, 51);
        agent.acceleration = randInt;
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();
    }

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
        float angle = Vector3.Angle(transform.forward, player.position); //changed walkPoint to player.position

        //Debug.DrawLine(Vector3.zero, transform.forward, Color.green, 10f);
        //Debug.DrawLine(Vector3.zero, player.position, Color.red, 10f);
        /*
        if (agent.pathPending == true)
        {
            agent.speed = 0;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
        }*/

        if (angle > 20)
            transform.LookAt(player.position); // WILL NEED TO CHANGE THIS. Make it so it follows along the up axis.

        agent.SetDestination(player.position);
    }

    private void OnTriggerStay(Collider other)
    {
        //Spider can damage either the truck of the player
        if (other.gameObject.tag == "Player" && !hitPlayer)
        {
            playerObj.GetComponent<PlayerController>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
        if (other.gameObject.tag == "Enemy_Truck")
        {
            other.gameObject.GetComponent<TruckAI>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }

    private void ResetAttack()
    {
        hitPlayer = false;
    }

    /* Creates a gizmo showing the sight range of the spider */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


    /*
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(GoToLocation), 3, 5);
    }

    private void GoToLocation()
    {
        agent.SetDestination(new Vector3(Random.Range(25f, 100f), 0, Random.Range(25f, 100f)));
        transform.eulerAngles = new Vector3(0, 180, 0);
    }
    */
}
