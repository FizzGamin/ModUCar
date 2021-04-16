using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootGeneratorSpot : MonoBehaviour
{
    public ItemQuality lootQuality = ItemQuality.F;
    // Start is called before the first frame update
    void Start()
    {
        if (LootService.instance)
        {
            IItem item = LootService.instance.GetItem(lootQuality);
            GameObject obj = item.CreateItem();
            obj.transform.position = transform.position;
        }       
        Destroy(gameObject);
    }
}
