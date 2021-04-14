using UnityEngine;


public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 5;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCentre;
    public Bounds bounds { get; set; }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    public HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDistance;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    //public List<GameObject> trees;
    //private int NUM_TREES;
    //private float TREE_Y_MIN;
    //private float TREE_Y_MAX;
    //TextureData textureSettings;


    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer,  Material worldMaterial)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);


        meshObject = new GameObject("Terrain Chunk");
        meshObject.layer = 7;
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshObject.AddComponent<TerrainCollider>();
        meshRenderer.material = worldMaterial;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject)
    {
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    Vector2 ViewerPosition => new Vector2(viewer.position.x, viewer.position.z);
    
    public void UpdateTerrainChunk()
    {
        if (heightMapReceived)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));

            bool wasVisible = IsVisible();
            bool visible = viewerDstFromNearestEdge <= maxViewDistance;

            if (visible)
            {
                int lodIndex = 0;
                for (int i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                        lodMesh.RequestMesh(heightMap, meshSettings);
                }
            }
            if (wasVisible != visible)
            {
                SetVisible(visible);
                if (onVisibilityChanged != null)
                {
                    onVisibilityChanged(this, visible);
                }       
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(ViewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    public GameObject GetTerrainObject()
    {
        return meshObject;
    }

}

class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod)
    {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }



    //public void GenerateTrees()
    //{
    //    Debug.LogWarning("Here");
    //    TREE_Y_MAX = GetGrassHeight();
    //    TREE_Y_MIN = 0;
    //    NUM_TREES = 100;
    //    Debug.LogWarning("3");
    //    Debug.LogWarning("4");
    //    Mesh mesh = meshFilter.mesh;
    //    Transform parent = meshFilter.transform;
    //    Vector3[] vertices = mesh.vertices;

    //    Debug.LogWarning("Max: " + TREE_Y_MAX);

    //    for (int i = 0; i < NUM_TREES; i++)
    //    {
    //        Debug.LogWarning("Here1");
    //        Vector3 position = transform.TransformPoint(vertices[Random.Range(0, vertices.Length)]);
    //        Debug.LogWarning("X: " + position.x + " Y: " + position.y +  " Z: " + position.z);
    //        if (position.y > TREE_Y_MIN && position.y < TREE_Y_MAX)
    //        {
    //            GameObject tree = (GameObject)Instantiate(trees[Random.Range(0, 2)], position, Quaternion.identity);
    //            tree.transform.parent = parent;
    //        }
    //    }
    //}

    //public float GetGrassHeight()
    //{
    //    float height = textureSettings.layers[1].startHeight;
    //    float heightMultiplier = heightMapSettings.heightMultiplier;
    //    return height * heightMultiplier;
    //}

}

