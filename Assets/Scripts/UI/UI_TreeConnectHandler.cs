using System;
using UnityEngine;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
}

public class UI_TreeConnectHandler : MonoBehaviour
{
    [SerializeField] private UI_TreeConnectDetails[] details;
    [SerializeField] private UI_TreeConnection[] connection;
    private RectTransform rect;

    private void OnValidate()
    {
        if(rect == null)
            rect = GetComponent<RectTransform>();

        if (details.Length != connection.Length)
        {
            Debug.LogWarning($"[UI_TreeConnectHandler] Details length {details.Length} is not equal to connection length {connection.Length}. Adjusting connection array size.");
            return;
        }

        // Defer the update to avoid SendMessage during OnValidate
        if (Application.isPlaying)
        {
            UpdateConnection();
        }
        else
        {
            // In editor mode, schedule update for next frame
            UnityEditor.EditorApplication.delayCall += UpdateConnection;
        }
    }

    private void UpdateConnection()
    {
        for(int i = 0; i < details.Length; i++)
        {
            var detail = details[i];
            var conn = connection[i];
            Vector2 targetPosition = conn.GetChildNodeConnectionPoint(rect);

            conn.DirectConnection(detail.direction, detail.length);
            detail.childNode?.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
