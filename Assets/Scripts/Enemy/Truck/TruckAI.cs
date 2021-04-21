using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TruckAI : IEnemy
{
    public List<WheelCollider> steeringWheelColliders;
    public List<WheelCollider> powerWheelColliders;
    public float vehiclePower = 10000f;
    public float vehicleSteeringAngle = 50f;
    float driveInfluence;
    float steeringInfluence;

    public LayerMask whatIsGround, whatIsPlayer;
    Transform player;
    GameObject playerObj;
    private static int MAXTURNANGLE = 50;

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

    GameObject healthBar;
    public override void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.GetComponent<Image>().fillAmount -= (0.01f * damage);
        if (health <= 0)
            Invoke(nameof(OnDeath), .5f);
    }

    public override void OnDeath()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

        //Reset each wheels break forces and torques back to 0
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.brakeTorque = 0;
            wc.motorTorque = 0;
        }

        //reset the steering as well (and brakes)
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.brakeTorque = 0;
            wc.steerAngle = Mathf.Lerp(wc.steerAngle, 0, 5f * Time.deltaTime);
        }

        if (!playerInSightRange)
            Patrol();
        if (playerInSightRange)
            AttackPlayer();
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    /// <summary>
    /// Handles the patrol mode for the truck. Finds a walk point and drives toward it.
    /// </summary>
    private void Patrol()
    {
        if (!walkPointSet)
        {
            Invoke(nameof(SearchWalkPoint), 1f);
        }
        if (walkPointSet)
        {
            Vector3 walkpointPosVector = (new Vector3(walkPoint.x, 0, walkPoint.z)) - transform.position;
            Vector3 truckDirVector = (new Vector3(transform.forward.x, 0, transform.forward.z)); //get the forward vector but in global terms

            float angle = Vector3.Angle(truckDirVector, walkpointPosVector);
            var cross = Vector3.Cross(walkpointPosVector, truckDirVector);
            if (cross.y > 0) angle = -angle;
            float turnDir = 1;
            float power = vehiclePower;
            if (angle > MAXTURNANGLE || angle < -MAXTURNANGLE)
            {
                if (angle > 0)
                {
                    angle = MAXTURNANGLE;
                    turnDir = 1;
                }
                else
                {
                    angle = -MAXTURNANGLE;
                    turnDir = -1;
                }
                power = vehiclePower / 2;
            }

            Drive(angle * turnDir, power);
        }

        // check if we have reached the walkPoint
        Vector3 distToWalkPoint = transform.position - walkPoint;
        if (distToWalkPoint.magnitude < 20f)
            walkPointSet = false;
    }

    /// <summary>
    /// Searches for a random point for the truck to drive to within walkPointRange and goes there if it is a valid point.
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
    /// Handles the attack mode for the truck. Calculates the angle to the player and a power based on the angle. Then calls the Drive function based on those values.
    /// </summary>
    private void AttackPlayer()
    {

        Vector3 playerPosVector = (new Vector3(player.position.x, 0, player.position.z)) - transform.position;
        Vector3 truckDirVector = (new Vector3(transform.forward.x, 0, transform.forward.z)); //get the forward vector but in global terms
        
        float angle = Vector3.Angle(truckDirVector, playerPosVector);
        var cross = Vector3.Cross(playerPosVector, truckDirVector);
        if (cross.y > 0) angle = -angle;
        
        //float turnDir = 1;
        float power = vehiclePower;
        if (this.GetComponent<Rigidbody>().velocity.magnitude > 40 && (angle > MAXTURNANGLE || angle < -MAXTURNANGLE))
            BrakeToStop();
        else if (angle > MAXTURNANGLE || angle < -MAXTURNANGLE)
        {
            if (angle > 0)
            {
                angle = MAXTURNANGLE;
                //turnDir = 1;
            }
            else
            {
                angle = -MAXTURNANGLE;
                //turnDir = -1;
            }
            power = vehiclePower / 2;
        }
        Drive(angle, power);
    }

    /// <summary>
    /// When the truck's attack colliders collide with something, checks if it is something that can take damage and call the appropriate takeDamage function.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        //truck can damage either the spider or the player
        if (other.gameObject.tag == "Player" && !hitPlayer)
        {
            playerObj.GetComponent<PlayerController>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
        if (other.gameObject.tag == "Enemy_Spider")
        {
            other.gameObject.GetComponent<SpiderAI>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }
    private void ResetAttack()
    {
        hitPlayer = false;
    }

    /// <summary>
    /// Applies torque on the wheels to move the truck forward based on the power input, and turns the wheels based on the direction dir.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="power"></param>
    private void Drive(float dir, float power)
    {
        vehicleSteeringAngle = dir;
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.steerAngle = Mathf.Lerp(wc.steerAngle, vehicleSteeringAngle, 5f * Time.deltaTime);
        }
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.motorTorque = power;
        }
    }

    /// <summary>
    /// Increases the brake torque to bring the truck to a stop or at least slow it down.
    /// </summary>
    private void BrakeToStop()
    {
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.brakeTorque = vehiclePower * 3;
        }
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.brakeTorque = vehiclePower * 3;
        }
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the truck
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


}
