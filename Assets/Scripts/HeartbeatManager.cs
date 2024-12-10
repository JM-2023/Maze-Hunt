using UnityEngine;
using StarterAssets; // Assuming this is where ThirdPersonController is located
using System.Linq;

public class HeartbeatManager : MonoBehaviour
{
    [Header("References")]
    public ThirdPersonController player;      // Assign in inspector if possible.
    public AudioSource heartbeatSource;       // Assign an AudioSource with a heartbeat clip

    [Header("Heartbeat Settings")]
    public float baseRate = 60f;              // BPM at max distance (just at trigger range)
    public float maxRate = 160f;              // BPM at minimum distance
    public float closeTriggerDistance = 10f;  // Distance within which heartbeat starts
    public float minDistanceForMaxRate = 1f;  // Distance at which heartbeat is at maxRate

    private float currentHeartRate;
    private float heartbeatTimer = 0f;

    void Start()
    {
        // Attempt to find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<ThirdPersonController>();
                if (player == null)
                {
                    Debug.LogWarning("HeartbeatManager: Found 'Player' tagged object but no ThirdPersonController component.");
                }
            }
            else
            {
                Debug.LogWarning("HeartbeatManager: No object with 'Player' tag found.");
            }
        }

        if (heartbeatSource == null)
        {
            Debug.LogError("HeartbeatManager: No AudioSource assigned!");
        }

        currentHeartRate = baseRate;
    }

    void Update()
    {
        if (player == null || heartbeatSource == null || heartbeatSource.clip == null) return;

        // Find the closest enemy in range
        EnemyAI closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) 
        {
            // No enemies found
            return;
        }

        float distanceToEnemy = Vector3.Distance(player.transform.position, closestEnemy.transform.position);

        // If enemy is outside the close trigger distance, no heartbeat
        if (distanceToEnemy > closeTriggerDistance)
        {
            return;
        }

        // Enemy is close enough, calculate the heartbeat rate
        float normalizedDist = Mathf.InverseLerp(minDistanceForMaxRate, closeTriggerDistance, distanceToEnemy);
        currentHeartRate = Mathf.Lerp(maxRate, baseRate, normalizedDist);

        float secondsPerBeat = 60f / currentHeartRate;
        heartbeatTimer += Time.deltaTime;
        if (heartbeatTimer >= secondsPerBeat)
        {
            heartbeatTimer -= secondsPerBeat;
            PlayHeartbeat();
        }
    }

    void PlayHeartbeat()
    {
        if (heartbeatSource != null && heartbeatSource.clip != null)
        {
            heartbeatSource.PlayOneShot(heartbeatSource.clip);
        }
        else
        {
            Debug.LogWarning("HeartbeatManager: Cannot play heartbeat. AudioSource or clip missing.");
        }
    }

    EnemyAI FindClosestEnemy()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        if (enemies == null || enemies.Length == 0) return null;

        Vector3 playerPos = player.transform.position;

        // Find the enemy closest to the player
        EnemyAI closest = enemies
            .OrderBy(e => Vector3.Distance(playerPos, e.transform.position))
            .FirstOrDefault();

        return closest;
    }
}
