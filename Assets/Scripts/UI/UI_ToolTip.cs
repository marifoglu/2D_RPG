using System;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300, 20);

    [Header("Fixed Position (anchored)")]
    [SerializeField] private bool useFixedPosition = false;
    [SerializeField] private bool useInspectorAnchoredPosition = true;
    [SerializeField] private Vector2 fixedAnchoredPosition = new Vector2(200, 200);

    [Header("Canvas (optional)")]
    [SerializeField] private Canvas parentCanvas;

    private RectTransform canvasRect;
    private Camera canvasCamera;

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
            canvasCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera;
        }
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        // Safety check - if rect is null, try to get it again
        if (rect == null)
            rect = GetComponent<RectTransform>();

        // Use active state instead of moving offscreen
        gameObject.SetActive(show);

        if (!show || rect == null)
            return;

        if (parentCanvas == null || canvasRect == null)
        {
            // fallback: behave like previous implementation using screen positions
            UpdatePositionFallback(targetRect);
            return;
        }

        // Fixed anchored position mode (recommended for designer-set position)
        if (useFixedPosition)
        {
            if (useInspectorAnchoredPosition)
            {
                // Do nothing: keep the anchoredPosition you set in the inspector.
                // This prevents script from overriding your manual placement.
                return;
            }

            // Use the explicit value from the field
            rect.anchoredPosition = fixedAnchoredPosition;
            return;
        }

        if (targetRect == null)
        {
            rect.anchoredPosition = Vector2.zero;
            return;
        }

        // Convert the target world position -> screen point -> local point in canvas space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvasCamera, targetRect.position);

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, canvasCamera, out Vector2 localPoint))
        {
            // If conversion fails, use fallback
            UpdatePositionFallback(targetRect);
            return;
        }

        // Decide left/right side relative to screen center to keep tooltip on-screen
        bool placeToRight = screenPoint.x <= (Screen.width / 2f);

        // Apply horizontal offset (in canvas local space)
        localPoint.x += placeToRight ? offset.x : -offset.x;

        // Vertical clamp to canvas bounds (canvas local coordinates range from -half..half)
        float halfCanvasHeight = canvasRect.sizeDelta.y * 0.5f;
        float halfTooltipHeight = rect.sizeDelta.y * 0.5f;

        float topLimit = halfCanvasHeight - halfTooltipHeight - offset.y;
        float bottomLimit = -halfCanvasHeight + halfTooltipHeight + offset.y;

        localPoint.y = Mathf.Clamp(localPoint.y, bottomLimit, topLimit);

        rect.anchoredPosition = localPoint;
    }

    // Fallback that mirrors original screen-space logic when no canvas is available
    private void UpdatePositionFallback(RectTransform targetRect)
    {
        if (targetRect == null || rect == null)
            return;

        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0;

        Vector2 targetPosition = targetRect.position;

        targetPosition.x = targetPosition.x > screenCenterX ? targetPosition.x - offset.x : targetPosition.x + offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        if (topY > screenTop)
            targetPosition.y = screenTop - verticalHalf - offset.y;
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom + verticalHalf + offset.y;

        rect.position = targetPosition;
    }

    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}