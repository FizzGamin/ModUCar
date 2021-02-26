using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using System.Linq;

public class LootService : UnitySingleton<LootService>
{
    public GameObject itemDirectory;

    private List<IItem> items;
    private List<int[]> itemSelectionArrays;
    private System.Random random; //This should eventually be seeded with the game seed

    private const string ITEM_PREFAB_PATH = "Assets/JacobTestZone/Prefabs/Items/";
    void Start()
    {
        items = new List<IItem>();
        random = new System.Random();

        GameManager.SetLootService(this);

        UnityEngine.Object[] itemObjs = Resources.LoadAll("Items");
        foreach(GameObject item in itemObjs)
        {
            items.Add(item.GetComponent<IItem>());
        }

        CreateWeightArrays();
    }

    private void CreateWeightArrays()
    {
        itemSelectionArrays = new List<int[]>();
        foreach (ItemQuality quality in Enum.GetValues(typeof(ItemQuality)))
        {
            itemSelectionArrays.Add(new int[items.Count + 1]);
            itemSelectionArrays[(int)quality][0] = 0;
            int cur = 0;
            for (int i = 0; i < items.Count; i++)
            {
                //We can at most have 4 "entries" into the item table
                int entries = 4 - Math.Abs(items[i].GetQuality() - quality);
                if (entries > 1)
                {
                    cur += items[i].GetWeight() * entries;
                }
                itemSelectionArrays[(int)quality][i + 1] = cur;
            }
        }
    }

    public IItem GetItem(ItemQuality quality)
    {
        int[] qualityArray = itemSelectionArrays[(int)quality];
        int selected = random.Next(0, qualityArray[qualityArray.Length - 1]);
        
        for (int i = 1; i < qualityArray.Length; i++)
        {
            if (selected < qualityArray[i])
            {
                return items[i - 1];
            }
        }

        throw new InvalidDataException("Attempted to generate an item but was out of bounds of the array: " + selected);
    }

    public void TestItemGeneration(ItemQuality quality, int count)
    {
        Dictionary<IItem, int> data = new Dictionary<IItem, int>();
        foreach (IItem item in items)
        {
            data.Add(item, 0);
        }

        for (int i = 0; i < count; i++)
        {
            data[GetItem(quality)] += 1;
        }

        foreach (IItem item in items)
        {
            int entries = 4 - Math.Abs(item.GetQuality() - quality);
            if (entries < 0) entries = 0;
            Debug.Log(item.GetName() + " - Expected: " + (count/itemSelectionArrays[(int)quality][itemSelectionArrays[(int)quality].Length-1]) * (entries * item.GetWeight()) + " - Actual: " + data[item]);
        }
    }
}
