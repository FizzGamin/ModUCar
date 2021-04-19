using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sun : MonoBehaviour
{
    public Material day;
    public Material night;
    private Transform sunTransform;
    private float rotationSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        sunTransform = GameObject.Find("Sun").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, rotationSpeed * Time.deltaTime);
        transform.LookAt(Vector3.zero);

        if (sunTransform.position.y > 0)
        {
            RenderSettings.skybox = day;
            rotationSpeed = 5f / 3;
            sunTransform.gameObject.GetComponent<Light>().intensity = 0.5f;
        }
        else
        {
            RenderSettings.skybox = night;
            rotationSpeed = 15f / 3;
            sunTransform.gameObject.GetComponent<Light>().intensity = 0f;
        }
    }
}
