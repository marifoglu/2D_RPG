using UnityEngine;

public class DebugConnections : MonoBehaviour
{
    [ContextMenu("Debug Current Node Connections")]
    public void DebugCurrentConnections()
    {
        UI_TreeNode currentNode = GetComponent<UI_TreeNode>();
        if (currentNode == null)
        {
            Debug.LogError("No UI_TreeNode found!");
            return;
        }

        Debug.Log($"=== DEBUGGING NODE: {currentNode.name} ===");

        // Check parent nodes
        Debug.Log($"PARENT NODES (neededNodes): {currentNode.neededNodes?.Length ?? 0}");
        if (currentNode.neededNodes != null)
        {
            foreach (var parent in currentNode.neededNodes)
            {
                if (parent != null)
                    Debug.Log($"  - {parent.name}");
            }
        }

        // Check child nodes
        UI_TreeConnectHandler handler = currentNode.GetComponent<UI_TreeConnectHandler>();
        if (handler != null)
        {
            UI_TreeNode[] children = handler.GetChildNodes();
            Debug.Log($"CHILD NODES (from handler): {children?.Length ?? 0}");
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null)
                        Debug.Log($"  - {child.name}");
                }
            }
        }
        else
        {
            Debug.Log("NO UI_TreeConnectHandler found!");
        }

        // Check conflict nodes
        Debug.Log($"CONFLICT NODES: {currentNode.conflictNodes?.Length ?? 0}");
        if (currentNode.conflictNodes != null)
        {
            foreach (var conflict in currentNode.conflictNodes)
            {
                if (conflict != null)
                    Debug.Log($"  - {conflict.name}");
            }
        }
    }
}