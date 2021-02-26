using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : UnitySingleton<UIManager>
{
    private InventoryUI inventoryUI;
    private void Start()
    {
        GameManager.SetUIManager(this);
    }

    public InventoryUI GetInventoryUI()
    {
        return inventoryUI;
    }

    public void SetInventoryUI(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
    }
}
