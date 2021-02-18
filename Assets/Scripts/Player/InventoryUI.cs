using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InventoryUI
{
    void UpdateInventory(IItem[] items, int selected);
}
