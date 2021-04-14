using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Material mapMaterial;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float meshWorldSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    public List<GameObject> trees;

    void Start()
    {
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

        UpdateVisibleChunks();
    }

    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);//, textureSettings, this.trees);
                        terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();




                        //newChunk.UpdateTerrainChunk();
                        //var h = newChunk.heightMap.values[1, 1];
                        //Debug.LogWarning(h);





                        //Generate buildings in the chunk
                        Random.InitState((int)(newChunk.bounds.center.x + newChunk.bounds.center.z));
                        GameObject building = WorldGenerator.instance.SpawnBuilding(new Vector3(
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.x,
                                0,
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.z)
                            , Quaternion.Euler(0,Random.Range(0,360),0));
                        building.transform.SetParent(newChunk.GetTerrainObject().transform);

                        //Generate Trees in the chunk
                        List<GameObject> trees = WorldGenerator.instance.SpawnTrees(new Vector3(
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.x,
                                0,
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.z));
                        foreach(GameObject tree in trees)
                        {
                            tree.transform.SetParent(newChunk.GetTerrainObject().transform);
                        }

                        //Generate Bushes in the chunk
                        List<GameObject> bushes = WorldGenerator.instance.SpawnBushes(new Vector3(
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.x,
                                0,
                                Random.Range(-newChunk.bounds.size.x / 2, newChunk.bounds.size.x / 2) + newChunk.bounds.center.z));
                        foreach (GameObject bush in bushes)
                        {
                            bush.transform.SetParent(newChunk.GetTerrainObject().transform);
                        }
                    }
                }

            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
}
[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;

    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}