using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public LayerMask myLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(gameObject.transform.position, Vector3.down);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(ray, out hit, 20, myLayerMask))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, hit.point.y + 3.88f, gameObject.transform.position.z);
                
        }
    }
}
