using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleModuleBuilder : MonoBehaviour
{
    List<Transform> moduleSlotLocations = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform)
        {
            if (child.name.Contains("ModSlot"))
                moduleSlotLocations.Add(child);
        }

        //Testing
        AddModule(1, 10);
        AddModule(2, 23);
        AddModule(3, 33);
        AddModule(4, 34);
    }

    void AddModule(int modSlot, int moduleID)
    {
        if (moduleSlotLocations.Count < modSlot)
        {
            Debug.LogError("Mod slot " + modSlot + " is out oof bounds");
            return;
        }

        GameObject module = Instantiate(ModuleService.instance.GetModule(moduleID));
        module.transform.SetParent(moduleSlotLocations[modSlot-1]);
        module.transform.localPosition = Vector3.zero;
        module.transform.localEulerAngles = Vector3.zero;
    }
}

