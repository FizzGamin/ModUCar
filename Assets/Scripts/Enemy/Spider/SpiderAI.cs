using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderAI : MonoBehaviour
{
    NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating(nameof(GoToLocation), 3, 5);
    }

    private void GoToLocation()
    {
        agent.SetDestination(new Vector3(Random.Range(25f, 100f), 0, Random.Range(25f, 100f)));
        transform.eulerAngles = new Vector3(0, 180, 0);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
