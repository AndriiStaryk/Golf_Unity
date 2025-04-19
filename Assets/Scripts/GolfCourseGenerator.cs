using UnityEngine;

public class GolfCourseGenerator : MonoBehaviour
{
    public Terrain terrain;
    public GameObject holePrefab;
    public GameObject flagPrefab;
    public GameObject[] treePrefabs;
    
    public int width = 512;
    public int height = 512;
    public float scale = 20f;
    
    public int treesCount = 1200;
    
    void Start()
    {
        if (terrain == null) {
            Debug.LogError("No terrain assigned! Please assign a terrain in the inspector.");
            return;
        }
        
        GenerateTerrain();
        CreateHoleWithFlag();
        GenerateTrees();
    }
    
    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        
        Vector3 originalSize = terrainData.size;
        
        terrainData.heightmapResolution = width + 1;
        
        terrainData.size = new Vector3(originalSize.x, 50, originalSize.z);
        
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
    holePosition = terrain.transform.position + new Vector3(holePosition.x, 0, holePosition.z);
    
    float holeHeight = terrain.SampleHeight(holePosition);
    holePosition.y = holeHeight;

    GameObject hole = Instantiate(holePrefab, holePosition, Quaternion.identity);

    // Create the trigger detection zone
    GameObject triggerZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    triggerZone.transform.position = holePosition + new Vector3(0, -0.2f, 0);
    triggerZone.transform.localScale = new Vector3(0.6f, 0.1f, 0.6f);
    triggerZone.GetComponent<Renderer>().enabled = false; // Hide it
    triggerZone.GetComponent<Collider>().isTrigger = true;
    triggerZone.name = "HoleTrigger";
    triggerZone.transform.parent = hole.transform;

    // Add HoleSink script
    triggerZone.AddComponent<HoleSink>();

    // Place flag above the hole
    Vector3 flagPosition = holePosition + new Vector3(0, 3f, 0);
    Instantiate(flagPrefab, flagPosition, Quaternion.identity);
}
    void GenerateTrees()
    {
        for (int i = 0; i < treesCount; i++)
        {
            float x = Random.Range(0, terrain.terrainData.size.x);
            float z = Random.Range(0, terrain.terrainData.size.z);
            
            Vector3 worldPos = terrain.transform.position + new Vector3(x, 0, z);
            
            int randomIndex = Random.Range(0, treePrefabs.Length);
            GameObject selectedPrefab = treePrefabs[randomIndex];
            
            RaycastHit hit;
            if (Physics.Raycast(worldPos + Vector3.up * 100, Vector3.down, out hit, 1000f) && 
                hit.collider.GetComponent<Terrain>() != null)
            {
                GameObject tree = Instantiate(selectedPrefab, hit.point, 
                    Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
            
        }
    }
}