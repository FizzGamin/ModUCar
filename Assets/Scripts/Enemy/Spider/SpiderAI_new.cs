using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpiderAI_new : IEnemy 
{
    public LayerMask whatIsGround, whatIsPlayer;
    public List<SpiderIKSolver> legs1;
    public List<SpiderIKlegsSecond> legs2; 
    public List<SpiderIKlegsBack> legs3;

    public float health;
    private float maxHealth;
    public float spiderDamage;

    Transform player;
    GameObject playerObj;

    public LayerMask myLayerMask;

    GameObject centerPoint;

    // PATROL
    Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    RaycastHit hit;
    bool paused;
    float waitTime;
    Rigidbody rb;
    float speed;
    int patrolSpeed;

    // ATTACKING
    public float timeBetweenAttacks;
    bool hitPlayer;
    bool hitEnemy;
    int randDifficulty;
    Ray forceRay;

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
        GameManager.GetLootService().DeathLoot(ItemQuality.C, .2, transform.position);
        Destroy(gameObject);
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerObj = GameObject.FindGameObjectWithTag("Player");
        healthBar = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).gameObject;
        maxHealth = health;
        paused = false;
        rb = gameObject.GetComponent<Rigidbody>();
        randDifficulty = Random.Range(25, 36);
        patrolSpeed = 10;
        centerPoint = gameObject.transform.GetChild(2).gameObject;
        forceRay = new Ray(centerPoint.transform.position, new Vector3(gameObject.transform.forward.x, centerPoint.transform.forward.y, gameObject.transform.forward.z));
    }

    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();

        HandleBouncing();
        HandleWheelCollision();
    }

    /// <summary>
    /// Handles the patrolling mode for the Spider. Changes speed of Spider and procedural legs and goes to walk point.
    /// </summary>
    private void Patrol()
    {
        // Update to patrol speeds
        speed = 5f;
        foreach (SpiderIKSolver leg in legs1)
            leg.SpiderPatrolSpeed();
        foreach (SpiderIKlegsSecond leg in legs2)
            leg.SpiderPatrolSpeed();
        foreach (SpiderIKlegsBack leg in legs3)
            leg.SpiderPatrolSpeed();

        if (paused)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Vector3 look = new Vector3(walkPoint.x, gameObject.transform.position.y, walkPoint.z);
            gameObject.transform.LookAt(look);
        }
        if (!paused)
        {
            if (!walkPointSet || Vector3.Distance(walkPoint, gameObject.transform.position) > sightRange)
            {
                Invoke(nameof(SearchWalkPoint), 0);
            }
            if (walkPointSet)
            {
                /*Vector3 look = new Vector3(walkPoint.x, gameObject.transform.position.y, walkPoint.z);
                gameObject.transform.LookAt(look);
                Ray ray = new Ray(centerPoint.transform.position, new Vector3(gameObject.transform.forward.x, centerPoint.transform.forward.y, gameObject.transform.forward.z));
                if (rb.velocity.magnitude < patrolSpeed)
                    rb.AddForce(ray.direction * rb.mass * speed, ForceMode.Impulse);*/
                float yLook = gameObject.transform.position.y;
                Ray ray = new Ray(centerPoint.transform.position, new Vector3(gameObject.transform.forward.x, centerPoint.transform.forward.y, gameObject.transform.forward.z));

                //change the angle that the spider applies force in.
                if (Physics.Raycast(ray, out hit, 10, myLayerMask))
                {
                    speed = 6;
                    yLook = walkPoint.y;
                    Ray walkPointDir = new Ray(gameObject.transform.position, (gameObject.transform.position - (walkPoint + new Vector3(0, 5, 0))).normalized);
                    ray = new Ray(gameObject.transform.position, -walkPointDir.direction);
                    Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 10f);
                }

                Vector3 targetPosition = new Vector3(walkPoint.x, yLook, walkPoint.z);
                transform.LookAt(targetPosition);

                //change sideways velocity so spider doesn't slide
                Vector3 tempVector = gameObject.GetComponent<Rigidbody>().velocity;
                tempVector = gameObject.transform.InverseTransformVector(tempVector);
                if (tempVector.x > 1 || tempVector.x < -1)
                {
                    tempVector.x = 0f;
                    tempVector = gameObject.transform.TransformVector(tempVector);
                    gameObject.GetComponent<Rigidbody>().velocity = tempVector;
                }

                //force for going straight
                    if (rb.velocity.magnitude < patrolSpeed)
                    rb.AddForce(ray.direction * rb.mass, ForceMode.Impulse);
                //force for going up hills
                if (speed > 5 && rb.velocity.magnitude < 100)
                    rb.AddForce(ray.direction * rb.mass * speed, ForceMode.Impulse);
            }
        }

        // check if we have reached the walkPoint
        waitTime = Random.Range(1f, 6f);
        Vector3 a = new Vector3(walkPoint.x, gameObject.transform.position.y, walkPoint.z);
        Vector3 distToWalkPoint = transform.position - a;
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
        walkPoint = new Vector3(transform.position.x + randX, 100, transform.position.z + randZ);

        // check if the point to walk to is on the ground
        Ray ray1 = new Ray(walkPoint, -transform.up);
        Ray ray2 = new Ray(walkPoint, transform.up);
        if (Physics.Raycast(ray1, out hit, 300f, whatIsGround))
            walkPoint = hit.point;
        else if (Physics.Raycast(ray2, out hit, 300f, whatIsGround))
            walkPoint = hit.point;
        else
            walkPoint.y = transform.position.y + 10;

        patrolSpeed = Random.Range(3, 8);
    }

    /// <summary>
    /// Handles the attacking mode for the Spider, changes the speed of the spider and legs, and looks toward the player and goes toward it.
    /// </summary>
    private void AttackPlayer()
    {
        // Update to attack speeds
        speed = 5;
        foreach (SpiderIKSolver leg in legs1)
            leg.SpiderChaseSpeed();
        foreach (SpiderIKlegsSecond leg in legs2)
            leg.SpiderChaseSpeed();
        foreach (SpiderIKlegsBack leg in legs3)
            leg.SpiderChaseSpeed();

        float yLook = gameObject.transform.position.y;
        Ray ray = new Ray(centerPoint.transform.position, new Vector3(gameObject.transform.forward.x, centerPoint.transform.forward.y, gameObject.transform.forward.z));

        //change the angle that the spider applies force in.
        if (Physics.Raycast(ray, out hit, 10, myLayerMask))
        {
            speed = 6;
            yLook = player.position.y;
            Ray playerDir = new Ray(gameObject.transform.position, (gameObject.transform.position - (player.position + new Vector3(0, 5, 0))).normalized);
            ray = new Ray(gameObject.transform.position, -playerDir.direction);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.red, 10f);
        }

        Vector3 targetPosition = new Vector3(player.position.x, yLook, player.position.z);
        transform.LookAt(targetPosition);

        //change sideways velocity so spider doesn't slide
        Vector3 tempVector = gameObject.GetComponent<Rigidbody>().velocity;
        tempVector = gameObject.transform.InverseTransformVector(tempVector);
        if (tempVector.x > 5 || tempVector.x < -5)
        {
            tempVector.x = 0f;
            tempVector = gameObject.transform.TransformVector(tempVector);
            gameObject.GetComponent<Rigidbody>().velocity = tempVector;
        }

        //force for going straight
        if (rb.velocity.magnitude < randDifficulty)
            rb.AddForce(ray.direction * rb.mass * speed, ForceMode.Impulse);
        //force for going up hills
        if (speed > 5 && rb.velocity.magnitude < 61)
            rb.AddForce(ray.direction * rb.mass * speed, ForceMode.Impulse);
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
        }
    }

    public void HandleBouncing()
    {
        Ray ray = new Ray(centerPoint.transform.position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 100, myLayerMask))
        {
            if (hit.point.magnitude > 1f || hit.point.magnitude < 0f) //changed from 0 to 0.001
                gameObject.transform.position = hit.point + new Vector3(0, 0.1f, 0);
        }
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the Spider
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.DrawCube(walkPoint, new Vector3(5, 5, 5));
    }
}
