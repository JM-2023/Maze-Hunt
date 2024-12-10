using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Assign the player's transform in the Inspector if possible.
    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource bitingSounds;

    [Header("Movement Settings")]
    public float walkSpeed = 1.2f;
    public float runSpeed = 3.4f;
    public float runDistance = 10f;
    public float biteDistance = 1.5f;
    public float stoppingDistance = 1.5f;

    private bool isRunning = false;
    private bool isBiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        bitingSounds = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("EnemyAI: Player not assigned and no GameObject with tag 'Player' found.");
            }
        }

        if (agent == null)
        {
            Debug.LogError("EnemyAI: No NavMeshAgent found on this enemy.");
        }

        // Initial conditions
        if (anim != null)
        {
            agent.speed = walkSpeed;
            anim.SetFloat("Speed", walkSpeed);
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsBiting", false);
        }
        else
        {
            Debug.LogWarning("EnemyAI: No animator found on the enemy or its children.");
        }
    }

    void Update()
    {
        // If we have no player reference yet, no movement
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Set the destination every frame
        agent.SetDestination(player.position);

        // Check if we should be biting the player
        if (distanceToPlayer <= biteDistance)
        {
            StartBiting();
        }
        else
        {
            StopBiting();

            // Decide whether to run or walk
            if (distanceToPlayer <= runDistance)
            {
                StartRunning();
            }
            else
            {
                StartWalking();
            }
        }

        // Handle stopping distance logic
        if (distanceToPlayer <= stoppingDistance && !isBiting)
        {
            agent.isStopped = true;
        }
        else if (!isBiting)
        {
            agent.isStopped = false;
        }
    }

    private void StartBiting()
    {
        if (!isBiting)
        {
            isBiting = true;
            isRunning = false;

            if (agent != null)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero; // Stop movement immediately
            }

            if (anim != null)
            {
                anim.SetBool("IsBiting", true);
                anim.SetBool("IsRunning", false);
                anim.SetFloat("Speed", 0f);
            }

            if (bitingSounds != null)
            {
                bitingSounds.Play();
            }

            // Deal damage to the player
            if (player != null)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1); // Player loses 1 health per bite
                }
            }
        }
    }


    private void StopBiting()
    {
        if (isBiting)
        {
            isBiting = false;
            isRunning = false;

            if (agent != null)
            {
                agent.isStopped = false;
            }

            if (anim != null)
            {
                anim.SetBool("IsBiting", false);
            }

            if (bitingSounds != null && bitingSounds.isPlaying)
            {
                bitingSounds.Stop();
            }
        }
    }

    private void StartRunning()
    {
        if (!isRunning)
        {
            isRunning = true;
            if (agent != null) agent.speed = runSpeed;
            if (anim != null)
            {
                anim.SetBool("IsRunning", true);
                anim.SetFloat("Speed", runSpeed);
            }
        }
    }

    private void StartWalking()
    {
        if (isRunning)
        {
            isRunning = false;
            if (agent != null) agent.speed = walkSpeed;
            if (anim != null)
            {
                anim.SetBool("IsRunning", false);
                anim.SetFloat("Speed", walkSpeed);
            }
        }
    }
}
