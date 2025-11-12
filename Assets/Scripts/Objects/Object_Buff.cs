using System.Collections;
using UnityEngine;

public class Object_Buff : MonoBehaviour

{
    private SpriteRenderer sr;
 
    [Header("Buff Settings")]
    [SerializeField] private float buffDuration = 5.0f;
    [SerializeField] private bool canBeUsed = false;

    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canBeUsed == false)
            return; 
        // Start Coroutine or logic to extinguish the candle
        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear; // Make the buff invisible or indicate it's used
        Debug.Log("Buff Activated");  

        yield return new WaitForSeconds(duration);

        Debug.Log("Buff Ended");

        Destroy(gameObject);
    }
}
