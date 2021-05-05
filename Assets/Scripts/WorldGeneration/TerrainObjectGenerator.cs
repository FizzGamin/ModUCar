using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObjectGenerator : MonoBehaviour
{
    private int GrassLayerNumber = 0;
    private int RockyLayerNumber = 1;
    private int RockySnowLayerNumber = 2;

    private TextureData textureSettings;
    private HeightMapSettings heightMapSettings;
    private MeshSettings meshSettings;

    private List<GameObject> trees;
    private List<GameObject> bushes;
    private List<GameObject> bigRocks;
    private List<GameObject> smallRocks;
    private List<GameObject> snowRocks;
    private List<GameObject> buildings;
    private List<GameObject> enemies;

    private int treeRadius;
    private int bushRadius;
    private int buildingRadius;
    private int bigRockRadius;
    private int smallRockRadius;
    private int snowRockRadius;
    private List<int> enemiesRadius = new List<int>();

    private Vector2 chunkCoord;
    private int enemySpawnRate;//The greater the difficulty the higher the spawn rate of monsters

    public void Start()
    {
        SetupRadius();
        StartCoroutines();
    }

    private void StartCoroutines()
    {
        int extraYIncrease = 0;
        Vector3 scale = new Vector3(2f, 2f, 2f);
        StartCoroutine(GenerateObjects(trees, treeRadius, extraYIncrease, GrassLayerNumber, scale));
        StartCoroutine(GenerateObjects(bushes, bushRadius, extraYIncrease, GrassLayerNumber, scale));
        scale = new Vector3(8f, 8f, 8f);
        StartCoroutine(GenerateObjects(bigRocks, bigRockRadius, extraYIncrease, RockyLayerNumber, scale));
        StartCoroutine(GenerateObjects(smallRocks, smallRockRadius, extraYIncrease, GrassLayerNumber, scale));
        StartCoroutine(GenerateObjects(snowRocks, snowRockRadius, extraYIncrease, RockySnowLayerNumber, scale));
        scale = new Vector3(1f, 1f, 1f);
        StartCoroutine(GenerateObjects(buildings, buildingRadius, extraYIncrease, GrassLayerNumber, scale));

        extraYIncrease = 50;//Increase to 50 for monsters so they have to fall giving time for NavSurface to be generated

        int index = 0;
        foreach (GameObject enemy in enemies)
        {
            //StartCoroutine(GenerateObjects(new List<GameObject>() { enemies[index] }, enemiesRadius[index++], extraYIncrease, GrassLayerNumber, new Vector3(1f, 1f, 1f)));
        }
    }

    public void SetupSettings(TextureData textureSettings, HeightMapSettings heightMapSettings, MeshSettings meshSettings, Vector2 coord)
    {
        this.textureSettings = textureSettings;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        chunkCoord = coord;
        SetupTrees();
        SetupBushes();
        SetupRocks();
        SetupBuildings();
        SetupEnemies();
    }

    private void SetupRadius()
    {
        enemySpawnRate = (int)(Mathf.Abs(chunkCoord.x) + Mathf.Abs(chunkCoord.y));

        if (Random.Range(0, 100) < 20) //20% Chance of spawning a forest
            treeRadius = Random.Range(4, 7);
        else
            treeRadius = Random.Range(10, 50);

        bushRadius = Random.Range(treeRadius, treeRadius + 5);
        bigRockRadius = Random.Range(10, 15);
        smallRockRadius = Random.Range(50, 60);
        snowRockRadius = Random.Range(10, 15);
        buildingRadius = Random.Range(80, 100);

        //if radius goes below 10 then sets radius to 10
        enemiesRadius.Add(Mathf.Max(10, Random.Range(30, 70) - enemySpawnRate)); //Spider at Index 0
        enemiesRadius.Add(Mathf.Max(10, Random.Range(70, 200) - enemySpawnRate)); //Truck at Index 1
    }

    private void SetupEnemies()
    {
        GameObject spider = (GameObject)Resources.Load("Prefabs/Enemy_Spider", typeof(GameObject));
        GameObject truck = (GameObject)Resources.Load("Prefabs/Enemy_Truck_AI", typeof(GameObject));

        enemies = new List<GameObject> { spider, truck };
    }

    private void SetupTrees()
    {
        GameObject tree1 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 1", typeof(GameObject));
        GameObject tree2 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 2", typeof(GameObject));
        GameObject tree3 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 3", typeof(GameObject));
        GameObject tree4 = (GameObject)Resources.Load("Prefabs/Trees/RedWood 4", typeof(GameObject));

        trees = new List<GameObject>() { tree1, tree2, tree3, tree4 };
    }

    private void SetupBushes()
    {
        GameObject bush1 = (GameObject)Resources.Load("Prefabs/Trees/Bush 1", typeof(GameObject));
        GameObject bush2 = (GameObject)Resources.Load("Prefabs/Trees/Bush 2", typeof(GameObject));

        bushes = new List<GameObject>() { bush1, bush2 };
    }

    private void SetupBuildings()
    {
        GameObject building1 = (GameObject)Resources.Load("Prefabs/Buildings/smallHouse", typeof(GameObject));
        GameObject building2 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreA", typeof(GameObject));
        GameObject building3 = (GameObject)Resources.Load("Prefabs/Buildings/smallStoreB", typeof(GameObject));

        buildings = new List<GameObject>() { building1, building2, building3 };
    }

    private void SetupRocks()
    {
        GameObject bigRock1 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock2", typeof(GameObject));
        GameObject bigRock2 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock3", typeof(GameObject));
        GameObject bigRock3 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock4A", typeof(GameObject));
        GameObject bigRock4 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock5A", typeof(GameObject));
        GameObject bigRock5 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock6A", typeof(GameObject));

        GameObject smallRock1 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock1B", typeof(GameObject));
        GameObject smallRock2 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock1C", typeof(GameObject));
        GameObject smallRock3 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock1D", typeof(GameObject));
        GameObject smallRock4 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs/Rock1E", typeof(GameObject));

        GameObject snowRock1 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs_snow/Rock1_grup1", typeof(GameObject));
        GameObject snowRock2 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs_snow/Rock1_grup2", typeof(GameObject));
        GameObject snowRock3 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs_snow/Rock1_grup3", typeof(GameObject));
        GameObject snowRock4 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs_snow/Rock1_grup4", typeof(GameObject));
        GameObject snowRock5 = (GameObject)Resources.Load("Prefabs/RocksandBoulders/Rocks/Prefabs_snow/Rock1_grup5", typeof(GameObject));

        bigRocks = new List<GameObject> { bigRock1, bigRock2, bigRock3, bigRock4, bigRock5 };
        smallRocks = new List<GameObject> { smallRock1, smallRock2, smallRock3, smallRock4 };
        snowRocks = new List<GameObject> { snowRock1, snowRock2, snowRock3, snowRock4, snowRock5 };
    }

    IEnumerator GenerateObjects(List<GameObject> objects, int radius, int yIncrease, int LayerNumber, Vector3 scale, int objectCountLimt = 99999999)
    {
        float spawnYMax = GetLayerHeight(LayerNumber, true);
        float spawnYMin = GetLayerHeight(LayerNumber, false);

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
                
                Vector3 position = transform.TransformPoint(vertices[index]);
                Debug.LogWarning("LayerNumber: " + LayerNumber + " YMax: " + spawnYMax + " YMin: " + spawnYMin + " Y: " + position.y + " Layer0: " + textureSettings.layers[0].startHeight + " Layer1: " + textureSettings.layers[1].startHeight + " Layer2: " + textureSettings.layers[2].startHeight + " Layer3: " + textureSettings.layers[3].startHeight);

                if (position.y > spawnYMin && position.y < spawnYMax)
                {
                    int objectIndex = Random.Range(0, objects.Count);
                    GameObject terrainObject = objects[objectIndex];
                    Quaternion rotateAngle = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                    Instantiate(terrainObject, new Vector3(position.x, position.y + yIncrease, position.z), rotateAngle);
                    terrainObject.transform.localScale = scale;
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

    private float GetLayerHeight(int LayerNumber, bool max)
    {

        float height;
        int add = max ? 1 : 0;
        if (LayerNumber < textureSettings.layers.Length)
            height = textureSettings.layers[LayerNumber + add].startHeight;
        else
            height = 1;
        float heightMultiplier = heightMapSettings.heightMultiplier;
        return height * heightMultiplier;
    }
}
