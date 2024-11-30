using UnityEngine;
using TMPro;

public class ExitDoor : MonoBehaviour
{
    [Header("Settings")]
    public bool isLocked = true;
    public int keysRequired = 5;
    public float openAngle = 90f;
    public float openSpeed = 90f;  // Changed to degrees per second
    public DoorHinge hingePosition = DoorHinge.Left;
    
    [Header("References")]
    public TextMeshProUGUI promptText;
    public GameObject doorModel;
    public Material lockedMaterial;
    public Material unlockedMaterial;
    
    [Header("Audio")]
    public AudioClip lockedSound;
    public AudioClip unlockSound;
    public AudioClip openSound;

    private KeyManager keyManager;
    private GameManager gameManager;
    private bool isOpening = false;
    private float currentAngle = 0f;
    private bool playerInRange = false;
    private AudioSource audioSource;
    private MeshRenderer meshRenderer;
    private Vector3 hingePivot;
    private bool hasWon = false;

    public enum DoorHinge
    {
        Left,
        Right
    }

    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();
        if (keyManager == null)
        {
            Debug.LogError("KeyManager not found!");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogWarning("GameManager not found!");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        meshRenderer = doorModel.GetComponent<MeshRenderer>();

        if (meshRenderer != null && lockedMaterial != null)
        {
            meshRenderer.material = lockedMaterial;
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }

        SetupHingePivot();
    }

    void SetupHingePivot()
    {
        if (doorModel != null)
        {
            Bounds bounds = doorModel.GetComponent<MeshRenderer>().bounds;
            
            if (hingePosition == DoorHinge.Left)
            {
                hingePivot = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z);
            }
            else
            {
                hingePivot = new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);
            }

            GameObject hinge = new GameObject("DoorHinge");
            hinge.transform.position = hingePivot;
            hinge.transform.rotation = transform.rotation;
            
            Transform originalParent = doorModel.transform.parent;
            hinge.transform.parent = originalParent;
            doorModel.transform.parent = hinge.transform;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }

        if (isOpening)
        {
            float targetAngle = hingePosition == DoorHinge.Left ? openAngle : -openAngle;
            
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, openSpeed * Time.deltaTime);
            doorModel.transform.parent.localRotation = Quaternion.Euler(0, currentAngle, 0);

            if (Mathf.Approximately(currentAngle, targetAngle))
            {
                isOpening = false;
                if (!hasWon && gameManager != null)
                {
                    hasWon = true;
                    gameManager.ShowWinScreen();
                }
            }
        }
    }

    void TryOpenDoor()
    {
        if (isLocked)
        {
            if (keyManager != null && keyManager.GetCollectedKeys() >= keysRequired)
            {
                UnlockDoor();
            }
            else
            {
                if (audioSource != null && lockedSound != null)
                {
                    audioSource.PlayOneShot(lockedSound);
                }
                UpdatePromptText($"Need {keysRequired} keys to unlock! ({keyManager?.GetCollectedKeys() ?? 0}/{keysRequired})");
            }
        }
        else if (!isOpening)
        {
            isOpening = true;
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
        if (audioSource != null && unlockSound != null)
        {
            audioSource.PlayOneShot(unlockSound);
        }
        
        if (meshRenderer != null && unlockedMaterial != null)
        {
            meshRenderer.material = unlockedMaterial;
        }
        
        UpdatePromptText("Press E to open the door");
    }

    void UpdatePromptText(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(true);
                UpdatePromptText(isLocked ? 
                    $"Need {keysRequired} keys to unlock! ({keyManager?.GetCollectedKeys() ?? 0}/{keysRequired})" : 
                    "Press E to open the door");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (doorModel != null)
        {
            Bounds bounds = doorModel.GetComponent<MeshRenderer>().bounds;
            Vector3 hinge = hingePosition == DoorHinge.Left ?
                new Vector3(bounds.min.x, bounds.center.y, bounds.center.z) :
                new Vector3(bounds.max.x, bounds.center.y, bounds.center.z);

            // Draw hinge position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(hinge, 0.1f);

            // Draw door open position
            Gizmos.color = Color.green;
            Vector3 direction = hingePosition == DoorHinge.Left ? 
                Quaternion.Euler(0, openAngle, 0) * Vector3.right :
                Quaternion.Euler(0, -openAngle, 0) * Vector3.right;
            Gizmos.DrawLine(hinge, hinge + direction * bounds.size.x);
        }
    }
}