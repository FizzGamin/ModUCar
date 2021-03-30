using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKArms : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform armRoot = default;
    [SerializeField] Transform body = default;
    [SerializeField] PlayerIKArms otherArm = default;
    float speed = 4f;
    //NOTE: stepDistance must be >= stepLength
    float stepDistance = 1f;
    float stepLength = 1f;
    float stepHeight = 1.5f;
    //if the foot for some reason goes into the ground, change this offset value
    Vector3 armOffset = new Vector3(0, 45, 180);
    float armSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // >= 1 means arm is not moving, otherwise it is
    float timeSinceLastMove;
    bool armReset = true;
    bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = armRoot.position;

        timeSinceLastMove = 0;
        armSpacing = transform.localPosition.y;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = armOffset;
        transform.eulerAngles = armOffset;
        lerp = 1;
        transform.position = currentPosition;
        transform.eulerAngles = armOffset;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (active == true)
        {
            transform.position = currentPosition;
            transform.eulerAngles = armOffset;

            // create the raycast Ray
            Ray ray = new Ray(armRoot.position + (armRoot.right * armSpacing), Vector3.down);

            // this is where the actual raycast is made, raycast information stored in info. Executes if statement on a successful raycast
            if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
            {
                timeSinceLastMove = timeSinceLastMove + Time.deltaTime;
                // checks if distance is big enough to move, the other arm is not moving, and this foot is not moving 
                if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherArm.IsMoving() && lerp >= 1)
                {
                    timeSinceLastMove = 0;
                    lerp = 0;
                    // checks if the arm should be moving forwards or backwards
                    int direction = armRoot.InverseTransformPoint(info.point).z > armRoot.InverseTransformPoint(newPosition).z ? 1 : -1;
                    newPosition = info.point + (body.up * stepLength * direction); // + (armRoot.forward * stepLength * direction) + footOffset;
                    newNormal = info.normal;
                    armReset = false;
                }
                // OR enough time has passed and the foot location is different than the racast location
                else if (timeSinceLastMove > 0.5 && Vector3.Distance(newPosition, info.point) > 0 && armReset == false)
                {
                    timeSinceLastMove = 0;
                    lerp = 0;
                    newPosition = info.point;
                    newNormal = info.normal;
                    armReset = true;
                }
            }

            // if we are within our movement cycle
            if (lerp < 1 && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // interpolates between 2 points, lerp is the percentage of completion.
                Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
                // make an arc in the movement
                tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

                currentPosition = tempPosition;
                currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
                lerp += Time.deltaTime * speed;
            }
            else
            {
                oldPosition = newPosition;
                oldNormal = newNormal;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }

    public void Enable()
    {
        active = true;
    }

    public void Disable()
    {
        active = false;
    }
}
