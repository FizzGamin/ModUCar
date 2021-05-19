using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VehicleGeneratorWeight
{
    public int weight = 1;
    public GameObject vehicleChassis;
}

public class VehicleGenerator : MonoBehaviour
{
    public List<VehicleGeneratorWeight> spawnWeights = new List<VehicleGeneratorWeight>();

    private void Start()
    {
        List<GameObject> allCars = new List<GameObject>();
        foreach (VehicleGeneratorWeight carType in spawnWeights)
            for (int i = 0; i < carType.weight; i++)
                allCars.Add(carType.vehicleChassis);

        int randomCar = Random.Range(0, allCars.Count);
        Instantiate(allCars[randomCar], transform.position, transform.rotation);
    }
}
