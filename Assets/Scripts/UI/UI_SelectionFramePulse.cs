using UnityEngine;

public class UI_SelectionFramePulse : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minScale = 1.0f;
    [SerializeField] private float maxScale = 1.15f;

    private RectTransform rect;
    private Vector3 originalScale;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        float scale = Mathf.Lerp(minScale, maxScale,
            (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f);

        rect.localScale = originalScale * scale;
    }

    private void OnDisable()
    {
        if (rect != null)
            rect.localScale = originalScale;
    }
}