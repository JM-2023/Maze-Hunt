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

    [Header("Audio")]
    public AudioClip keyPickupClip;

    private List<GameObject> spawnedKeys = new List<GameObject>();
    private List<RectTransform> keyMarkers = new List<RectTransform>();
    private int collectedKeys = 0;
    private AudioSource audioSource;

    void Start()
    {
        // Check Prefabs
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

        // Set up AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
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
            if (i < spawnedKeys.Count && i < keyMarkers.Count)
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

        int keysPlaced = 0;
        int attempts = 0;
        // Try to place the required number of keys
        // If a chosen floor is not suitable (because it's blocked), try next floor.
        // Limit attempts to avoid infinite loops.
        while (keysPlaced < totalKeysNeeded && attempts < availableFloors.Count)
        {
            GameObject chosenFloor = availableFloors[attempts];
            Vector3 spawnPosition = chosenFloor.transform.position + Vector3.up * 0.5f;

            // Check if spawn position is inside a wall
            if (!IsPositionBlockedByWall(spawnPosition))
            {
                // Spawn the key
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

                keysPlaced++;
            }

            attempts++;
        }

        if (keysPlaced < totalKeysNeeded)
        {
            Debug.LogWarning("Not enough valid floors found to place all keys. Some keys may not have spawned.");
        }
    }

    bool IsPositionBlockedByWall(Vector3 position)
    {
        // Check for walls near this position
        // Adjust radius as needed based on your wall sizes
        float checkRadius = 0.4f;
        Collider[] hits = Physics.OverlapSphere(position, checkRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
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

            // Play key pickup sound
            if (keyPickupClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(keyPickupClip);
            }

            UpdateKeyCountUI();

            if (collectedKeys >= totalKeysNeeded)
            {
                ExitDoor exitDoor = FindObjectOfType<ExitDoor>();
                if (exitDoor != null)
                {
                    exitDoor.UnlockDoor();
                }
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

    public int GetCollectedKeys()
    {
        return collectedKeys;
    }
}
