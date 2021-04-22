using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : ToggleableUI
{
    private const string STORAGE_UI_RESOURCE = "UI/StorageUI";
    private const string STORAGE_SLOT_RESOURCE = "UI/StorageSlot";
    private const int WIDTH_INCREMENT = 200;

    [SerializeField]
    protected GameObject mainPanel;
    [SerializeField]
    protected GameObject gridPanel;

    private List<ItemSlot> slots = new List<ItemSlot>();

    void Start()
    {

    }

    void Update()
    {
        CloseOnEscape();
        CloseOnF();
    }

    public static StorageUI CreateStorageUI(StorageModule storageModule)
    {
        GameObject storageUIPrefab = Resources.Load<GameObject>(STORAGE_UI_RESOURCE);
        GameObject newUI = Instantiate<GameObject>(storageUIPrefab);
        newUI.transform.SetParent(UIManager.GetUIBase().transform);
        return newUI.GetComponent<StorageUI>().Initialize(storageModule);
    }

    public StorageUI Initialize(StorageModule storageModule)
    {
        IItem[] items = storageModule.GetItems();
        int itemCount = items.Length;

        if (itemCount < 1 || itemCount > 8) throw new ArgumentException("Can only initialize storage UIs to have between 1 and 8 slots");
        ModifyWidth(mainPanel, itemCount);
        ModifyWidth(gridPanel, itemCount);

        GameObject slotPrefab = Resources.Load<GameObject>(STORAGE_SLOT_RESOURCE);
        for (int i = 0; i < itemCount; i++)
        {
            AddSlot(slotPrefab, storageModule, items[i], i);
        }

        gameObject.SetActive(false);

        return this;
    }

    private void AddSlot(GameObject slotPrefab, StorageModule storageModule, IItem item, int i)
    {
        GameObject newSlot = Instantiate<GameObject>(slotPrefab);
        Vector3 origScale = newSlot.transform.localScale;
        newSlot.transform.SetParent(gridPanel.transform);
        newSlot.transform.localScale = origScale;
        StorageSlot slotComponent = newSlot.GetComponent<StorageSlot>();
        slotComponent.SetConnectedStorageSlot(storageModule, i);
        slotComponent.SetItem(item);
        slots.Add(slotComponent);
    }

    private void ModifyWidth(GameObject panel, int numModules)
    {
        panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WIDTH_INCREMENT * (int)Math.Ceiling((double)numModules / 2));
    }
}
