using UnityEngine;
using UnityEngine.UI;

public class MazeMinimap : MonoBehaviour
{
    [Header("UI References")]
    public RawImage minimapImage;
    public RectTransform playerMarker;
    public CanvasGroup minimapCanvasGroup;
    public RectTransform minimapPanel;

    [Header("Minimap Settings")]
    public int textureSize = 512;
    public Color wallColor = Color.white;
    public Color floorColor = Color.black;
    public Color playerColor = Color.red;
    public float updateInterval = 0.1f;
    public KeyCode toggleKey = KeyCode.M;

    [Header("UI Settings")]
    [Range(0.05f, 0.5f)]
    public float minimapSizeRatio = 0.2f;
    public float padding = 20f;

    private Texture2D minimapTexture;
    private GameObject player;
    private float nextUpdateTime;
    private Camera mainCamera;
    private bool isVisible = false;
    private Vector2 minimapScale;
    private Vector2 minimapOffset;
    private Canvas parentCanvas;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure it has the 'Player' tag.");
            enabled = false;
            return;
        }

        mainCamera = Camera.main;
        parentCanvas = GetComponentInParent<Canvas>();
        
        if (parentCanvas == null)
        {
            Debug.LogError("Canvas not found! Make sure the minimap is placed under a Canvas.");
            enabled = false;
            return;
        }


        SetupMinimapTransform();

        GenerateMinimapTexture();
        
        minimapCanvasGroup.alpha = 0;
        minimapCanvasGroup.blocksRaycasts = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMinimap();
        }

        if (isVisible && Time.time >= nextUpdateTime)
        {
            UpdatePlayerMarkerPosition();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    void SetupMinimapTransform()
    {
        if (minimapPanel != null)
        {
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera || 
                parentCanvas.renderMode == RenderMode.WorldSpace)
            {
               
                screenWidth = parentCanvas.GetComponent<RectTransform>().rect.width;
                screenHeight = parentCanvas.GetComponent<RectTransform>().rect.height;
            }

           
            float mapSize = Mathf.Min(screenWidth, screenHeight) * minimapSizeRatio;

          
            minimapPanel.anchorMin = new Vector2(0, 1);
            minimapPanel.anchorMax = new Vector2(0, 1);
            minimapPanel.pivot = new Vector2(0, 1);

            
            minimapPanel.sizeDelta = new Vector2(mapSize, mapSize);
            minimapPanel.anchoredPosition = new Vector2(padding, -padding);

          
            minimapImage.rectTransform.anchorMin = Vector2.zero;
            minimapImage.rectTransform.anchorMax = Vector2.one;
            minimapImage.rectTransform.sizeDelta = Vector2.zero;
            minimapImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }


    void OnRectTransformDimensionsChange()
    {
        SetupMinimapTransform();
    }


    void GenerateMinimapTexture()
    {
        minimapTexture = new Texture2D(textureSize, textureSize);
        minimapTexture.filterMode = FilterMode.Point;

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

        // Find maze bounds
        Vector3 minBounds = Vector3.one * float.MaxValue;
        Vector3 maxBounds = Vector3.one * float.MinValue;

        foreach (GameObject wall in walls)
        {
            minBounds = Vector3.Min(minBounds, wall.transform.position);
            maxBounds = Vector3.Max(maxBounds, wall.transform.position);
        }
        foreach (GameObject floor in floors)
        {
            minBounds = Vector3.Min(minBounds, floor.transform.position);
            maxBounds = Vector3.Max(maxBounds, floor.transform.position);
        }

        // Calculate scale factors and offset for proper mapping
        Vector3 mazeSize = maxBounds - minBounds;
        float xScale = textureSize / mazeSize.x;
        float zScale = textureSize / mazeSize.z;
        float scale = Mathf.Min(xScale, zScale);

        // Store these for player position mapping
        minimapScale = new Vector2(scale, scale);
        minimapOffset = new Vector2(-minBounds.x, -minBounds.z);

        // Fill texture with floor color
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = floorColor;
        }

        // Draw walls
        foreach (GameObject wall in walls)
        {
            Vector3 worldPos = wall.transform.position;
            Vector2 mapPos = WorldToMinimapPosition(worldPos);
            
            int pixelX = Mathf.RoundToInt(mapPos.x);
            int pixelY = Mathf.RoundToInt(mapPos.y);

            // Draw wall pixels with thickness
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int px = pixelX + x;
                    int py = pixelY + y;
                    if (px >= 0 && px < textureSize && py >= 0 && py < textureSize)
                    {
                        pixels[py * textureSize + px] = wallColor;
                    }
                }
            }
        }

        minimapTexture.SetPixels(pixels);
        minimapTexture.Apply();
        minimapImage.texture = minimapTexture;
        
        // Update player marker color
        if (playerMarker.GetComponent<Image>() != null)
        {
            playerMarker.GetComponent<Image>().color = playerColor;
        }
    }

    Vector2 WorldToMinimapPosition(Vector3 worldPos)
    {
        // Convert world position to minimap position
        float x = (worldPos.x + minimapOffset.x) * minimapScale.x;
        float y = (worldPos.z + minimapOffset.y) * minimapScale.y;
        
        return new Vector2(x, y);
    }

    Vector2 WorldToMinimapUV(Vector3 worldPos)
    {
        Vector2 mapPos = WorldToMinimapPosition(worldPos);
        return new Vector2(
            mapPos.x / textureSize,
            mapPos.y / textureSize
        );
    }

    void UpdatePlayerMarkerPosition()
    {
        if (player == null) return;

        Vector2 uv = WorldToMinimapUV(player.transform.position);

       
        playerMarker.anchorMin = new Vector2(uv.x, uv.y);
        playerMarker.anchorMax = new Vector2(uv.x, uv.y);
        playerMarker.anchoredPosition = Vector2.zero;

        float rotation = mainCamera != null ? mainCamera.transform.eulerAngles.y : player.transform.eulerAngles.y;
        playerMarker.rotation = Quaternion.Euler(0, 0, -rotation);
    }

    void ToggleMinimap()
    {
        isVisible = !isVisible;
        minimapCanvasGroup.alpha = isVisible ? 1 : 0;
        minimapCanvasGroup.blocksRaycasts = isVisible;
    }
}