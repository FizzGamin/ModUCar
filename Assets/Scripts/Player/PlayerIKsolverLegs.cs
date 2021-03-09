using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIKsolverLegs: MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform legRoot = default;
    [SerializeField] Transform body = default;
    [SerializeField] PlayerIKsolverLegs otherFoot = default;
    float speed = 5f;
    //NOTE: stepDistance must be >= stepLength
    float stepDistance = 1.7f;
    float stepLength = 1.7f;
    float stepHeight = 1.5f;
    //if the foot for some reason goes into the ground, change this offset value
    Vector3 footOffset = new Vector3(-250f, 90, 0);
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // >= 1 means leg is not moving, otherwise it is
    float timeSinceLastMove = 0;
    bool legReset = true;
   
    // Start is called before the first frame update
    void Start()
    {
        footSpacing = -transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = footOffset;
        lerp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //need to change direction foot is pointing to the same direction the body is pointing
        transform.position = currentPosition;
        transform.eulerAngles = currentNormal;
        transform.eulerAngles = new Vector3(currentNormal.x, body.eulerAngles.y, currentNormal.z);

        // create the raycast Ray
        Ray ray = new Ray(legRoot.position + (legRoot.right * footSpacing), Vector3.down);

        // this is where the actual raycast is made, raycast information stored in info. Executes if statement on a successful raycast
        if (Physics.Raycast(ray, out RaycastHit info, 10, terrainLayer.value))
        {
            timeSinceLastMove = timeSinceLastMove + Time.deltaTime;
            // checks if distance is big enough to move, the other leg is not moving, and this foot is not moving 
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                timeSinceLastMove = 0;
                lerp = 0;
                // checks if the leg should be moving forwards or backwards
                int direction = legRoot.InverseTransformPoint(info.point).z > legRoot.InverseTransformPoint(newPosition).z ? 1 : -1;
                /*int swayDir = 0;
                if (legRoot.InverseTransformPoint(info.point).x > legRoot.InverseTransformPoint(newPosition).x) swayDir = 1;
                if (legRoot.InverseTransformPoint(info.point).x < legRoot.InverseTransformPoint(newPosition).x) swayDir = -1;*/
                int swayDir = 0;
                if (Input.GetKey(KeyCode.A))
                {
                    swayDir = -1;
                    direction = 0;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    swayDir = 1;
                    direction = 0;
                }
                newPosition = info.point + (body.up * stepLength * direction) + (body.right * stepLength * swayDir); // + (legRoot.forward * stepLength * direction) + footOffset;
                newNormal = info.normal + footOffset;
                legReset = false;
            }
            // OR enough time has passed and the foot location is different than the racast location
            else if (timeSinceLastMove > 0.5 && Vector3.Distance(newPosition, info.point) > 0 && legReset == false)
            {
                timeSinceLastMove = 0;
                lerp = 0;
                newPosition = info.point;
                newNormal = info.normal + footOffset;
                legReset = true;
            }
        }

        // if we are within our movement cycle
        if (lerp < 1)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}
