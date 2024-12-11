using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light keyLight;
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.2f;
    public float flickerSpeed = 5f;

    void Update()
    {
        if (keyLight != null)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
            keyLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        }
    }
}
