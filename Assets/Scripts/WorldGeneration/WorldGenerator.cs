using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : UnitySingleton<WorldGenerator>
{
    public List<GameObject> allBuildings;

    List<Vector2> pointsForTrees;
    List<Vector2> pointsForBushes;

    public List<GameObject> trees;
    public List<GameObject> bushes;

    public TextureData textureSettings;
    public HeightMapSettings heightMapSettings;

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
       //Debug.LogWarning("Vector: " + vec.x + " " + vec.y + " " + vec.z);
        pointsForTrees = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
        //Debug.LogWarning("Finished " + pointsForTrees.Count);
        List<GameObject> treesInstantiated = new List<GameObject>();
        foreach (Vector2 point in pointsForTrees)
        {
            int treeIndex = Random.Range(0, trees.Count);
            Vector3 pos = new Vector3(point.x,0,point.y);
            //Debug.LogWarning("pos: " + pos.x + " " + pos.y + " " + pos.z);
            float y = GetLandHeight(pos);
            //Debug.LogWarning(y);
            pos.y = y;
            float grassHeight = GetGrassHeight();
            if (y < grassHeight)
                treesInstantiated.Add(Instantiate(trees[treeIndex], pos, Quaternion.Euler(0, 0, 0)));
        }
        return treesInstantiated;
    }

    public List<GameObject> SpawnBushes(Vector3 vec)
    {
        pointsForBushes = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
        List<GameObject> bushesInstantiated = new List<GameObject>();
        foreach (Vector2 point in pointsForBushes)
        {
            int bushIndex = Random.Range(0, bushes.Count);
            Vector3 pos = new Vector3(point.x, 0, point.y);
            float y = GetLandHeight(pos);
            pos.y = y;
            bushesInstantiated.Add(Instantiate(bushes[bushIndex], pos, Quaternion.Euler(0, 0, 0)));
        }
        return bushesInstantiated;
    }

    public float GetGrassHeight()
    {
        float height = textureSettings.layers[1].startHeight;
        float heightMultiplier = heightMapSettings.heightMultiplier;
        return height * heightMultiplier;
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

    public float MyLandHeight(Vector3 coords)
    {
        RaycastHit hit;
        float heightAboveGround = 0;

        if (Physics.Raycast(coords, transform.TransformDirection(Vector3.down),out hit))
        {
            heightAboveGround = hit.point.y;
        }

        return hit.point.y;
    }
}
