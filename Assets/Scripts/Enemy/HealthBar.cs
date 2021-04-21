using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        enemy = this.transform.parent.parent.parent.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
