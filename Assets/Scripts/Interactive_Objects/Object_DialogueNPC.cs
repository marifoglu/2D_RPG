using UnityEngine;


public class Object_DialogueNPC : Object_NPC
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueLineSO greetingDialogue;
    [SerializeField] private DialogueLineSO questAvailableDialogue;
    [SerializeField] private DialogueLineSO questTurnInDialogue;
    [SerializeField] private DialogueLineSO noQuestDialogue;

    [Header("Quest Settings")]
    [SerializeField] private QuestDataSO[] availableQuests;

    [Header("NPC Behavior")]
    [SerializeField] private bool canGiveQuests = true;
    [SerializeField] private bool canReceiveQuestTurnIns = true;

    private DialogueNPCData cachedDialogueData;

    protected override void Awake()
    {
        base.Awake();
        CacheDialogueData();
    }

    private void CacheDialogueData()
    {
        cachedDialogueData = new DialogueNPCData(
            npcID,
            npcName,
            rewardNpc,
            canGiveQuests ? availableQuests : null,
            canReceiveQuestTurnIns
        );
    }

    public override void Interact()
    {
        base.Interact();

        if (ui == null)
        {
            Debug.LogError($"Cannot interact with {npcName}: UI reference is null!");
            return;
        }

        // Determine which dialogue to play
        DialogueLineSO dialogueToPlay = DetermineDialogue();

        if (dialogueToPlay == null)
        {
            Debug.LogWarning($"{npcName} has no dialogue configured!");
            return;
        }

        // Update dialogue data before opening
        CacheDialogueData();

        ui.OpenDialogueUI(dialogueToPlay, cachedDialogueData);
    }

    private DialogueLineSO DetermineDialogue()
    {
        // Priority 1: Player can turn in quests
        if (CanTurnInQuests() && questTurnInDialogue != null)
        {
            return questTurnInDialogue;
        }

        // Priority 2: NPC has available quests for player
        if (HasAvailableQuests() && questAvailableDialogue != null)
        {
            return questAvailableDialogue;
        }

        // Priority 3: No quests - use default dialogue
        if (noQuestDialogue != null)
        {
            return noQuestDialogue;
        }

        // Fallback to greeting
        return greetingDialogue;
    }

    protected override bool HasAvailableQuests()
    {
        if (!canGiveQuests || availableQuests == null || questManager == null)
            return false;

        foreach (var quest in availableQuests)
        {
            if (quest == null) continue;

            // Skip if already active or completed
            if (questManager.QuestIsActive(quest) || questManager.QuestIsCompleted(quest))
                continue;

            // Skip if prerequisite not met
            if (quest.prerequisiteQuest != null && !questManager.QuestIsCompleted(quest.prerequisiteQuest))
                continue;

            return true;
        }

        return false;
    }

    protected override bool CanTurnInQuests()
    {
        if (!canReceiveQuestTurnIns || questManager == null)
            return false;

        return base.CanTurnInQuests();
    }

    public override DialogueNPCData GetDialogueData()
    {
        if (cachedDialogueData == null)
        {
            CacheDialogueData();
        }
        return cachedDialogueData;
    }

    public QuestDataSO[] GetAvailableQuests()
    {
        if (!canGiveQuests || availableQuests == null || questManager == null)
            return new QuestDataSO[0];

        var available = new System.Collections.Generic.List<QuestDataSO>();

        foreach (var quest in availableQuests)
        {
            if (quest == null) continue;

            if (questManager.QuestIsActive(quest) || questManager.QuestIsCompleted(quest))
                continue;

            if (quest.prerequisiteQuest != null && !questManager.QuestIsCompleted(quest.prerequisiteQuest))
                continue;

            available.Add(quest);
        }

        return available.ToArray();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        // Warn if no dialogue is set
        if (greetingDialogue == null && questAvailableDialogue == null &&
            questTurnInDialogue == null && noQuestDialogue == null)
        {
            Debug.LogWarning($"{gameObject.name}: No dialogue configured for this NPC!");
        }
    }
#endif
}