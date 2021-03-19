using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BasicInventoryUI : MonoBehaviour, InventoryUI
{
    private List<ItemSlot> slots;
    private int currentlySelected = 0;
    void Start()
    {
        UIManager.SetInventoryUI(this);

        List<GameObject> temp = new List<GameObject>();
        foreach (Transform child in transform)
        {
            temp.Add(child.gameObject);
        }
        slots = temp.OrderBy(o => o.transform.position.x).ToList().ConvertAll<ItemSlot>((obj) =>
        {
            return obj.GetComponent<ItemSlot>();
        });

        slots[0].Select();
    }

    public void SetItem(int slot, IItem item)
    {
        slots[slot].SetItem(item);
    }

    public IItem GetItem(int slot)
    {
        return slots[slot].GetItem();
    }

    public void Select(int slot)
    {
        if (currentlySelected != slot)
        {
            slots[currentlySelected].Deselect();
            slots[slot].Select();
            currentlySelected = slot;
        }
    }

    public int GetSize()
    {
        return 3;
    }

    public IItem GetSelectedItem()
    {
        return slots[currentlySelected].GetItem();
    }

    public int GetSelectedIndex()
    {
        return currentlySelected;
    }
}
