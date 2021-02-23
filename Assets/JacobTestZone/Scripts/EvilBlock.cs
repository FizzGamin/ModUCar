using UnityEngine;

public class EvilBlock : MonoBehaviour, IDamageable
{
    float health = 100f;
    public void TakeDamage(float damage)
    {
        if (health < 0) return;

        health -= damage;
        if (health < 0)
        {
            Debug.Log("Evil block died");
            GameObject.Destroy(gameObject, .5f);
            
        }
    }

    void OnDestroy()
    {
        //Drop loot
        GameObject dropped = GameManager.instance.GetLootService().GetItem(ItemQuality.D).CreateItem();
        dropped.transform.position = gameObject.transform.position;
    }
}
