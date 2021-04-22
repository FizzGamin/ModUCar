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
    private MeshSettings meshSettings;

    private List<GameObject> trees;
    private List<GameObject> bushes;
    private List<GameObject> buildings;

    private int treeRadius;
    private int bushRadius;
    private int buildingRadius;

    public void Start()
    {
        spawnYMin = GrassHeightMin;
        spawnYMax = GetGrassHeightMax();
        treeRadius = 10 * (int)meshSettings.meshScale;
        bushRadius = 15 * (int)meshSettings.meshScale;
        buildingRadius = 80 * (int)meshSettings.meshScale;
        StartCoroutine(GenerateObjects(trees, treeRadius));
        StartCoroutine(GenerateObjects(bushes, bushRadius));
        StartCoroutine(GenerateObjects(buildings, buildingRadius, 1));
    }



    public void SetupSettings(TextureData textureSettings, HeightMapSettings heightMapSettings, MeshSettings meshSettings)
    {
        this.textureSettings = textureSettings;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
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

        bushes = new List<GameObject>() { bush1, bush2 };
    }

    public void SetupBuildings()
    {
        GameObject building1 = (GameObject)Resources.Load("Prefabs/Buildings/smallHouse", typeof(GameObject));
        GameObject building2 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreA", typeof(GameObject));
        GameObject building3 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreB", typeof(GameObject));

        buildings = new List<GameObject>() { building1, building2, building3 };
    }


    IEnumerator GenerateObjects(List<GameObject> objects, int radius, int objectCountLimt = 99999999)
    {
        yield return new WaitForSeconds(.1f);
        Mesh mesh = GetComponent<MeshFilter>().mesh; //Get the mesh filter of the gameobject we are connected to (The terrain)
        Vector3[] vertices = mesh.vertices; //Create an array and reference it to all of the vertices in the terrain. (basically create an array that lists all of the vertices)
        int chunkSize = getChunkSize(vertices);
        float size = chunkSize / meshSettings.meshScale;
        List<Vector2> pointsForGeneration = PoissonDiscSampling.GeneratePoints(radius, new Vector2(size, size));
        Debug.LogWarning("Length: " + vertices.Length);
        //Debug.LogWarning(vertices[0] + "" +  vertices[1] + "" + vertices[2] + "" + vertices[3]);

        int count = 0;

        foreach (Vector2 v in pointsForGeneration)
        {
            if (count++ < objectCountLimt)
            {
                int index = getIndex((int)v.x, (int)v.y, chunkSize, vertices.Length, vertices[0]);//v.x, v.y, vertices[0]);

                Vector3 position = transform.TransformPoint(vertices[index]);
                if (position.y > spawnYMin && position.y < spawnYMax)
                {
                    int objectIndex = Random.Range(0, objects.Count);
                    GameObject terrainObject = objects[objectIndex];
                    Instantiate(terrainObject, position, Quaternion.identity);
                }
            }
        }

        //for (int i = 0; i < vertices.Length; i++) // For every single vertex in array (and by extension the terrain)
        //{
        //    Vector3 position = transform.TransformPoint(vertices[i]); // Get the position of the vertex we're on.

        //    Vector2 temp = new Vector2(Mathf.Abs(Mathf.CeilToInt(position.x % chunkSize / meshSettings.meshScale)), Mathf.Abs(Mathf.CeilToInt(position.z % chunkSize / meshSettings.meshScale)));

        //    if (pointsForGeneration.Contains(temp))
        //    {
        //        if (position.y > spawnYMin && position.y < spawnYMax)
        //        {
        //            int objectIndex = Random.Range(0, objects.Count);
        //            GameObject terrainObject = objects[objectIndex];
        //            Instantiate(terrainObject, position, Quaternion.identity);
        //        }
        //    }
        //}
        yield break;
    }

    private int getIndex(int x, int y, int chunkSize, int length, Vector3 vector3)
    {
        if (length < 1)
            return 0;

        if (Mathf.Abs(x) < chunkSize)
            chunkSize = chunkSize / 2 + 1;
        else
            chunkSize++;

        x = x % chunkSize / (int)meshSettings.meshScale;
        y = y % chunkSize / (int)meshSettings.meshScale;

        if (length > Mathf.Pow(2 * x, 2))
        {
            int startingX = (int)(vector3.x % chunkSize / meshSettings.meshScale);
            int startingY = (int)(vector3.z % chunkSize / meshSettings.meshScale); //Since vector 3 is a 3d vector the y value in vector2 is equivalent to the z value in vector3

            //Debug.LogWarning("startingX: " + startingX + " startingY: " + startingY);

            int differenceX = Mathf.Abs((startingX - x)); //Since starting x will almosty always be negative then take absolute value
            int differenceY = startingY - y;

            int yCalculated = (differenceY * (Mathf.Abs(startingY * 2) + 1)); //convert y coord to index value, add the 1 for the offset

            return differenceX + yCalculated;
        }
        else
        {

            return 0;
        }

    }

    private int getChunkSize(Vector3[] vertices)
    {
        if (vertices.Length < 1)
            return 0;
        float max = vertices[vertices.Length - 1].x;
        float min = vertices[0].x;
        return (int)Mathf.Abs(max - min);
    }


    public float GetGrassHeightMax()
    {
        float height = textureSettings.layers[GrassLayerNumber + 1].startHeight;
        float heightMultiplier = heightMapSettings.heightMultiplier;
        return height * heightMultiplier;
    }

    public float GrassHeightMin => textureSettings.layers[GrassLayerNumber].startHeight;
}
