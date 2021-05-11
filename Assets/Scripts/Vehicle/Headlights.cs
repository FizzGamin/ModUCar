using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headlights : MonoBehaviour
{
    private Transform sunTransform;

    // Start is called before the first frame update
    void Start()
    {
        sunTransform = GameObject.Find("Sun").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (sunTransform.position.y > 0)
            this.GetComponent<Light>().enabled = false;
        else
            this.GetComponent<Light>().enabled = true;
    }
}
