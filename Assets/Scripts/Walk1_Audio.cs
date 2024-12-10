using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Walk1_Audio : MonoBehaviour
{
    private AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();

        if (aud == null)
        {
            Debug.LogWarning("Walk1_Audio: No AudioSource found on this GameObject. Ensure an AudioSource is present.");
        }
    }

    public void PlaySound()
    {
        if (aud != null && aud.clip != null)
        {
            aud.Play();
        }
        else
        {
            Debug.LogWarning("Walk1_Audio: Unable to play sound. AudioSource or AudioClip missing.");
        }
    }
}
