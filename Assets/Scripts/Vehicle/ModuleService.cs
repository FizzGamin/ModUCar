using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleService : UnitySingleton<ModuleService>
{
    Dictionary<int, GameObject> modules = new Dictionary<int, GameObject>();

    void Start()
    {
        Debug.Log("Starting Module Service Loading...");

        UnityEngine.Object[] itemObjs = Resources.LoadAll("VehicleModules/Control-10");
        int index = 10;// control start index
        foreach (GameObject item in itemObjs)
        {
            modules.Add(index++, item);
        }
        itemObjs = Resources.LoadAll("VehicleModules/FuelTank-20");
        index = 20;// fuel tank start index
        foreach (GameObject item in itemObjs)
        {
            modules.Add(index++, item);
        }
        itemObjs = Resources.LoadAll("VehicleModules/Storage-30");
        index = 30;// storage start index
        foreach (GameObject item in itemObjs)
        {
            modules.Add(index++, item);
        }

        Debug.Log("Finished Module Service Loading...");
    }

    public GameObject GetModule(int moduleId)
    {
        if (modules.ContainsKey(moduleId))
            return modules[moduleId];
        else
        {
            Debug.LogError("Couldn't Find Module by ID");
            return null;
        }
    }
}