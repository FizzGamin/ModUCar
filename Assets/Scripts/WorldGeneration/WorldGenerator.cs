using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : UnitySingleton<WorldGenerator>
{
    public List<GameObject> allBuildings;

    List<Vector2> points;

    public GameObject tree;

    public float radius = 1;
    public Vector2 regionSize = Vector2.one;
    public int rejectionSamples = 30;

    private void Update()
    {

    }

    public GameObject SpawnBuilding(Vector3 pos, Quaternion rot)
    {
        float y = GetLandHeight(pos);
        int buildingIndex = Random.Range(0, allBuildings.Count);
        GameObject building = Instantiate(allBuildings[buildingIndex], new Vector3(pos.x, y, pos.z), rot);
        return building;
    }

    public List<GameObject> SpawnTrees(Vector3 vec)
    {
        Debug.LogWarning("Vector: " + vec.x + " " + vec.y + " " + vec.z);
        points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
        Debug.LogWarning("Finished " + points.Count);
        List<GameObject> treesInstantiated = new List<GameObject>();
        foreach (Vector2 point in points)
        {
            Vector3 pos = new Vector3(point.x,0,point.y);
            Debug.LogWarning("pos: " + pos.x + " " + pos.y + " " + pos.z);
            float y = GetLandHeight(pos);
            treesInstantiated.Add(Instantiate(tree, pos, Quaternion.Euler(0, 0, 0)));
        }
        return treesInstantiated;
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
