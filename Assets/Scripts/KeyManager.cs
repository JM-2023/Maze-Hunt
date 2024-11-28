// KeyManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    [Header("Key Settings")]
    public GameObject keyPrefab;
    public int totalKeysNeeded = 5;
    public float keyRotationSpeed = 50f;
    
    [Header("Minimap Settings")]
    public GameObject keyMarkerPrefab;
    public MazeMinimap mazeMinimap;
    
    [Header("UI")]
    public TextMeshProUGUI keyCountText;
    public Color keyMarkerColor = Color.yellow;
    
    private List<GameObject> spawnedKeys = new List<GameObject>();
    private List<RectTransform> keyMarkers = new List<RectTransform>();
    private int collectedKeys = 0;

    void Start()
    {
        // Check
        if (keyPrefab == null)
        {
            Debug.LogError("Key Prefab is not assigned to KeyManager!");
            enabled = false;
            return;
        }

        if (keyMarkerPrefab == null)
        {
            Debug.LogError("Key Marker Prefab is not assigned to KeyManager!");
            enabled = false;
            return;
        }

        // If find no MazeMinimap，try to find it
        if (mazeMinimap == null)
        {
            mazeMinimap = FindObjectOfType<MazeMinimap>();
            if (mazeMinimap == null)
            {
                Debug.LogError("MazeMinimap not found in scene!");
                enabled = false;
                return;
            }
        }

        SpawnKeys();
        UpdateKeyCountUI();
    }

    void Update()
    {
        if (spawnedKeys == null || keyMarkers == null) return;
        
        // Update key marker positions
        for (int i = spawnedKeys.Count - 1; i >= 0; i--)
        {
            if (i < spawnedKeys.Count && i < keyMarkers.Count)  // double check
            {
                if (spawnedKeys[i] != null && keyMarkers[i] != null)
                {
                    UpdateKeyMarkerPosition(spawnedKeys[i].transform.position, keyMarkers[i]);
                }
            }
        }
    }

    void SpawnKeys()
    {
        if (mazeMinimap == null) return;

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        if (floors == null || floors.Length == 0)
        {
            Debug.LogError("No floor objects found with 'Floor' tag!");
            return;
        }

        List<GameObject> availableFloors = new List<GameObject>(floors);
        
        // Shuffle available floors
        for (int i = availableFloors.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = availableFloors[i];
            availableFloors[i] = availableFloors[randomIndex];
            availableFloors[randomIndex] = temp;
        }

        // Clear existing keys if any
        foreach (var key in spawnedKeys)
        {
            if (key != null) Destroy(key);
        }
        foreach (var marker in keyMarkers)
        {
            if (marker != null) Destroy(marker.gameObject);
        }
        spawnedKeys.Clear();
        keyMarkers.Clear();

        // Spawn new keys
        for (int i = 0; i < totalKeysNeeded && i < availableFloors.Count; i++)
        {
            // Spawn key
            Vector3 spawnPosition = availableFloors[i].transform.position + Vector3.up * 0.5f;
            GameObject key = Instantiate(keyPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0f, 360f), 0));
            spawnedKeys.Add(key);

            // Create marker
            GameObject markerObj = Instantiate(keyMarkerPrefab, mazeMinimap.minimapImage.transform);
            RectTransform markerRect = markerObj.GetComponent<RectTransform>();
            if (markerObj.TryGetComponent<Image>(out Image markerImage))
            {
                markerImage.color = keyMarkerColor;
            }
            keyMarkers.Add(markerRect);

            // Initial position update
            UpdateKeyMarkerPosition(spawnPosition, markerRect);
        }
    }

    void UpdateKeyMarkerPosition(Vector3 worldPos, RectTransform marker)
    {
        if (mazeMinimap == null || marker == null) return;

        Vector2 uv = mazeMinimap.WorldToMinimapUV(worldPos);
        marker.anchorMin = new Vector2(uv.x, uv.y);
        marker.anchorMax = new Vector2(uv.x, uv.y);
        marker.anchoredPosition = Vector2.zero;
        marker.pivot = new Vector2(0.5f, 0.5f);
    }

    public void CollectKey(GameObject key)
    {
        int keyIndex = spawnedKeys.IndexOf(key);
        if (keyIndex != -1 && keyIndex < keyMarkers.Count)
        {
            // Remove key and its marker
            if (keyMarkers[keyIndex] != null)
            {
                Destroy(keyMarkers[keyIndex].gameObject);
            }
            keyMarkers.RemoveAt(keyIndex);
            spawnedKeys.RemoveAt(keyIndex);
            Destroy(key);

            collectedKeys++;
            UpdateKeyCountUI();

            if (collectedKeys >= totalKeysNeeded)
            {
                Debug.Log("All keys collected! You win!");
                // Add your win condition handling here
            }
        }
    }

    void UpdateKeyCountUI()
    {
        if (keyCountText != null)
        {
            keyCountText.text = $"Keys: {collectedKeys}/{totalKeysNeeded}";
        }
    }
}