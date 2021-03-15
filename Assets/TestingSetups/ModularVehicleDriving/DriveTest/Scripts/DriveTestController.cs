using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTestController : MonoBehaviour
{
    //Prefabs
    public GameObject _1x1CarPrefab;
    public GameObject _2x1CarPrefab;
    public GameObject _3x1CarPrefab;
    public GameObject _4x1CarPrefab;
    public GameObject cameraPrefab;

    private GameObject curCar;

    void DestroyOldCar()
    {
        Destroy(curCar);
    }

    void CreateCamera()
    {
        if(Camera.main)
            Destroy(Camera.main.gameObject);

        Transform camera = Instantiate(cameraPrefab, Vector3.zero, Quaternion.identity).transform;
        camera.SetParent(curCar.transform);
        camera.localPosition = new Vector3(0, 25, -80);
        camera.localEulerAngles = new Vector3(10, 0, 0);
    }

    public void Spawn1x1Car()
    {
        DestroyOldCar();
        curCar = Instantiate(_1x1CarPrefab, Vector3.zero, Quaternion.identity);
        CreateCamera();
        curCar.GetComponent<VehicleController>().GiveControl();
    }
    public void Spawn2x1Car()
    {
        DestroyOldCar();
        curCar = Instantiate(_2x1CarPrefab, Vector3.zero, Quaternion.identity);
        CreateCamera();
        curCar.GetComponent<VehicleController>().GiveControl();
    }
    public void Spawn3x1Car()
    {
        DestroyOldCar();
        curCar = Instantiate(_3x1CarPrefab, Vector3.zero, Quaternion.identity);
        CreateCamera();
        curCar.GetComponent<VehicleController>().GiveControl();
    }
    public void Spawn4x1Car()
    {
        DestroyOldCar();
        curCar = Instantiate(_4x1CarPrefab, Vector3.zero, Quaternion.identity);
        CreateCamera();
        curCar.GetComponent<VehicleController>().GiveControl();
    }
}
