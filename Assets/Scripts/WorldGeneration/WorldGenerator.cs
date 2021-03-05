using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : UnitySingleton<WorldGenerator>
{
    public GameObject smallBuilding;

    private void Update()
    {
        Debug.DrawRay(new Vector3(100, 20000, 100), Vector3.down * 40000, Color.red, 7);
    }

    public void SpawnBuilding(Vector3 pos, Quaternion rot)
    {
        pos.y = GetLandHeight(pos);
        GameObject building = Instantiate(smallBuilding, pos, rot);
    }

    public float GetLandHeight(Vector3 worldPos)
    {
        RaycastHit hit;
        int layerMask = 7;
        worldPos.y = 999999;
        Physics.Raycast(worldPos, Vector3.down, out hit, Mathf.Infinity, layerMask);
        Debug.DrawLine(Vector3.one * 2 + hit.point, Vector3.one * -2 + hit.point);
        return hit.point.y;
    }
}
