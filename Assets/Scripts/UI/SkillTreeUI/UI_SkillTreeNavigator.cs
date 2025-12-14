using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class UI_SkillTreeNavigator : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private float navigationDelay = 0.2f;
    [SerializeField] private float stickDeadzone = 0.5f;
    [SerializeField] private float directionThreshold = 0.4f; // How strict the direction matching is (lower = stricter)
    [SerializeField] private float maxSearchDistance = 800f; // Maximum distance to search for nodes

    [Header("Visual Feedback")]
    [SerializeField] private GameObject selectionFrame;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightScale = 1.1f;
    [SerializeField] private Vector2 frameSizeOffset = Vector2.zero;
    [SerializeField] private Vector2 framePositionOffset = Vector2.zero;

    [Header("References")]
    [SerializeField] private UI_TreeNode startingNode;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip navigationSound;
    [SerializeField] private AudioClip selectionSound;
    [SerializeField] private AudioClip errorSound;

    private PlayerInputSet input;
    private UI ui;
    private UI_TreeNode currentNode;
    private Image selectionFrameImage;
    private RectTransform selectionFrameRect;
    private AudioSource audioSource;

    private float navigationTimer;
    private bool isNavigating;
    private bool isInitialized;
    private bool isUsingGamepad;

    private List<UI_TreeNode> allNodes = new List<UI_TreeNode>();

    private void Awake()
    {
        input = new PlayerInputSet();
        ui = GetComponentInParent<UI>();

        if (selectionFrame != null)
        {
            selectionFrameImage = selectionFrame.GetComponent<Image>();
            selectionFrameRect = selectionFrame.GetComponent<RectTransform>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (navigationSound != null || selectionSound != null || errorSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        RefreshNodeList();
    }

    private void OnEnable()
    {
        input.Enable();

        if (startingNode == null && allNodes.Count > 0)
        {
            startingNode = allNodes.Find(node => node != null && node.skillData != null && node.skillData.unlockedByDefault);
            if (startingNode == null)
                startingNode = allNodes[0];
        }

        StartCoroutine(InitialSelectionCo());
    }

    private IEnumerator InitialSelectionCo()
    {
        yield return new WaitForEndOfFrame();

        if (startingNode != null)
        {
            SelectNode(startingNode, false);
            isInitialized = true;
        }

        EnsureFrameOnTop();
    }

    private void OnDisable()
    {
        input.Disable();
        HideSelectionFrame();
        isInitialized = false;
        isUsingGamepad = false;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy || !isInitialized)
            return;

        DetectInputMethod();

        if (isUsingGamepad)
        {
            HandleGamepadNavigation();
            HandleGamepadSelection();
        }
        else
        {
            HandleMouseTracking();
        }
    }

    private void DetectInputMethod()
    {
        Vector2 gamepadInput = input.PlayerCharacter.Movement.ReadValue<Vector2>();
        bool gamepadButtonPressed = input.PlayerCharacter.Interaction.WasPressedThisFrame();

        if (gamepadInput.magnitude > 0.1f || gamepadButtonPressed)
        {
            if (!isUsingGamepad)
            {
                isUsingGamepad = true;
                ShowSelectionFrame(true);
            }
        }

        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            if (isUsingGamepad)
            {
                isUsingGamepad = false;
            }
        }
    }

    private void HandleMouseTracking()
    {
        UI_TreeNode hoveredNode = GetNodeUnderMouse();

        if (hoveredNode != null && hoveredNode != currentNode)
        {
            SelectNode(hoveredNode, true);
        }
    }

    private UI_TreeNode GetNodeUnderMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            UI_TreeNode node = result.gameObject.GetComponent<UI_TreeNode>();
            if (node != null)
                return node;

            node = result.gameObject.GetComponentInParent<UI_TreeNode>();
            if (node != null)
                return node;
        }

        return null;
    }

    private void HandleGamepadNavigation()
    {
        Vector2 navigationInput = input.PlayerCharacter.Movement.ReadValue<Vector2>();

        if (navigationInput.magnitude < stickDeadzone)
        {
            isNavigating = false;
            navigationTimer = 0f;
            return;
        }

        if (isNavigating && navigationTimer > 0f)
        {
            navigationTimer -= Time.unscaledDeltaTime;
            return;
        }

        NavigateToNextNode(navigationInput);
        navigationTimer = navigationDelay;
        isNavigating = true;
    }

    private void NavigateToNextNode(Vector2 direction)
    {
        if (currentNode == null)
            return;

        UI_TreeNode nextNode = FindBestNodeInDirection(direction);

        if (nextNode != null && nextNode != currentNode)
        {
            SelectNode(nextNode, true);
            PlaySound(navigationSound);
        }
    }

    private UI_TreeNode FindBestNodeInDirection(Vector2 direction)
    {
        if (currentNode == null || allNodes.Count == 0)
            return null;

        // Normalize the input direction
        direction = direction.normalized;

        Vector2 currentPos = currentNode.transform.position;

        UI_TreeNode bestNode = null;
        float bestScore = float.MaxValue;

        // Search through ALL nodes for the best match
        foreach (var candidate in allNodes)
        {
            // Skip the current node
            if (candidate == null || candidate == currentNode)
                continue;

            Vector2 candidatePos = candidate.transform.position;
            Vector2 toCandidate = candidatePos - currentPos;
            float distance = toCandidate.magnitude;

            // Skip nodes that are too far away
            if (distance > maxSearchDistance)
                continue;

            Vector2 directionToCandidate = toCandidate.normalized;

            // Calculate how well the candidate aligns with the desired direction
            // Dot product: 1 = same direction, 0 = perpendicular, -1 = opposite
            float alignment = Vector2.Dot(direction, directionToCandidate);

            // Only consider nodes that are generally in the right direction
            if (alignment < directionThreshold)
                continue;

            // Calculate a score that favors:
            // 1. Better alignment with the desired direction
            // 2. Closer distance
            // The formula: distance / alignment^2
            // - Squaring alignment makes direction matching more important
            // - Dividing distance by alignment means closer + better aligned = lower score (better)
            float alignmentWeight = alignment * alignment; // Square it to emphasize good alignment
            float score = distance / (alignmentWeight + 0.1f); // Add small value to avoid division by zero

            // Keep track of the best (lowest) score
            if (score < bestScore)
            {
                bestScore = score;
                bestNode = candidate;
            }
        }

        return bestNode;
    }

    private void SelectNode(UI_TreeNode node, bool showFrame)
    {
        if (node == null)
            return;

        currentNode = node;

        if (showFrame)
        {
            ShowSelectionFrame(true);
            UpdateSelectionFramePosition(node);
        }

        if (ui != null && ui.skillToolTip != null)
        {
            RectTransform nodeRectTransform = node.GetComponent<RectTransform>();
            if (nodeRectTransform != null)
            {
                ui.skillToolTip.ShowToolTip(true, nodeRectTransform, node.skillData, node);
            }
        }
    }

    private void UpdateSelectionFramePosition(UI_TreeNode node)
    {
        if (selectionFrame == null || selectionFrameRect == null || node == null)
            return;

        RectTransform nodeRect = node.GetComponent<RectTransform>();
        if (nodeRect == null)
            return;

        selectionFrameRect.position = nodeRect.position + (Vector3)framePositionOffset;

        Vector2 targetSize = nodeRect.sizeDelta * highlightScale + frameSizeOffset;
        selectionFrameRect.sizeDelta = targetSize;

        if (selectionFrameImage != null)
            selectionFrameImage.color = highlightColor;

        selectionFrameRect.SetAsLastSibling();
    }

    private void ShowSelectionFrame(bool show)
    {
        if (selectionFrame != null)
        {
            selectionFrame.SetActive(show);

            if (show && currentNode != null)
            {
                UpdateSelectionFramePosition(currentNode);
            }
        }
    }

    private void EnsureFrameOnTop()
    {
        if (selectionFrameRect != null)
        {
            selectionFrameRect.SetAsLastSibling();
        }
    }

    private void HandleGamepadSelection()
    {
        if (input.PlayerCharacter.Interaction.WasPressedThisFrame())
        {
            if (currentNode != null)
            {
                TryUnlockCurrentNode();
            }
        }
    }

    private void TryUnlockCurrentNode()
    {
        if (currentNode == null)
            return;

        // Use the new public method instead of simulating pointer events
        currentNode.TryUnlockFromGamepad();
        PlaySound(selectionSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void HideSelectionFrame()
    {
        ShowSelectionFrame(false);

        if (ui != null && ui.skillToolTip != null)
            ui.skillToolTip.ShowToolTip(false, null, null, null);
    }

    public void SetStartingNode(UI_TreeNode node)
    {
        startingNode = node;
        if (gameObject.activeInHierarchy && isInitialized)
            SelectNode(node, isUsingGamepad);
    }

    public void RefreshNodeList()
    {
        allNodes.Clear();
        UI_TreeNode[] foundNodes = GetComponentsInChildren<UI_TreeNode>(true);

        foreach (var node in foundNodes)
        {
            if (node != null && node.skillData != null)
                allNodes.Add(node);
        }
    }

    public void ForceUpdateFrame()
    {
        if (currentNode != null)
        {
            UpdateSelectionFramePosition(currentNode);
            EnsureFrameOnTop();
        }
    }
}