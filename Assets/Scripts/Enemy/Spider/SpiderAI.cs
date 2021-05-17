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
    private float maxHealth;
    public float spiderDamage;

    Transform player;
    GameObject playerObj;

    private Vector3 centerPoint = new Vector3(0, 2, 0);

    // PATROL
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    RaycastHit hit;
    bool paused;
    float waitTime;
    Rigidbody rb;

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
        Debug.Log("Enemy health: " + health + "  damage: " + damage);
        healthBar.GetComponent<Image>().fillAmount -= (0.01f * damage * 100/maxHealth);
        if (health <= 0)
            Invoke(nameof(OnDeath), .1f);
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }

    public Vector3 GetCenterPoint()
    {
        return centerPoint;
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
        maxHealth = health;
        paused = false;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();

        HandleWheelCollision();
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

        if (paused)
        {
            agent.isStopped = true;
        }
        if (!paused)
        {
            if (!walkPointSet)
            {
                Invoke(nameof(SearchWalkPoint), 0);
            }
            if (walkPointSet)
            {
                agent.isStopped = false;
                agent.SetDestination(walkPoint);
            }
        }

        // check if we have reached the walkPoint
        waitTime = Random.Range(1f, 4f);
        Vector3 distToWalkPoint = transform.position - walkPoint;
        if (distToWalkPoint.magnitude < 10f)
        {
            paused = true;
            walkPointSet = false;
            Invoke(nameof(ResetPause), waitTime);
        }
    }

    private void ResetPause()
    {
        paused = false;
    }

    /// <summary>
    /// Searches for a random point for the Spider to walk to within walkPointRange and goes there if it is a valid point.
    /// </summary>
    private void SearchWalkPoint()
    {
        walkPointSet = true;
        // create the point to go to
        float randZ;
        float randX;
        int randBool = Random.Range(0, 2);
        if (randBool == 0)
        {
            randZ = Random.Range(-walkPointRange, -5);
            randX = Random.Range(-walkPointRange, -5);
        }
        else
        {
            randZ = Random.Range(5, walkPointRange);
            randX = Random.Range(5, walkPointRange);
        }
        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        // check if the point to walk to is on the ground
        Ray ray1 = new Ray(walkPoint, -transform.up);
        Ray ray2 = new Ray(walkPoint, transform.up);
        if (Physics.Raycast(ray1, out hit, 300f, whatIsGround))
            walkPoint = hit.point;
        else if (Physics.Raycast(ray2, out hit, 300f, whatIsGround))
            walkPoint = hit.point;
        else
            walkPoint.y = transform.position.y + 10;
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
    /// When the spider's attack colliders collide with something, makes that thing take damage if it can.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.transform.root.gameObject.GetComponent<IDamageable>();
        if (damageable != null && !hitEnemy)
        {
            damageable.TakeDamage(spiderDamage);
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
    /// Handles the case when a spider would get launched really fast by the car wheels or another force. Makes the spider take damage and sets velocity to 0.
    /// </summary>
    public void HandleWheelCollision()
    {
        if (rb.velocity.magnitude > 200)
        {
            rb.velocity = Vector3.zero;
            TakeDamage(20);
        }
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the Spider
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawCube(gameObject.transform.position + centerPoint, new Vector3(2, 2, 2));
        Gizmos.DrawCube(walkPoint, new Vector3(5, 5, 5));
    }
}
