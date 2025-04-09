using UnityEngine;

public class GolfCourseGenerator : MonoBehaviour
{
    public Terrain terrain;
    public GameObject holePrefab;
    public GameObject flagPrefab;
    public GameObject treePrefab;

    public int width = 512;
    public int height = 512;
    public float scale = 20f;

    void Start()
    {
        GenerateTerrain();
        CreateHoleWithFlag();
        GenerateTrees();
        //GenerateFairway();
    }

    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, 50, height);

        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = Mathf.PerlinNoise(x / scale, y / scale) * 0.1f;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void CreateHoleWithFlag()
    {
        Vector3 holePosition = new Vector3(width / 2, 0, height / 2);

        // Instantiate the hole prefab at the ground level
        GameObject hole = Instantiate(holePrefab, holePosition, Quaternion.identity);

        // Adjust the hole position to sit on the terrain
        float holeHeight = terrain.SampleHeight(holePosition);
        hole.transform.position = new Vector3(holePosition.x, holeHeight, holePosition.z);

        // Instantiate the flag prefab slightly above the hole
        Vector3 flagPosition = holePosition + new Vector3(0, 3f, 0);
        GameObject flag = Instantiate(flagPrefab, flagPosition, Quaternion.identity);
    }

    void GenerateTrees()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 position = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
            Instantiate(treePrefab, position, Quaternion.identity);
        }
    }

    // void GenerateFairway()
    // {
    //     TerrainData terrainData = terrain.terrainData;
    //     float[,] heights = terrainData.GetHeights(0, 0, width, height);

    //     int startX = 10;
    //     int startY = 10;
    //     int endX = width / 2;
    //     int endY = height / 2;
    //     int fairwayWidth = 20;

    //     for (int x = startX; x < endX; x++)
    //     {
    //         for (int y = startY; y < endY; y++)
    //         {
    //             if (Mathf.Abs(x - startX) < fairwayWidth || Mathf.Abs(y - startY) < fairwayWidth)
    //             {
    //                 heights[x, y] = 0.02f;
    //             }
    //         }
    //     }

    //     terrainData.SetHeights(0, 0, heights);
    // }
}
