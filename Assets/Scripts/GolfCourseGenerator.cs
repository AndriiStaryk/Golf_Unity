using UnityEngine;

public class GolfCourseGenerator : MonoBehaviour
{
    public Terrain terrain;
    public int width = 512;
    public int height = 512;
    public float scale = 20f;

    void Start()
    {
        GenerateTerrain();
        CreateHole();
        GenerateTrees();
        GenerateFairway();
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

    void CreateHole()
    {
        GameObject hole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        hole.transform.position = new Vector3(width / 2, 0, height / 2);
        hole.transform.localScale = new Vector3(1f, 0.1f, 1f);
        hole.GetComponent<Renderer>().material.color = Color.black;
    }

    void GenerateTrees()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 position = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
            GameObject tree = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.transform.position = position;
            tree.transform.localScale = new Vector3(2f, 10f, 2f);
            tree.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    void GenerateFairway()
    {
        TerrainData terrainData = terrain.terrainData;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        int startX = 10;
        int startY = 10;
        int endX = width / 2;
        int endY = height / 2;
        int fairwayWidth = 20;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (Mathf.Abs(x - startX) < fairwayWidth || Mathf.Abs(y - startY) < fairwayWidth)
                {
                    heights[x, y] = 0.02f;
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
