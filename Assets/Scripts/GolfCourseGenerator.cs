using UnityEngine;

public class GolfCourseGenerator : MonoBehaviour
{
    public Terrain terrain;
    public GameObject holePrefab;
    public GameObject flagPrefab;
    public GameObject[] treePrefabs;
    public GameObject skyIndicatorPrefab; 

    public int width = 512;
    public int height = 512;
    public float scale = 20f;

    public int treesCount = 1200;
    public float treeHeightMultiplier = 1.5f; 

    // Mountain range settings
    public bool createMountainBoundary = true;
    public float mountainHeight = 0.8f; // How tall the mountains are (0-1)
    public float mountainWidth = 30f;   // How wide the mountain boundary is
    public float mountainRoughness = 12f; // Controls how jagged the mountains are
    
    // Out of bounds settings
    public bool checkOutOfBounds = true;
    public float outOfBoundsY = -5f;    // Y level that triggers reset

    // Controls for hole placement
    [Range(0.15f, 0.35f)]
    public float minDistanceFromCenter = 0.2f; // (as percentage)
    [Range(0.35f, 0.65f)]
    public float maxDistanceFromCenter = 0.4f; // (as percentage)

    public float skyIndicatorHeight = 100f;
    public Color skyIndicatorColor = Color.red;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("No terrain assigned! Please assign a terrain in the inspector.");
            return;
        }

        GenerateTerrain();
        CreateHoleWithFlag();
        GenerateTrees();
        
        if (checkOutOfBounds)
        {
            CreateOutOfBoundsDetector();
        }
    }

    void GenerateTerrain()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 originalSize = terrainData.size;
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(originalSize.x, 50, originalSize.z);

        float[,] heights = new float[width, height];

        // Base terrain - gentle hills using Perlin noise
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = Mathf.PerlinNoise(x / scale, y / scale) * 0.1f;
            }
        }

        // Add mountain range around edges if enabled
        if (createMountainBoundary)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Calculate distance from edge
                    float distFromEdgeX = Mathf.Min(x, width - 1 - x);
                    float distFromEdgeZ = Mathf.Min(y, height - 1 - y);
                    float distFromEdge = Mathf.Min(distFromEdgeX, distFromEdgeZ);
                    
                    if (distFromEdge < mountainWidth)
                    {
                        // Mountain influence decreases as we move away from edge
                        float influence = 1 - (distFromEdge / mountainWidth);
                        
                        // Add some variation to mountain height
                        float noise = Mathf.PerlinNoise(x / mountainRoughness, y / mountainRoughness);
                        float mountainInfluence = mountainHeight * influence * (0.8f + 0.4f * noise);
                        
                        // Combine with existing height
                        heights[x, y] = Mathf.Max(heights[x, y], mountainInfluence);
                    }
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void CreateHoleWithFlag()
    {
        float centerX = width / 2;
        float centerZ = height / 2;

        // Random angle and distance from center
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distanceFromCenter = Random.Range(
            minDistanceFromCenter * Mathf.Min(width, height),
            maxDistanceFromCenter * Mathf.Min(width, height)
        );

        // Calculate position with random offset from center
        float holeX = centerX + Mathf.Cos(angle) * distanceFromCenter;
        float holeZ = centerZ + Mathf.Sin(angle) * distanceFromCenter;

        // Ensure the hole is within bounds
        holeX = Mathf.Clamp(holeX, width * 0.1f, width * 0.9f);
        holeZ = Mathf.Clamp(holeZ, height * 0.1f, height * 0.9f);

        // Convert to world position
        Vector3 holePosition = terrain.transform.position + new Vector3(holeX, 0, holeZ);

        RaycastHit hit;
        if (Physics.Raycast(holePosition + Vector3.up * 100, Vector3.down, out hit, 1000f) &&
            hit.collider.GetComponent<Terrain>() != null)
        {
            holePosition = hit.point;
        }

        // Instantiate hole at the exact ground height
        GameObject hole = Instantiate(holePrefab, holePosition, Quaternion.identity);

        // Create the trigger detection zone
        GameObject triggerZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        triggerZone.transform.position = holePosition + new Vector3(0, -0.2f, 0);
        triggerZone.transform.localScale = new Vector3(0.6f, 0.1f, 0.6f);
        triggerZone.GetComponent<Renderer>().enabled = false; // Hide it
        triggerZone.GetComponent<Collider>().isTrigger = true;
        triggerZone.name = "HoleTrigger";
        triggerZone.transform.parent = hole.transform;

        triggerZone.AddComponent<HoleSink>();

        CreateSkyIndicator(holePosition);
    }

    void CreateSkyIndicator(Vector3 holePosition)
    {
        if (skyIndicatorPrefab != null)
        {
            GameObject indicator = Instantiate(skyIndicatorPrefab,
                                             holePosition + new Vector3(0, 2f, 0),
                                             Quaternion.identity);
            indicator.transform.localScale = new Vector3(1f, skyIndicatorHeight, 1f);
        }
        else
        {
            // If no prefab provided, create a simple line renderer
            GameObject indicator = new GameObject("HoleIndicator");
            indicator.transform.position = holePosition;

            LineRenderer lineRenderer = indicator.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, holePosition + new Vector3(0, 1f, 0));
            lineRenderer.SetPosition(1, holePosition + new Vector3(0, skyIndicatorHeight, 0));
            lineRenderer.startWidth = 0.5f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = skyIndicatorColor;
            lineRenderer.endColor = new Color(skyIndicatorColor.r, skyIndicatorColor.g, skyIndicatorColor.b, 0.2f);
        }
    }

    void GenerateTrees()
    {
        // Get hole position to prevent trees from spawning too close
        Vector3 holePos = Vector3.zero;
        GameObject hole = GameObject.FindGameObjectWithTag("Hole");
        if (hole != null)
        {
            holePos = hole.transform.position;
        }

        for (int i = 0; i < treesCount; i++)
        {
            float x = Random.Range(0, terrain.terrainData.size.x);
            float z = Random.Range(0, terrain.terrainData.size.z);

            Vector3 worldPos = terrain.transform.position + new Vector3(x, 0, z);

            // Skip if too close to the hole (minimum 3 units away)
            if (hole != null && Vector3.Distance(new Vector3(worldPos.x, 0, worldPos.z), new Vector3(holePos.x, 0, holePos.z)) < 3f)
            {
                continue;
            }

            int randomIndex = Random.Range(0, treePrefabs.Length);
            GameObject selectedPrefab = treePrefabs[randomIndex];

            // Use raycast to place tree on the terrain surface
            RaycastHit hit;
            if (Physics.Raycast(worldPos + Vector3.up * 100, Vector3.down, out hit, 1000f) &&
                hit.collider.GetComponent<Terrain>() != null)
            {
                // Instantiate tree at the exact hit point on terrain
                GameObject tree = Instantiate(selectedPrefab, hit.point,
                    Quaternion.Euler(0, Random.Range(0, 360), 0));

                // Apply small random scale variation with increased height
                float widthScale = Random.Range(0.8f, 1.2f);
                float heightScale = Random.Range(0.9f, 1.3f) * treeHeightMultiplier;
                
                // Apply different scale factors to x, y, and z
                tree.transform.localScale = new Vector3(
                    tree.transform.localScale.x * widthScale,
                    tree.transform.localScale.y * heightScale, 
                    tree.transform.localScale.z * widthScale
                );
            }
        }
    }
    
    void CreateOutOfBoundsDetector()
    {
        GameObject detector = new GameObject("OutOfBoundsDetector");
        OutOfBoundsReset resetScript = detector.AddComponent<OutOfBoundsReset>();
        resetScript.minYPosition = outOfBoundsY;
    }
}