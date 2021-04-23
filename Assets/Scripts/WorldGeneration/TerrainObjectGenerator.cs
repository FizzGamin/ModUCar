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
        if (Random.Range(0, 100) < 20) //20% Chance of spawning a forest
            treeRadius = Random.Range(4, 7);
        else
            treeRadius = Random.Range(10, 25);
        bushRadius = Random.Range(treeRadius, treeRadius + 5);
        buildingRadius = Random.Range(80, 200);
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
        float chunkSizeScaled = getChunkSize(vertices) / meshSettings.meshScale;
        List<Vector2> pointsForGeneration = PoissonDiscSampling.GeneratePoints(radius, new Vector2(chunkSizeScaled, chunkSizeScaled));

        int count = 0;

        foreach (Vector2 v in pointsForGeneration)
        {
            if (count++ < objectCountLimt)
            {
                Vector2 point = new Vector2(v.x - 61, v.y - 61);

                int index = getIndex((int)point.x, (int)point.y, Mathf.RoundToInt(chunkSizeScaled), vertices.Length);
                Debug.LogWarning("X: " + point.x + " Y: " + point.y + " vertices[0]: " + vertices[0] + " vertices[" + index + "]: " + vertices[index]);

                Vector3 position = transform.TransformPoint(vertices[index]);
                if (position.y > spawnYMin && position.y < spawnYMax)
                {
                    int objectIndex = Random.Range(0, objects.Count);
                    GameObject terrainObject = objects[objectIndex];
                    Instantiate(terrainObject, position, Quaternion.identity);
                }
            }
        }
        yield break;
    }

    private int getIndex(int x, int y, int chunkSizeScaled, int length)
    {
        if (length < 1) return 0;

        //The Starting x and Y cords will always be -61 and 61  (Times mesh scale but we're scaling everything down anyways so we leave it at -61,61)
        int startingX = -61; 
        int startingY = 61;

        if (length > Mathf.Pow(chunkSizeScaled, 2))//if LOD is greater than 0 then vertices count will be less normal
        {
            int differenceX = Mathf.Abs(startingX - x); //Since starting x will always be negative then take absolute value
            int differenceY = startingY - y;

            int yCalculated = differenceY * (chunkSizeScaled + 1); //convert y coord to index value, add the 1 for the offset

            return differenceX + yCalculated;
        }
        else //For the case when LOD > 0 for the current chunk
        {
            //Code goes Here
            Debug.LogWarning("Length: " + length + " MathF: " + Mathf.Pow(chunkSizeScaled, 2));
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
