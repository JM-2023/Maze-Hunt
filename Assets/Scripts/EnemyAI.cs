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
    private bool isPerformingBite = false; // To prevent multiple bites overlapping

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
                Debug.LogWarning("EnemyAI: Player not assigned and no Player tag found.");
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
    }

    void Update()
    {
        if (player == null || agent == null) return;

        // If we are performing a bite, do not update movement or run logic.
        if (isPerformingBite) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update timers for run and cooldown
        if (runTimer > 0f)
        {
            runTimer -= Time.deltaTime;
            if (runTimer <= 0f)
            {
                runCooldownTimer = runCooldown;
                runTimer = 0f;
                StartWalking();
            }
        }

        if (runCooldownTimer > 0f)
        {
            runCooldownTimer -= Time.deltaTime;
        }

        agent.SetDestination(player.position);

        if (distanceToPlayer <= biteDistance)
        {
            // Start the bite if not already biting
            if (!isBiting && !isPerformingBite)
            {
                StartCoroutine(PerformBite());
            }
        }
        else
        {
            // If not biting, handle run/walk logic
            StopBiting();
            
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

        // Handle stopping distance
        if (distanceToPlayer <= stoppingDistance && !isBiting && !isPerformingBite)
        {
            agent.isStopped = true;
        }
        else if (!isBiting && !isPerformingBite)
        {
            agent.isStopped = false;
        }
    }

    private IEnumerator PerformBite()
    {
        isPerformingBite = true;
        isBiting = true;

        // Stop movement completely while biting
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        // Play bite animation and sound
        if (anim != null)
        {
            anim.SetBool("IsBiting", true);
            anim.SetFloat("Speed", 0f);
        }

        if (bitingSounds != null)
        {
            bitingSounds.Play();
        }

        // Wait the delay before damage application
        float delayBeforeDamage = 0.0f;
        yield return new WaitForSeconds(delayBeforeDamage);

        // Check if player is still in range for damage
        if (player != null && Vector3.Distance(transform.position, player.position) <= biteDistance)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                yield return new WaitForSeconds(0.24f);
                playerHealth.TakeDamage(1);
            }
        }

        // Optionally, wait until bite animation finishes before ending the bite.
        // If you know the bite animation length, you can yield for that duration or wait for an animation event.
        float biteAnimationLength = 2.4f; // Adjust this according to your animation length
        yield return new WaitForSeconds(biteAnimationLength - delayBeforeDamage);

        // Bite action finished
        if (anim != null)
        {
            anim.SetBool("IsBiting", false);
        }

        // Allow movement again
        isBiting = false;
        isPerformingBite = false;
        agent.isStopped = false;
    }

    private void StopBiting()
    {
        // This is only for external interruptions or logic cleanup if needed
        // In this approach, we don't interrupt a bite once started, so we might leave this empty.
    }

    private void StartRunning()
    {
        if (!isRunning && !isPerformingBite)
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
