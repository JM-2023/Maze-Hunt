using UnityEngine;
using StarterAssets;


public class HeartbeatManager : MonoBehaviour
{
    [Header("References")]
    public EnemyAI enemy;                // Assign the enemy object with EnemyAI in inspector
    public ThirdPersonController player; // Assign the player reference
    public AudioSource heartbeatSource;  // Assign an AudioSource with a heartbeat "thump"

    [Header("Heartbeat Settings")]
    public float baseRate = 60f;          // BPM when enemy is at max distance (just at trigger range)
    public float maxRate = 160f;          // BPM when enemy is extremely close
    public float closeTriggerDistance = 10f; // Distance within which heartbeat starts
    public float minDistanceForMaxRate = 1f; // Distance at which heartbeat is at maxRate (enemy basically on top of player)
    
    private float currentHeartRate;
    private float heartbeatTimer;

    void Start()
    {
        if (heartbeatSource == null)
        {
            Debug.LogError("HeartbeatManager: No AudioSource assigned!");
        }

        currentHeartRate = baseRate;
        heartbeatTimer = 0f;
    }

void Update()
{
    if (enemy == null || player == null)
    {
        // No references, no heartbeat
        return;
    }

    float distanceToEnemy = Vector3.Distance(player.transform.position, enemy.transform.position);

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
        // Play the heartbeat sound once
        if (heartbeatSource != null && heartbeatSource.clip != null)
        {
            heartbeatSource.PlayOneShot(heartbeatSource.clip);
        }
    }
}
