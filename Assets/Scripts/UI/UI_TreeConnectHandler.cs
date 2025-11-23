using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class UI_TreeConnectDetails
{
    public UI_TreeConnectHandler childNode;
    public NodeDirectionType direction;
    [Range(100f, 350f)] public float length = 150f;
    [Range(-50f, 50f)] public float rotation;
}
[ExecuteAlways]
public class UI_TreeConnectHandler : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;
    [SerializeField] private UI_TreeConnection[] connections;

    private Vector2 lastTargetPosition;
    private Image connectionImage;
    private Color originalColor;

    private void Awake()
    {
        if (connectionImage != null)
            originalColor = connectionImage.color;
    }

    public UI_TreeNode[] GetChildNodes()
    {
        List<UI_TreeNode> childToReturn = new List<UI_TreeNode>();

        if (connectionDetails == null) return childToReturn.ToArray();

        foreach (var node in connectionDetails)
        {
            if (node?.childNode != null)
                childToReturn.Add(node.childNode.GetComponent<UI_TreeNode>());
        }
        return childToReturn.ToArray();
    }

    public void UpdateConnections(bool reorderHierarchy = true)
    {
        // Validate arrays exist and have matching lengths
        if (connectionDetails == null || connections == null)
            return;

        if (connectionDetails.Length != connections.Length)
        {
            Debug.LogError($"Array length mismatch in {gameObject.name}: connectionDetails={connectionDetails.Length}, connections={connections.Length}");
            return;
        }

        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = connections[i];

            // Skip if either detail or connection is null
            if (detail == null || connection == null)
            {
                if (connection == null)
                    Debug.LogWarning($"Connection at index {i} is null in {gameObject.name}");
                continue;
            }

            Vector2 targetPosition = connection.GetConnectionPoint(rect);
            Image connectionImage = connection.GetConnectionImage();

            connection.DirectConnection(detail.direction, detail.length, detail.rotation);

            if (detail.childNode == null)
                continue;

            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionImage(connectionImage);

            if (reorderHierarchy)
                detail.childNode.transform.SetAsLastSibling();
        }
    }

    public void UpdateAllConnections()
    {
        UpdateConnections();

        if (connectionDetails == null) return;

        foreach (var node in connectionDetails)
        {
            if (node?.childNode == null) continue;
            node.childNode.UpdateConnections();
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null)
            return;

        connectionImage.color = unlocked ? Color.white : originalColor;
    }
    public void SetConnectionImage(Image image) => connectionImage = image;
    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;

    private void OnValidate()
    {
        // Only run validation if we have valid arrays
        if (connectionDetails == null || connectionDetails.Length <= 0)
            return;

        if (connections == null || connectionDetails.Length != connections.Length)
        {
            Debug.LogWarning($"Connection arrays need to be synchronized in {gameObject.name}");
            return;
        }

        // Only update connections if all connections are assigned
        bool allConnectionsValid = true;
        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] == null)
            {
                Debug.LogWarning($"Connection at index {i} is not assigned in {gameObject.name}");
                allConnectionsValid = false;
            }
        }

        if (allConnectionsValid)
        {
            UpdateConnections(false); // Don't reorder hierarchy during validation
        }
    }
}