using UnityEngine;

public class EvilBlock : MonoBehaviour, IDamageable
{
    private float health = 100f;
    private bool died = false;
    public void TakeDamage(float damage)
    {
        if (health < 0) return;

        health -= damage;
        if (health < 0)
        {
            Debug.Log("Evil block died");
            GameObject.Destroy(gameObject, .5f);
            died = true;
        }
    }
    public void OnDeath()
    {

    }

    void OnDestroy()
    {
        if (died) {
            //Drop loot
            GameObject dropped = GameManager.GetLootService().GetItem(ItemQuality.B).CreateItem();
            dropped.transform.position = gameObject.transform.position;
        }
    }
}
