using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUI : ToggleableUI
{
    private const string MODULE_UI_RESOURCE = "UI/ModuleUI";
    private const string MODULE_SLOT_RESOURCE = "UI/ModuleUISlot";
    private const int WIDTH_INCREMENT = 200;

    [SerializeField]
    protected GameObject mainPanel;
    [SerializeField]
    protected GameObject gridPanel;

    private List<ItemSlot> slots = new List<ItemSlot>();
    private VehicleController vehicle;

    void Start()
    {
        
    }

    void Update()
    {
        CloseOnEscape();
    }

    public static ModuleUI CreateModuleUI(VehicleController vehicle)
    {
        GameObject moduleUIPrefab = Resources.Load<GameObject>(MODULE_UI_RESOURCE);
        GameObject newUI = Instantiate<GameObject>(moduleUIPrefab);
        newUI.transform.SetParent(UIManager.GetUIBase().transform);
        return newUI.GetComponent<ModuleUI>().Initialize(vehicle);
    }

    public ModuleUI Initialize(VehicleController vehicle)
    {
        this.vehicle = vehicle;
        List<VehicleModule> modules = vehicle.GetVehicleModules();
        List<ModuleSlot> moduleSlots = vehicle.GetModuleSlots();
        int numModules = modules.Count;

        if (numModules < 1 || numModules > 8) throw new ArgumentException("Can only initialize module UIs to have between 1 and 8 slots");
        ModifyWidth(mainPanel, numModules);
        ModifyWidth(gridPanel, numModules);

        GameObject slotPrefab = Resources.Load<GameObject>(MODULE_SLOT_RESOURCE);
        for (int i = 0; i < numModules; i++)
        {
            AddSlot(slotPrefab, modules[i], moduleSlots[i]);
        }

        gameObject.SetActive(false);

        return this;
    }

    private void AddSlot(GameObject slotPrefab, VehicleModule module, ModuleSlot moduleSlot)
    {
        GameObject newSlot = Instantiate<GameObject>(slotPrefab);
        Vector3 origScale = newSlot.transform.localScale;
        newSlot.transform.SetParent(gridPanel.transform);
        newSlot.transform.localScale = origScale;
        ModuleUISlot slotComponent = newSlot.GetComponent<ModuleUISlot>();
        slotComponent.SetConnectedModuleSlot(moduleSlot);
        slotComponent.SetItem(module);
        slots.Add(slotComponent);
    }

    private void ModifyWidth(GameObject panel, int numModules)
    {
        panel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, WIDTH_INCREMENT * numModules);
    }
}
