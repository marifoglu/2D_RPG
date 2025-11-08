using UnityEngine;
using UnityEngine.Rendering.Universal; // Needed for Light2D

public class FlickeringLight_Candle : MonoBehaviour
{
    private Light2D light2D;

    [Header("Flicker Settings")]
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float flickerSpeed = 0.1f;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
        if (light2D == null)
        {
            Debug.LogError("FlickeringLight_Candle requires a Light2D component.");
        }
    }

    private void Update()
    {
        if (light2D != null)
        {
            float randomIntensity = Random.Range(minIntensity, maxIntensity);
            light2D.intensity = Mathf.Lerp(light2D.intensity, randomIntensity, flickerSpeed * Time.deltaTime);
        }
    }
}
