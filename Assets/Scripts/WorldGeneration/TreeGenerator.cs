using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public List<GameObject> trees;
    public List<GameObject> bushes;
    public int NUM_TREES;
    private float spawnYMin;
    private float spawnYMax;
    public TextureData textureSettings;
    public HeightMapSettings heightMapSettings;
    private int howOftenTreesSpawn = 20;
    private int howOftenbushesSpawn = 25;

    //List<Vector2> pointsForTrees;
    //List<Vector2> pointsForBushes;
    //pointsForBushes = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
    //    foreach (Vector2 point in pointsForBushes)

    public void Start()
    {
        spawnYMin = GetGrassHeightMin();
        spawnYMax = GetGrassHeightMax();
        StartCoroutine(GenerateTree());
        StartCoroutine(GenerateBushes());
    }

    IEnumerator GenerateTree()
    {
        yield return new WaitForSeconds(.2f);
        int treeIndex = Random.Range(0, trees.Count);
        GameObject tree = trees[treeIndex];
        Mesh mesh = GetComponent<MeshFilter>().mesh; //Get the mesh filter of the gameobject we are connected to (Should be the terrain.)
        Vector3[] vertices = mesh.vertices; // Create an array and reference it to all of the vertices in the terrain. (basically create an array that lists all of the vertices)
        for (int j = 0; j < vertices.Length; j += howOftenTreesSpawn) // For every single vertex in array (and by extension the terrain)
        {
            Vector3 position = transform.TransformPoint(vertices[j]); // Get the position of the vertex we're on.
            if (position.y > spawnYMin && position.y < spawnYMax)
            {
                Instantiate(tree, position, Quaternion.identity);
            }
        }
        yield break;
    }

    IEnumerator GenerateBushes()
    {
        yield return new WaitForSeconds(.2f);
        int bushesIndex = Random.Range(0, bushes.Count);
        GameObject bush = bushes[bushesIndex];
        Mesh mesh = GetComponent<MeshFilter>().mesh; //Get the mesh filter of the gameobject we are connected to (Should be the terrain.)
        Vector3[] vertices = mesh.vertices; // Create an array and reference it to all of the vertices in the terrain. (basically create an array that lists all of the vertices)
        for (int j = 0; j < vertices.Length; j += howOftenbushesSpawn) // For every single vertex in array (and by extension the terrain)
        {
            Vector3 position = transform.TransformPoint(vertices[j]); // Get the position of the vertex we're on.
            if (position.y > spawnYMin && position.y < spawnYMax)
            {
                Instantiate(bush, position, Quaternion.identity);
            }
        }
        yield break;
    }

    public float GetGrassHeightMax()
    {
        float height = textureSettings.layers[1].startHeight;
        float heightMultiplier = heightMapSettings.heightMultiplier;
        return height * heightMultiplier;
    }

    public float GetGrassHeightMin()
    {
        return textureSettings.layers[0].startHeight;
    }
}
