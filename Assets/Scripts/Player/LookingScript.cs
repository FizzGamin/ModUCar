using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingScript : MonoBehaviour
{
    GameObject cam;
    Vector3 oldPos;

    // Start is called before the first frame update
    void Start()
    {
        oldPos = transform.localPosition;
        cam = GameObject.Find("PlayerCamera");
    }

    // Update is called once per frame
    void Update()
    {
        //if in the car, parent the target to the Main camera with an offset
        if (IsPlayerInVehicle())
        {
            transform.position = cam.transform.position + (cam.transform.forward * 100);
        }
        //else unparent the target
        else
        {
            transform.localPosition = oldPos;
        }
    }

    private bool IsPlayerInVehicle()
    {
        GameObject player = GameObject.Find("Player");
        if (player.transform.parent != null)
            return true;
        else
            return false;
    }
}
