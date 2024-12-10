using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;         // Assign the player's transform in the Inspector.
    private NavMeshAgent agent;
    private Animator anim;

    AudioSource bitingSounds;

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

        // Initial conditions
        agent.speed = walkSpeed;
        anim.SetFloat("Speed", walkSpeed);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsBiting", false);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
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
            // When we start biting, the enemy is no longer running.
            isRunning = false;

            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Stop movement immediately

            anim.SetBool("IsBiting", true);
            anim.SetBool("IsRunning", false);
            anim.SetFloat("Speed", 0f);

            bitingSounds.Play();
        }
    }

    private void StopBiting()
    {
        if (isBiting)
        {
            isBiting = false;
            // Reset isRunning here to allow the run animation to occur again if conditions are met.
            isRunning = false;

            agent.isStopped = false;
            anim.SetBool("IsBiting", false);
            bitingSounds.Stop();
        }
    }

    private void StartRunning()
    {
        if (!isRunning)
        {
            isRunning = true;
            agent.speed = runSpeed;
            anim.SetBool("IsRunning", true);
            anim.SetFloat("Speed", runSpeed);
        }
    }

    private void StartWalking()
    {
        if (isRunning)
        {
            isRunning = false;
            agent.speed = walkSpeed;
            anim.SetBool("IsRunning", false);
            anim.SetFloat("Speed", walkSpeed);
        }
    }
}
