using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        player = GameObject.Find("Player").transform;
        playerObj = GameObject.Find("Player");
        //driveInfluence = Input.GetAxis("Vertical");
        //steeringInfluence = Input.GetAxis("Horizontal");
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
            Vector3 walkpointPosVector = (new Vector3(walkPoint.x, 0, walkPoint.z)) - transform.position;
            Vector3 truckDirVector = (new Vector3(transform.forward.x, 0, transform.forward.z)); //get the forward vector but in global terms

            float angle = Vector3.SignedAngle(truckDirVector, walkpointPosVector, transform.position);
            //float angle = Vector3.Angle(truckDirVector, walkpointPosVector);
            //var cross = Vector3.Cross(walkpointPosVector, truckDirVector);
            //if (cross.y > 0) angle = -angle;
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
        Vector3 playerPosVector = (new Vector3(player.position.x, 0, player.position.z)) - transform.position;
        Vector3 truckDirVector = (new Vector3(transform.forward.x, 0, transform.forward.z)); //get the forward vector but in global terms

        //float angle = Vector3.SignedAngle(truckDirVector, playerPosVector, transform.position);
        
        float angle = Vector3.Angle(truckDirVector, playerPosVector);
        var cross = Vector3.Cross(playerPosVector, truckDirVector);
        if (cross.y > 0) angle = -angle;
        
        float turnDir = 1;
        float power = vehiclePower;
        if (this.GetComponent<Rigidbody>().velocity.magnitude > 40 && (angle > MAXTURNANGLE || angle < -MAXTURNANGLE))
            BrakeToStop();
        else if (angle > MAXTURNANGLE || angle < -MAXTURNANGLE)
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
        Drive(angle, power);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("You have been hit by " + this.name);

        //truck can damage either the spider or the player
        if (other.gameObject.name == "Player" && !hitPlayer)
        {
            playerObj.GetComponent<PlayerController>().TakeDamage(10);
            hitPlayer = true;
            Invoke(nameof(ResetAttack), 0.5f);
        }
        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.name == "Enemy_Spider")
            {
                other.gameObject.GetComponent<SpiderAI>().TakeDamage(10);
                hitPlayer = true;
                Invoke(nameof(ResetAttack), 0.5f);
            }
        }
    }
    private void ResetAttack()
    {
        hitPlayer = false;
    }

    private void Drive(float dir, float power)
    {
        vehicleSteeringAngle = dir;
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.steerAngle = Mathf.Lerp(wc.steerAngle, vehicleSteeringAngle/* * steeringInfluence*/, 5f * Time.deltaTime);
        }
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.motorTorque = /*driveInfluence * */power;
        }
    }

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

    /* Creates a gizmo showing the sight range of the truck */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }


}
