using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : UnitySingleton<WorldGenerator>
{
    public List<GameObject> allBuildings;

    private void Update()
    {

    }

    public GameObject SpawnBuilding(Vector3 pos, Quaternion rot)
    {
        float y = GetLandHeight(pos);
        int buildingIndex =  UnityEngine.Random.Range(0, allBuildings.Count);
        GameObject building = Instantiate(allBuildings[buildingIndex], new Vector3(pos.x, y, pos.z), rot);
        return building;
    }

    public float GetLandHeight(Vector3 worldPos)
    {
        RaycastHit hit;
        Vector3 startPos = new Vector3(worldPos.x, 10000, worldPos.z);
        Physics.Raycast(startPos, Vector3.down, out hit, 20000f);
        Debug.DrawLine(Vector3.one * 2 + hit.point, Vector3.one * -2 + hit.point);
        Debug.DrawLine(startPos, hit.point, Color.red, 10000f);
        return hit.point.y;
    }
}
