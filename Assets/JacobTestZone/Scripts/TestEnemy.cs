using UnityEngine.AI;
using UnityEngine;

public class TestEnemy : IEnemy
{
    private const float MAX_HEALTH = 100f;

    private float health;
    private NavMeshAgent navAgent;

    void Start()
    {
        health = MAX_HEALTH;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        navAgent.SetDestination(GameManager.GetPlayer().GetGameObject().transform.position);
    }

    public override void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Took damage");
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
