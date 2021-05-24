using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public LayerMask myLayerMask;
    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(gameObject.transform.position, Vector3.down);
        parent = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(ray, out hit, 20, myLayerMask))
        {
            float y = hit.point.y - parent.transform.position.y;
            gameObject.transform.localPosition = new Vector3(gameObject.transform.position.x, y + 5f, gameObject.transform.position.z); //3.88
                
        }
    }
}
