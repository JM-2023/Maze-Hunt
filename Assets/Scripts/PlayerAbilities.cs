using UnityEngine;
using TMPro;
using StarterAssets;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Flash Skill Settings")]
    public float flashDistance = 5f;          // How far the player will flash forward
    public float flashCooldown = 10f;         // Cooldown in seconds
    public TextMeshProUGUI cooldownText;      // Assign this in the inspector

    [Header("Audio")]
    public AudioClip flashSound;              // Assign a flash sound clip in the Inspector

    private float currentCooldownTime = 0f;
    private bool isOnCooldown = false;

    private ThirdPersonController playerController;
    private StarterAssetsInputs input;
    private AudioSource audioSource;

    void Start()
    {
        playerController = GetComponent<ThirdPersonController>();
        input = GetComponent<StarterAssetsInputs>();

        // Get or add an AudioSource component for playing sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateCooldownUI();
    }

    void Update()
    {
        HandleCooldown();
        CheckForFlashInput();
    }

    void HandleCooldown()
    {
        if (isOnCooldown)
        {
            currentCooldownTime -= Time.deltaTime;
            if (currentCooldownTime <= 0f)
            {
                currentCooldownTime = 0f;
                isOnCooldown = false;
            }
            UpdateCooldownUI();
        }
    }

    void CheckForFlashInput()
    {
        // If you prefer using StarterAssetsInputs, you can define a new action.
        // For simplicity, we check directly with Input.GetKeyDown here.
        if (Input.GetKeyDown(KeyCode.F) && !isOnCooldown && playerController != null)
        {
            PerformFlash();
            StartCooldown();
        }
    }

    void PerformFlash()
    {
        Vector3 forward = transform.forward;
        float actualFlashDistance = flashDistance;

        // Play flash sound
        if (flashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(flashSound);
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc)
        {
            cc.enabled = false;
            transform.position += forward * actualFlashDistance;
            cc.enabled = true;
        }
        else
        {
            transform.position += forward * actualFlashDistance;
        }


    }

    void StartCooldown()
    {
        currentCooldownTime = flashCooldown;
        isOnCooldown = true;
        UpdateCooldownUI();
    }

    void UpdateCooldownUI()
    {
        if (cooldownText != null)
        {
            if (isOnCooldown)
            {
                // Display the remaining cooldown time, rounded to one decimal
                cooldownText.text = $"CD: {currentCooldownTime:F1}s";
            }
            else
            {
                cooldownText.text = "Press F";
            }
        }
    }
}
