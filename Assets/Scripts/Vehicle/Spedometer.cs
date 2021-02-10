using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spedometer : MonoBehaviour
{
    public Rigidbody car;
    public RectTransform spedTicker;

    // Update is called once per frame
    void Update()
    {
        spedTicker.transform.eulerAngles = new Vector3(0, 0, -car.velocity.magnitude + 44);
    }
}
