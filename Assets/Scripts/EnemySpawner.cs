using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject player;              // Assign your player gameobject in the inspector
    public GameObject enemyPrefab;         // The enemy prefab to spawn
    public Transform enemiesParent;        // Optional: Assign a parent transform to keep the hierarchy tidy

    [Header("Spawn Settings")]
    public int maxEnemies = 3;             // Maximum number of enemies that can be on the map
    public float initialSpawnDelay = 5f;   // Time after start before the first enemy spawns
    public float spawnInterval = 30f;      // Time between subsequent enemy spawns
    public float minSpawnDistance = 10f;   // Minimum distance from the player to spawn enemies
    public float maxSpawnDistance = 50f;   // Maximum distance from the player to spawn enemies

    // Internally track spawned enemies
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float spawnTimer;
    private int enemiesSpawnedCount = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Initialize timer so first spawn occurs after initialSpawnDelay
        spawnTimer = initialSpawnDelay;
    }

    void Update()
    {
        // If we already spawned the max enemies, do nothing
        if (enemiesSpawnedCount >= maxEnemies) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            enemiesSpawnedCount++;

            // Reset spawn timer for the next enemy
            if (enemiesSpawnedCount < maxEnemies)
            {
                spawnTimer = spawnInterval;
            }
        }
    }

    void SpawnEnemy()
    {
        // Find all floors
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        if (floors.Length == 0)
        {
            Debug.LogWarning("No floor objects found to spawn enemies on!");
            return;
        }

        // We will try multiple times to find a valid spawn point
        int attempts = 0;
        bool spawned = false;
        while (attempts < 50 && !spawned)
        {
            attempts++;

            // Pick a random floor
            GameObject chosenFloor = floors[Random.Range(0, floors.Length)];
            Vector3 spawnPosition = chosenFloor.transform.position + Vector3.up * 1f;

            float dist = Vector3.Distance(spawnPosition, player.transform.position);
            // Check if it's within the allowed spawn distance
            if (dist >= minSpawnDistance && dist <= maxSpawnDistance)
            {
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemiesParent);
                spawnedEnemies.Add(newEnemy);
                spawned = true;
            }
        }

        if (!spawned)
        {
            Debug.LogWarning("Failed to find a suitable spawn point for the enemy.");
        }
    }
}
