using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSlot : MonoBehaviour
{
    private VehicleController vehicleController;
    private VehicleModule module;

    void Start()
    {
        vehicleController = gameObject.GetComponentInParent<VehicleController>();
        module = gameObject.GetComponentInChildren<VehicleModule>();
        if (module != null)
        {
            module.Equip(vehicleController);
        }
    }

    public VehicleModule GetModule()
    {
        return module;
    }

    public VehicleModule SetModule(VehicleModule newModule)
    {
        VehicleModule old = RemoveModule();
        newModule.transform.parent = gameObject.transform;
        newModule.gameObject.SetActive(true);
        newModule.Equip(vehicleController);
        return old;
    }

    public VehicleModule RemoveModule()
    {
        if (module == null) return null;

        VehicleModule old = module;
        module.gameObject.SetActive(false);
        module.transform.parent = null;
        module.Unequip(vehicleController);
        module = null;

        return old;
    }
}
