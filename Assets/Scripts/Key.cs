using UnityEngine;

public class Key : MonoBehaviour
{
    private KeyManager keyManager;
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    void Start()
    {
        keyManager = FindObjectOfType<KeyManager>();
        if (keyManager == null)
        {
            Debug.LogError("KeyManager not found in scene!");
        }
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate the key
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && keyManager != null)
        {
            keyManager.CollectKey(gameObject);
        }
    }
}