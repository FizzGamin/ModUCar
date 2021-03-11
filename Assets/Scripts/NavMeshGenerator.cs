using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(CalculateNavMesh), 2);
    }

    private void CalculateNavMesh()
    {
        surface.BuildNavMesh();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
