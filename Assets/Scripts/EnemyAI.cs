using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player; 
    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource bitingSounds;

    [Header("Movement Settings")]
    public float walkSpeed = 1.2f;
    public float runSpeed = 3.4f;
    public float runDistance = 10f;      
    public float biteDistance = 1.5f;
    public float stoppingDistance = 1.5f;

    [Header("Behavior Settings")]
    public float runDuration = 5f;       
    public float runCooldown = 10f;      

    private float runTimer = 0f;         
    private float runCooldownTimer = 0f; 

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
        if (player == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update timers
        if (runTimer > 0f)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0f)
            {
                // Run ended, start cooldown
                runCooldownTimer = runCooldown;
                runTimer = 0f;
                StartWalking();
            }
        }

        if (runCooldownTimer > 0f)
        {
            runCooldownTimer -= Time.deltaTime;
        }

        // Set the destination every frame
        agent.SetDestination(player.position);

        if (distanceToPlayer <= biteDistance)
        {
            StartBiting();
        }
        else
        {
            StopBiting();
            
            // If not currently running, consider running if conditions are met
            if (!isRunning && runCooldownTimer <= 0f && distanceToPlayer <= runDistance)
            {
                StartRunning();
                runTimer = runDuration;
            }
            else if (!isRunning)
            {
                StartWalking();
            }
        }

        // Handle stopping distance logic if not biting
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

            // Start the delayed damage coroutine
            StartCoroutine(DelayedDamage(0.3f));
        }
    }

    private IEnumerator DelayedDamage(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Deal damage to the player after the delay
        if (player != null)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // This will cause the player to take damage and play the hurt sound
                playerHealth.TakeDamage(1);
            }
        }
    }

    private void StopBiting()
    {
        if (isBiting)
        {
            isBiting = false;
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
