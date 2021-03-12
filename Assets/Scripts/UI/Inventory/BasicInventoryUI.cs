using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BasicInventoryUI : MonoBehaviour, InventoryUI
{
    private const string MISSING_SPRITE_NAME = "MissingSprite";

    private List<GameObject> slots;
    private Sprite missingSprite;
    private int currentSelected = 0;
    private const int INVENTORY_SIZE = 3;
    void Start()
    {
        UIManager.SetInventoryUI(this);

        List<GameObject> temp = new List<GameObject>();
        foreach (Transform child in transform)
        {
            temp.Add(child.gameObject);
        }
        slots = temp.OrderBy(o => o.transform.position.x).ToList();
        Select(0);

        missingSprite = GetSpriteByName(MISSING_SPRITE_NAME);
    }

    public void UpdateInventory(IItem[] items, int selected)
    {
        if (items.Length != slots.Count)
        {
            throw new ArgumentException("Basic inventory UI holds " + slots.Count + " objects");
        }

        for (int i = 0; i < slots.Count; i++)
        {
            Image slotImage = slots[i].GetComponentsInChildren<Image>(true).Where((img) => img.gameObject != slots[i]).First();
            if (items[i] != null)
            {
                Sprite itemSprite = GetSpriteByName(items[i].GetSpriteName());
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

        if (currentSelected != selected)
        {
            Deselect(currentSelected);
            Select(selected);
            currentSelected = selected;
        }
    }

    private void Select(int i)
    {
        slots[i].transform.localScale = new Vector3(.95f, .95f, .95f);
    }

    private void Deselect(int i)
    {
        slots[i].transform.localScale = new Vector3(.9f, .9f, .9f);
    }

    private Sprite GetSpriteByName(string name)
    {
        return GameManager.GetSpriteService().GetSprite(name);
    }

    public int GetSize()
    {
        return 3;
    }
}
