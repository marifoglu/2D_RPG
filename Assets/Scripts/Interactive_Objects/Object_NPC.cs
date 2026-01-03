using UnityEngine;

public class Object_NPC : MonoBehaviour, IInteractable
{
    protected Transform player;
    protected UI ui;
    protected Player_QuestManager questManager;

    [Header("NPC Identity")]
    [SerializeField] protected string npcID;
    [SerializeField] protected string npcName;

    [Header("NPC Quest Settings")]
    [SerializeField] protected string npcTargetQuestID; // For tracking "talk to this NPC" objectives
    [SerializeField] protected RewardType rewardNpc;

    [Header("Visual Settings")]
    [SerializeField] protected Transform npc;
    [SerializeField] protected GameObject interactToolTip;
    [SerializeField] protected GameObject questAvailableIndicator; // "!" icon
    [SerializeField] protected GameObject questTurnInIndicator;    // "?" icon
    protected bool facingRight = true;

    [Header("Floating Settings")]
    [SerializeField] protected float floatSpeed = 8f;
    [SerializeField] protected float floatRange = 0.1f;
    protected Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();

        if (interactToolTip != null)
        {
            startPosition = interactToolTip.transform.position;
            interactToolTip.SetActive(false);
        }

        // Auto generate NPC ID if not set
        if (string.IsNullOrEmpty(npcID))
        {
            npcID = gameObject.name;
        }
    }

    protected virtual void Start()
    {
        if (Player.instance != null)
        {
            questManager = Player.instance.questManager;
        }

        UpdateQuestIndicators();
    }

    protected virtual void Update()
    {
        HandleNpcFlip();
        HandleToolTipFloat();
    }

    protected void HandleToolTipFloat()
    {
        if (interactToolTip != null && interactToolTip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactToolTip.transform.position = startPosition + new Vector3(0, yOffset);
        }
    }

    protected void HandleNpcFlip()
    {
        if (player == null || npc == null)
            return;

        if (player.position.x > npc.position.x && facingRight)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
        else if (player.position.x < npc.position.x && !facingRight)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        }
    }

    protected virtual void UpdateQuestIndicators()
    {
        if (questManager == null) return;

        // Show "!" if quests available
        if (questAvailableIndicator != null)
        {
            questAvailableIndicator.SetActive(HasAvailableQuests());
        }

        // Show "?" if can turn in quests
        if (questTurnInIndicator != null)
        {
            questTurnInIndicator.SetActive(CanTurnInQuests());
        }
    }

    protected virtual bool HasAvailableQuests()
    {
        return false;
    }

    protected virtual bool CanTurnInQuests()
    {
        if (questManager == null) return false;

        // Check by NPC ID
        if (!string.IsNullOrEmpty(npcID) && questManager.HasCompletedQuestForNpc(npcID))
            return true;

        // Check by reward type
        if (questManager.HasCompletedQuestFor(rewardNpc))
            return true;

        return false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = collision.transform;

        if (interactToolTip != null)
            interactToolTip.SetActive(true);

        UpdateQuestIndicators();
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        player = null;

        if (interactToolTip != null)
            interactToolTip.SetActive(false);
    }

    public virtual void Interact()
    {
        // Add progress for "talk to NPC" quests
        if (!string.IsNullOrEmpty(npcTargetQuestID))
        {
            questManager?.AddProgress(npcTargetQuestID);
        }

        // Also check by NPC ID for complex quests
        if (!string.IsNullOrEmpty(npcID))
        {
            questManager?.AddProgress(QuestObjectiveType.Talk, npcID);
        }

        UpdateQuestIndicators();
    }

    public virtual DialogueNPCData GetDialogueData()
    {
        return new DialogueNPCData(npcID, npcName, rewardNpc, null, true);
    }

    public string GetNpcID() => npcID;

    public string GetNpcName() => npcName;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        // Auto generate NPC ID from game object name if empty
        if (string.IsNullOrEmpty(npcID))
        {
            npcID = gameObject.name.Replace("(Clone)", "").Trim();
        }
    }
#endif
}