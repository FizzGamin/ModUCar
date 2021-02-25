using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMove : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(new Vector3(Random.Range(25f, 100f), 0, Random.Range(25f, 100f)));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
