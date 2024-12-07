using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;          // Reference to the player GameObject
    public float detectionRadius = 15f; // Radius for detecting the player
    public float attackRadius = 2f;    // Radius for triggering the attack
    public float runSpeed = 5f;        // Speed of the enemy when running

    [SerializeField]public Animator animator;         // Reference to the Animator component

    private NavMeshAgent agent;       // NavMeshAgent for navigation

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        agent.speed = runSpeed; // Set initial run speed
    }

    void Update()
    {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRadius)
        {
            AttackPlayer(); // Attack if in range
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer(); // Chase if in detection range
        }
        else
        {
            Idle(); // Stand still if out of range
        }

        UpdateAnimation(); // Update animations based on movement
    }

    void ChasePlayer()
    {
        // Set the destination to the player's position
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        // Stop moving and trigger the attack animation
        animator.SetTrigger("Attack");
    }

    void Idle()
    {
        // Stop the NavMeshAgent and ensure no animations are playing
        agent.ResetPath();
    }

    void UpdateAnimation()
    {
        // Determine if the enemy is moving
        bool isMoving = agent.velocity.magnitude > 0.1f;

        // Set animation parameters based on movement state
        animator.SetBool("IsRunning", isMoving && Vector3.Distance(transform.position, player.position) > attackRadius);
    }

    public void SetRunSpeed(float newSpeed)
    {
        // Allow dynamic change of the enemy's run speed
        runSpeed = newSpeed;
        agent.speed = newSpeed;
    }
}
