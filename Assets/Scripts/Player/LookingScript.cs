using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingScript : MonoBehaviour
{
    GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("PlayerCamera");
    }

    // Update is called once per frame
    void Update()
    {
        //if in the car, parent the target to the Main camera with an offset
        //if (in car)
        //transform.SetParent(cam.transform);

        //else, unparent the target

    }
}
