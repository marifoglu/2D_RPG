using UnityEngine;
using UnityEngine.Rendering.Universal; // Needed for Light2D

public class FlickeringLight_Candle : MonoBehaviour
{
    private Light2D light2D;


    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float flickerSpeed = 2f;

    private float noiseOffset;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("FlickeringLight_Candle requires a Light2D component.");
        }

        // Random offset so each candle flickers differently
        noiseOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (light2D == null) return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseOffset);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        light2D.intensity = intensity;
    }
}
