using System;
using UnityEngine;

[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length;
}
[ExecuteAlways]

public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private void OnValidate()
    {
        if (connectionDetails.Length <= 0)
            return;

        if (connectionDetails.Length != connections.Length)
        {
            Debug.LogWarning($"[UI_TreeConnectHandler] Details length {connectionDetails.Length} is not equal to connection length {connections.Length}. Adjusting connection array size.");
            return;
        }

        UpdateConnection();
    }

    private void UpdateConnection()
    {
        for(int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var conn = connections[i];
            Vector2 targetPosition = conn.GetChildNodeConnectionPoint(rect);

            conn.DirectConnection(detail.direction, detail.length);
            detail.childNode?.SetPosition(targetPosition);
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;
}
