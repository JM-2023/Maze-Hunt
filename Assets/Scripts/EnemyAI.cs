using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;         // Assign the player's transform in the Inspector.
    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource audioSource; // AudioSource attached to the enemy

    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float runDistance = 10f;
    public float biteDistance = 2f;
    public float stoppingDistance = 1.5f;

    [Header("Audio Clips")]
    public AudioClip movementClip;   // Single looped sound for any movement (walk or run)
    public AudioClip biteClip;       // One-shot bite sound

    private bool isRunning = false;
    private bool isBiting = false;
    private bool isMovementSoundPlaying = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();


        // Initial conditions
        agent.speed = walkSpeed;
        anim.SetFloat("Speed", walkSpeed);
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsBiting", false);

        // Start moving sound by default since the enemy begins walking
        StartMovementSound();
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
            StopMovementSound();
        }
        else if (!isBiting)
        {
            agent.isStopped = false;
            // If not stopped, ensure movement sound is playing
            if (!isMovementSoundPlaying)
                StartMovementSound();
        }
    }

    private void StartBiting()
    {
        if (!isBiting)
        {
            isBiting = true;
            agent.isStopped = true;
            agent.velocity = Vector3.zero; // Stop movement immediately

            anim.SetBool("IsBiting", true);
            anim.SetBool("IsRunning", false);
            anim.SetFloat("Speed", 0f);

            // Stop movement sound when attacking
            StopMovementSound();

            // Play bite sound once
            PlayBiteSound();
        }
    }

    private void StopBiting()
    {
        if (isBiting)
        {
            isBiting = false;
            agent.isStopped = false;
            anim.SetBool("IsBiting", false);

            // After biting, if we should be moving, start the movement sound again
            if (isRunning || (!isRunning && !agent.isStopped))
            {
                StartMovementSound();
            }
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

            // Ensure movement sound is playing
            if (!isMovementSoundPlaying && !isBiting)
                StartMovementSound();
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

            // Ensure movement sound is playing
            if (!isMovementSoundPlaying && !isBiting && !agent.isStopped)
                StartMovementSound();
        }
        else
        {
            // If we are already walking and not biting or stopped, ensure movement sound is playing
            if (!isMovementSoundPlaying && !isBiting && !agent.isStopped)
                StartMovementSound();
        }
    }

    // Audio methods
    private void StartMovementSound()
    {
        if (audioSource == null || movementClip == null) return;
        if (isMovementSoundPlaying) return;

        // Stop whatever was playing and start movement sound
        audioSource.Stop();
        audioSource.clip = movementClip;
        audioSource.loop = true;
        audioSource.Play();
        isMovementSoundPlaying = true;
    }

    private void StopMovementSound()
    {
        if (audioSource == null) return;
        if (!isMovementSoundPlaying) return;

        audioSource.Stop();
        isMovementSoundPlaying = false;
    }

    private void PlayBiteSound()
    {
        if (audioSource == null || biteClip == null) return;

        // PlayOneShot doesn't interrupt the current looped sound since we stopped it already
        audioSource.PlayOneShot(biteClip);
    }
}
