using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    static int distance = 20;
    static int height = 20;
    static GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player");
        transform.Rotate(new Vector3(20, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + height, target.transform.position.z - distance);
    }
}
