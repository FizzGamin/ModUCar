using UnityEngine;
using UnityEngine.AI;

public abstract class IEnemy : MonoBehaviour, IDamageable
{
    public abstract void TakeDamage(float damage);
}
