using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderIKSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform legRoot = default;
    [SerializeField] Transform body = default;
    [SerializeField] SpiderIKlegsSecond otherFoot = default;

    float speed = 10f;
    float stepDistance = .2f;
    float stepLength = .2f;
    float stepHeight = .3f;

    Vector3 footOffset = default;
    float footSpacing;
    Vector3 oldPosition, currentPosition, newPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // >= 1 means leg is not moving, otherwise it is

    // Start is called before the first frame update
    void Start()
    {
        footSpacing = legRoot.localPosition.x + (System.Math.Sign(legRoot.localPosition.x) * .5f);
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = footOffset;
        lerp = 1;
        transform.eulerAngles = new Vector3(270, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentPosition;
        transform.eulerAngles = currentNormal;

        // create the raycast Ray
        Ray ray = new Ray(legRoot.position + (body.right * footSpacing), Vector3.down);    //legRoot.position + (legRoot.right * footSpacing), Vector3.down);

        // this is where the actual raycast is made, raycast information stored in info. Executes if statement on a successful raycast
        if (Physics.Raycast(ray, out RaycastHit info, 5, terrainLayer.value))
        {
            // checks if distance is big enough to move, the other leg is not moving, and this foot is not moving
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                // checks if the leg should be moving forwards or backwards
                int direction = legRoot.InverseTransformPoint(info.point).z > legRoot.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = info.point + (body.up * stepLength * direction);
                newNormal = info.normal + footOffset;
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

    public void SpiderChaseSpeed()
    {
        speed = 30f;
        stepDistance = 2f;
        stepLength = 2f;
        stepHeight = .3f;
    }

    public void SpiderPatrolSpeed()
    {
        speed = 10f;
        stepDistance = .2f;
        stepLength = .2f;
        stepHeight = .3f;
    }
}
