using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UI_SkillTreeNavigator_ConnectionBased : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private float navigationDelay = 0.2f;
    [SerializeField] private float stickDeadzone = 0.5f;
    [SerializeField] private float maxSearchDistance = 500f;

    [Header("References")]
    [SerializeField] private RectTransform selectionFrame;
    [SerializeField] private UI_TreeNode[] allNodes;

    private UI_TreeNode currentNode;
    private PlayerInputSet input;
    private float navigationTimer;

    private void Start()
    {
        input = new PlayerInputSet();
        input.Enable();

        if (allNodes == null || allNodes.Length == 0)
            allNodes = FindObjectsByType<UI_TreeNode>(FindObjectsSortMode.None);

        if (allNodes.Length > 0)
            SelectNode(allNodes[0]);
    }

    private void Update()
    {
        if (currentNode == null) return;

        navigationTimer -= Time.deltaTime;

        Vector2 stickInput = input.PlayerCharacter.Movement.ReadValue<Vector2>();

        if (stickInput.magnitude > stickDeadzone && navigationTimer <= 0f)
        {
            Navigate(stickInput);
            navigationTimer = navigationDelay;
        }
    }

    private void Navigate(Vector2 direction)
    {
        UI_TreeNode nextNode = FindBestNodeInDirection(direction);

        if (nextNode != null)
        {
            SelectNode(nextNode);
        }
    }

    private UI_TreeNode FindBestNodeInDirection(Vector2 direction)
    {
        UI_TreeNode bestMatch = null;
        float bestScore = float.MaxValue;

        Vector2 currentPos = currentNode.transform.position;

        foreach (var node in allNodes)
        {
            if (node == null || node == currentNode) continue;

            Vector2 nodePos = node.transform.position;
            Vector2 toNode = (nodePos - currentPos).normalized;
            float distance = Vector2.Distance(currentPos, nodePos);

            // Skip if too far
            if (distance > maxSearchDistance) continue;

            float alignment = Vector2.Dot(direction.normalized, toNode);

            // Must be in the right direction (forward hemisphere)
            if (alignment > 0.5f)
            {
                float score = distance / alignment;

                if (score < bestScore)
                {
                    bestScore = score;
                    bestMatch = node;
                }
            }
        }

        return bestMatch;
    }

    private void SelectNode(UI_TreeNode node)
    {
        currentNode = node;
        if (selectionFrame != null)
        {
            selectionFrame.position = node.transform.position;
        }
    }

    private void OnDestroy()
    {
        input?.Disable();
    }
}