using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObjectGenerator : MonoBehaviour
{
    private int GrassLayerNumber = 0;
    private float spawnYMin;
    private float spawnYMax;
    private TextureData textureSettings;
    private HeightMapSettings heightMapSettings;


    private List<GameObject> trees;
    private List<GameObject> bushes;
    private List<GameObject> buildings;

    private int numOfTrees;
    private int numOfBushes;
    private int numOfBuildings;

    private int howOftenTreesSpawn = 20;
    private int howOftenBushesSpawn = 25;
    private int howOftenBuildingsSpawn = 3000;

    //List<Vector2> pointsForTrees;
    //List<Vector2> pointsForBushes;
    //pointsForBushes = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    //    foreach (Vector2 point in pointsForBushes)

    public void Start()
    {
        spawnYMin = GetGrassHeightMin();
        spawnYMax = GetGrassHeightMax();
        StartCoroutine(GenerateObjects(trees, numOfTrees, howOftenTreesSpawn));
        StartCoroutine(GenerateObjects(bushes, numOfBushes, howOftenBushesSpawn));
        StartCoroutine(GenerateObjects(buildings, numOfBuildings, howOftenBuildingsSpawn));
    }

    public void SetupSettings(TextureData textureSettings, HeightMapSettings heightMapSettings)
    {
        this.textureSettings = textureSettings;
        this.heightMapSettings = heightMapSettings;
        SetupTrees();
        SetupBushes();
        SetupBuildings();
    }

    public void SetupTrees()
    {
        GameObject tree1 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 1", typeof(GameObject));
        GameObject tree2 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 2", typeof(GameObject));
        GameObject tree3 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 3", typeof(GameObject));
        GameObject tree4 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 4", typeof(GameObject));

        trees = new List<GameObject>() { tree1, tree2, tree3, tree4 };
    }

    public void SetupBushes()
    {
        GameObject bush1 = (GameObject)Resources.Load("Prefabs/Trees/Bush 1", typeof(GameObject));
        GameObject bush2 = (GameObject)Resources.Load("Prefabs/Trees/Bush 2", typeof(GameObject));

        bushes = new List<GameObject>() { bush1, bush2};
    }

    public void SetupBuildings()
    {
        GameObject building1 = (GameObject)Resources.Load("Prefabs/Buildings/smallHouse", typeof(GameObject));
        GameObject building2 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreA", typeof(GameObject));
        GameObject building3 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreB", typeof(GameObject));

        buildings = new List<GameObject>() { building1, building2, building3 };
    }


    IEnumerator GenerateObjects(List<GameObject> objects, int numOfObjects, int howOftenObjectSpawn)
    {
        yield return new WaitForSeconds(.2f);
        int objectIndex = Random.Range(0, objects.Count);
        GameObject terrainObject = objects[objectIndex];
        Mesh mesh = GetComponent<MeshFilter>().mesh; //Get the mesh filter of the gameobject we are connected to (Should be the terrain.)
        Vector3[] vertices = mesh.vertices; // Create an array and reference it to all of the vertices in the terrain. (basically create an array that lists all of the vertices)
        for (int j = 0; j < vertices.Length; j += howOftenObjectSpawn) // For every single vertex in array (and by extension the terrain)
        {
            Vector3 position = transform.TransformPoint(vertices[j]); // Get the position of the vertex we're on.
            if (position.y > spawnYMin && position.y < spawnYMax)
            {
                Instantiate(terrainObject, position, Quaternion.identity);
            }
        }
        yield break;
    }

    public float GetGrassHeightMax()
    {
        float height = textureSettings.layers[GrassLayerNumber + 1].startHeight;
        float heightMultiplier = heightMapSettings.heightMultiplier;
        return height * heightMultiplier;
    }

    public float GetGrassHeightMin()
    {
        return textureSettings.layers[GrassLayerNumber].startHeight;
    }
}
