using System;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    private RectTransform rect;
    [SerializeField] private Vector2 offset = new Vector2(300,20);

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowToolTip(bool show, RectTransform targetRect)
    {
        if(show == false)
        {
            rect.position = targetRect.position;
            return;
        }
        UpdatePosition(targetRect);
    }
        
    private void UpdatePosition(RectTransform targetRect)
    {

        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        float screenBottom = 0f;

        Vector2 targetPosition = targetRect.position;

        targetPosition.x = targetPosition.x > screenCenterX ? targetPosition.x - offset.x : targetPosition.x + offset.x;

        float vectorHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + vectorHalf;
        float bottomY = targetPosition.y - vectorHalf;
        
        if(topY > screenTop)
            targetPosition.y = screenTop - vectorHalf;
        else if (bottomY < screenBottom)
            targetPosition.y = screenBottom - vectorHalf - offset.y;

        rect.position = targetRect.position;

    }
}
