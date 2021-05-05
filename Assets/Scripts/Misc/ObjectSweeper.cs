using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSweeper : MonoBehaviour
{
    public int iteration;
    private int count;
    public int yThreshold;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        count++;
        if (count > iteration)
        {
            count = 0;

            GameObject[] objects = (GameObject[]) FindObjectsOfType(typeof(GameObject));
            foreach (GameObject gameObj in objects)
            {
                if (gameObj.transform.position.y < yThreshold)
                    Destroy(gameObj);
            }
        }
    }
}
