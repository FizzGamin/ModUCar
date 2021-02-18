using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BasicInventoryUI : MonoBehaviour, InventoryUI
{
    private List<GameObject> slots;
    private Sprite missingSprite;
    void Start()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (Transform child in transform)
        {
            temp.Add(child.gameObject);
        }
        slots = temp.OrderBy(o => o.transform.position.x).ToList();

        missingSprite = Resources.Load("Sprites/MissingSprite", typeof(Sprite)) as Sprite;
    }

    public void UpdateInventory(IItem[] items, int selected)
    {
        if (items.Length != slots.Count)
        {
            throw new ArgumentException("Basic inventory UI holds " + slots.Count + " objects");
        }

        for (int i = 0; i < slots.Count; i++)
        {
            Image slotImage = slots[i].GetComponentInChildren<Image>();
            if (items[i] != null)
            {
                Sprite itemSprite = items[i].GetSprite();
                if (itemSprite != null)
                {
                    slotImage.sprite = itemSprite;
                }
                else
                {
                    slotImage.sprite = missingSprite;
                }
                slotImage.gameObject.SetActive(true);
            } else
            {
                slotImage.sprite = null;
                slotImage.gameObject.SetActive(false);
            }
        }
    }
}
