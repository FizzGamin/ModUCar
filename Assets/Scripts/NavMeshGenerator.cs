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
        Invoke(nameof(CalculateNavMesh), 2f);
    }

    private void CalculateNavMesh()
    {
        surface.BuildNavMesh();
    }
    // Update is called once per frame
    int count = 0;
    void Update()
    {
        count++;
        if (count > 300)
        {
            count = 0;
            Debug.Log("Creating coroutine...");
            IEnumerator coroutine = UpdateSurface();
            Debug.Log("starting coroutine...");
            StartCoroutine(coroutine);
            Debug.Log("coroutine finished.");
        }
    }

    private IEnumerator UpdateSurface()
    {
        return (IEnumerator)surface.UpdateNavMesh(surface.navMeshData);
    }
}
