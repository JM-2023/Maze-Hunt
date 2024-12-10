using UnityEngine;
using StarterAssets; // Assuming ThirdPersonController is in this namespace
using TMPro; // Import TextMeshPro namespace

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Speed Increase on Bite")]
    public float speedMultiplier = 2.0f;
    public float speedBoostDuration = 5.0f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 10f; 
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    [Header("Audio")]
    public AudioClip injuredSound;
    private AudioSource audioSource;

    [Header("UI (TextMeshPro)")]
    public TMP_Text heartsText; // Assign a TextMeshPro Text here

    private ThirdPersonController controller;
    private float normalSprintSpeed;
    private float speedBoostTimer = 0f;
    private bool isSpeedBoosted = false;

    private GameManager gameManager;

    void Start()
    {
        currentHealth = maxHealth;
        controller = GetComponent<ThirdPersonController>();

        if (controller != null)
        {
            normalSprintSpeed = controller.SprintSpeed;
        }
        else
        {
            Debug.LogWarning("PlayerHealth: No ThirdPersonController found on the player!");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("PlayerHealth: No AudioSource found on this GameObject. Please add one to play injured sounds.");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("PlayerHealth: GameManager not found in the scene.");
        }

        UpdateHeartsDisplay();
    }

    void Update()
    {
        // Handle speed boost duration
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            if (speedBoostTimer <= 0f)
            {
                ResetSpeed();
            }
        }

        // Handle invincibility duration
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        // If the player is currently invincible, do nothing
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player Health: " + currentHealth);

        // Play injured sound if available
        if (audioSource != null && injuredSound != null)
        {
            audioSource.PlayOneShot(injuredSound);
        }

        // Trigger invincibility
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;

        UpdateHeartsDisplay();

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
        else
        {
            // If the player is still alive, apply speed boost
            ApplySpeedBoost();
        }
    }

    void HandleDeath()
    {
        Debug.Log("Player Died!");
        // Show game over screen
        if (gameManager != null)
        {
            gameManager.ShowGameOverScreen();
        }
    }

    void ApplySpeedBoost()
    {
        if (controller == null) return;
        controller.SprintSpeed = normalSprintSpeed * speedMultiplier;
        speedBoostTimer = speedBoostDuration;
        isSpeedBoosted = true;
    }

    void ResetSpeed()
    {
        if (controller == null) return;
        controller.SprintSpeed = normalSprintSpeed;
        isSpeedBoosted = false;
    }

    void UpdateHeartsDisplay()
    {
        if (heartsText == null) return;

        // Create a string with one "♥" per health point
        string hearts = "";
        for (int i = 0; i < currentHealth; i++)
        {
            hearts += "♥";
        }

        heartsText.text = hearts;
    }
}
