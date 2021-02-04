using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public int count = 20;
    public int radius = 10;
    public float rotSpeed = 1;
    private float rot = 0;
    private List<GameObject> spheres = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            spheres.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spheres.Count; i++)
        {
            float offset = (360 / count * i) + rot;
            Transform t = spheres[i].transform;
            t.position = transform.position + new Vector3(0,(float)Math.Cos(rad(offset)) * radius, (float)Math.Sin(rad(offset)) * radius);
        }
        rot += rotSpeed;
    }

    private float rad(float degrees)
    {
        return (float)(degrees * Math.PI) / 180;
    }
}
