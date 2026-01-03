using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;
    private DialogueNPCData npcData;
    private Player_QuestManager questManager;

    [Header("Dialogue Display")]
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoiceText; // 4 choices available

    [Header("Typing Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private bool skipTypingOnClick = true;

    [Header("Visual Indicators")]
    [SerializeField] private GameObject continueIndicator;

    [Header("Dynamic Choice Settings")]
    [SerializeField] private string completeQuestText = "Complete Quest";
    [SerializeField] private string viewQuestsText = "View Quests";
    [SerializeField] private string leaveText = "Leave";
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color completeQuestColor = Color.green;

    private string fullTextToShow;
    private Coroutine typeTextCo;

    private DialogueLineSO currentLine;
    private DialogueLineSO[] currentChoices;
    private int selectedChoiceIndex;

    private bool waitingToConfirm;
    private bool canInteract;

    // Dynamic choices system
    private List<DynamicChoice> dynamicChoices = new List<DynamicChoice>();
    private bool usingDynamicChoices = false;

    private class DynamicChoice
    {
        public string text;
        public DialogueActionType action;
        public DialogueLineSO dialogueLine;
        public bool isHighlighted;
        public Color textColor;

        public DynamicChoice(string text, DialogueActionType action, DialogueLineSO line = null)
        {
            this.text = text;
            this.action = action;
            this.dialogueLine = line;
            this.isHighlighted = false;
            this.textColor = Color.white;
        }
    }

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
    }

    private void Start()
    {
        if (Player.instance != null)
        {
            questManager = Player.instance.questManager;
        }
    }

    public void SetupNpcData(DialogueNPCData npcData)
    {
        this.npcData = npcData;

        if (questManager == null && Player.instance != null)
        {
            questManager = Player.instance.questManager;
        }
    }

    public void PlayDialogueLine(DialogueLineSO line)
    {
        if (line == null)
        {
            Debug.LogError("Cannot play dialogue line: line is null!");
            CloseDialogue();
            return;
        }

        if (line.speaker == null)
        {
            Debug.LogError($"Cannot play dialogue line: speaker is null in line '{line.name}'!");
            CloseDialogue();
            return;
        }

        currentLine = line;
        currentChoices = line.choiceLines;
        canInteract = false;
        selectedChoiceIndex = 0;
        usingDynamicChoices = false;

        HideAllChoices();
        HideContinueIndicator();

        speakerPortrait.sprite = line.speaker.speakerPortrait;
        speakerName.text = line.speaker.speakerName;

        fullTextToShow = DetermineDialogueText(line);

        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
        }
        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));

        StartCoroutine(EnableInteractionCo());
    }

    private string DetermineDialogueText(DialogueLineSO line)
    {
        if (line.actionType != DialogueActionType.None &&
            line.actionType != DialogueActionType.PlayerMakeChoice)
        {
            return !string.IsNullOrEmpty(line.actionLine) ? line.actionLine : line.GetRandomLine();
        }

        return line.GetRandomLine();
    }

    private void HandleNextAction()
    {
        // Handle dynamic choice action
        if (usingDynamicChoices && selectedChoiceIndex < dynamicChoices.Count)
        {
            DynamicChoice selected = dynamicChoices[selectedChoiceIndex];
            ExecuteAction(selected.action, selected.dialogueLine);
            return;
        }

        // Handle regular dialogue action
        switch (currentLine.actionType)
        {
            case DialogueActionType.None:
                CloseDialogue();
                break;

            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;

            case DialogueActionType.PlayerMakeChoice:
                if (!usingDynamicChoices)
                {
                    ShowDynamicChoices();
                }
                else
                {
                    // Selection already made, handled above
                }
                break;

            case DialogueActionType.OpenQuest:
                OpenQuestUI();
                break;

            case DialogueActionType.GetQuestReward:
                CompleteQuests();
                break;

            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                ui.OpenCraftUI(true);
                break;

            case DialogueActionType.CloseDialogue:
                CloseDialogue();
                break;
        }
    }

    private void ExecuteAction(DialogueActionType action, DialogueLineSO nextLine = null)
    {
        switch (action)
        {
            case DialogueActionType.GetQuestReward:
                CompleteQuests();
                break;

            case DialogueActionType.OpenQuest:
                OpenQuestUI();
                break;

            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;

            case DialogueActionType.OpenCraft:
                ui.SwitchToInGameUI();
                ui.OpenCraftUI(true);
                break;

            case DialogueActionType.CloseDialogue:
                CloseDialogue();
                break;

            case DialogueActionType.PlayerMakeChoice:
                if (nextLine != null)
                {
                    PlayDialogueLine(nextLine);
                }
                break;

            default:
                if (nextLine != null)
                {
                    PlayDialogueLine(nextLine);
                }
                else
                {
                    CloseDialogue();
                }
                break;
        }
    }

    private void CompleteQuests()
    {
        ui.SwitchToInGameUI();

        if (npcData != null && !string.IsNullOrEmpty(npcData.npcID))
        {
            questManager?.TryGetRewardFromNpc(npcData.npcID);
        }
        else if (npcData != null)
        {
            questManager?.TryGetRewardFrom(npcData.npcRewardType);
        }
    }

    private void OpenQuestUI()
    {
        if (npcData != null && npcData.availableQuests != null)
        {
            var availableQuests = npcData.GetAcceptableQuests(questManager);
            if (availableQuests.Length > 0)
            {
                ui.SwitchToInGameUI();
                ui.OpenQuestUI(availableQuests);
            }
            else
            {
                CloseDialogue();
            }
        }
        else
        {
            CloseDialogue();
        }
    }

    public void DialogueInteraction()
    {
        if (!canInteract) return;

        if (typeTextCo != null)
        {
            if (skipTypingOnClick)
            {
                CompleteTyping();

                if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
                {
                    waitingToConfirm = true;
                    ShowContinueIndicator();
                }
                else
                {
                    HandleNextAction();
                }
            }
            return;
        }

        if (waitingToConfirm || usingDynamicChoices)
        {
            waitingToConfirm = false;
            HideContinueIndicator();
            HandleNextAction();
        }
    }

    private void CompleteTyping()
    {
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
            dialogueText.text = fullTextToShow;
            typeTextCo = null;
        }
    }

    private void ShowDynamicChoices()
    {
        dynamicChoices.Clear();
        usingDynamicChoices = true;
        selectedChoiceIndex = 0;

        // 1. Check for completable quests - "Complete Quest" option (GREEN)
        bool hasCompletableQuests = HasCompletableQuestsForNpc();
        if (hasCompletableQuests)
        {
            var choice = new DynamicChoice(GetCompleteQuestDisplayText(), DialogueActionType.GetQuestReward);
            choice.textColor = completeQuestColor;
            dynamicChoices.Add(choice);
        }

        // 2. Add choices from dialogue line (if any)
        if (currentChoices != null)
        {
            foreach (var choiceLine in currentChoices)
            {
                if (choiceLine == null) continue;

                // Skip GetQuestReward if we already added it dynamically
                if (choiceLine.actionType == DialogueActionType.GetQuestReward && hasCompletableQuests)
                    continue;

                // Check if choice should be shown
                if (!ShouldShowChoice(choiceLine))
                    continue;

                var choice = new DynamicChoice(choiceLine.playerChoiceAnswer, choiceLine.actionType, choiceLine);
                dynamicChoices.Add(choice);
            }
        }

        // 3. Check for available quests - "View Quests" option
        bool hasAvailableQuests = HasAvailableQuestsForNpc();
        bool questChoiceExists = dynamicChoices.Exists(c => c.action == DialogueActionType.OpenQuest);

        if (hasAvailableQuests && !questChoiceExists)
        {
            dynamicChoices.Add(new DynamicChoice(viewQuestsText, DialogueActionType.OpenQuest));
        }

        // 4. Always add "Leave" as last option
        bool leaveExists = dynamicChoices.Exists(c => c.action == DialogueActionType.CloseDialogue);
        if (!leaveExists)
        {
            dynamicChoices.Add(new DynamicChoice(leaveText, DialogueActionType.CloseDialogue));
        }

        // Display the choices
        DisplayDynamicChoices();
    }

    private string GetCompleteQuestDisplayText()
    {
        if (questManager == null || npcData == null) return completeQuestText;

        var completable = questManager.GetCompletableQuestsForNpc(npcData.npcID, npcData.npcRewardType);

        if (completable.Count == 0)
            return completeQuestText;

        if (completable.Count == 1)
            return $"{completeQuestText}: {completable[0].questDataSo.questName}";

        return $"{completeQuestText} ({completable.Count} quests)";
    }


    private bool HasCompletableQuestsForNpc()
    {
        if (questManager == null) return false;

        if (npcData != null)
        {
            if (!string.IsNullOrEmpty(npcData.npcID))
            {
                return questManager.HasCompletedQuestForNpc(npcData.npcID);
            }
            return questManager.HasCompletedQuestFor(npcData.npcRewardType);
        }

        return questManager.HasCompletedQuest();
    }

    private bool HasAvailableQuestsForNpc()
    {
        if (npcData == null || npcData.availableQuests == null || questManager == null)
            return false;

        return npcData.GetAcceptableQuests(questManager).Length > 0;
    }

    private void DisplayDynamicChoices()
    {
        HideAllChoices();

        for (int i = 0; i < dialogueChoiceText.Length && i < dynamicChoices.Count; i++)
        {
            DynamicChoice choice = dynamicChoices[i];
            TextMeshProUGUI choiceUI = dialogueChoiceText[i];

            choiceUI.gameObject.SetActive(true);

            string displayText = choice.text;
            bool isSelected = (i == selectedChoiceIndex);

            // Format based on selection and special colors
            if (isSelected)
            {
                if (choice.textColor == completeQuestColor)
                {
                    // Green highlight for complete quest
                    choiceUI.text = $"<color=#{ColorUtility.ToHtmlStringRGB(completeQuestColor)}>> {displayText}</color>";
                }
                else
                {
                    // Yellow highlight for normal selection
                    choiceUI.text = $"<color=#{ColorUtility.ToHtmlStringRGB(highlightColor)}>> {displayText}</color>";
                }
            }
            else
            {
                if (choice.textColor == completeQuestColor)
                {
                    // Green for complete quest (not selected)
                    choiceUI.text = $"<color=#{ColorUtility.ToHtmlStringRGB(completeQuestColor)}>  {displayText}</color>";
                }
                else
                {
                    // Normal white
                    choiceUI.text = $"  {displayText}";
                }
            }
        }
    }


    private void ShowChoicesLegacy()
    {
        if (currentChoices == null || currentChoices.Length == 0)
        {
            CloseDialogue();
            return;
        }

        int visibleChoices = 0;

        for (int i = 0; i < dialogueChoiceText.Length; i++)
        {
            if (i < currentChoices.Length)
            {
                DialogueLineSO choice = currentChoices[i];

                if (choice == null)
                {
                    dialogueChoiceText[i].gameObject.SetActive(false);
                    continue;
                }

                bool shouldShow = ShouldShowChoice(choice);
                dialogueChoiceText[i].gameObject.SetActive(shouldShow);

                if (shouldShow)
                {
                    string choiceText = choice.playerChoiceAnswer;

                    dialogueChoiceText[i].text = selectedChoiceIndex == i ?
                        $"<color=yellow>> {choiceText}</color>" :
                        $"  {choiceText}";

                    visibleChoices++;
                }
            }
            else
            {
                dialogueChoiceText[i].gameObject.SetActive(false);
            }
        }

        if (visibleChoices > 0 && !dialogueChoiceText[selectedChoiceIndex].gameObject.activeSelf)
        {
            for (int i = 0; i < dialogueChoiceText.Length; i++)
            {
                if (dialogueChoiceText[i].gameObject.activeSelf)
                {
                    selectedChoiceIndex = i;
                    ShowChoicesLegacy();
                    break;
                }
            }
        }
    }

    private bool ShouldShowChoice(DialogueLineSO choice)
    {
        if (choice.actionType == DialogueActionType.GetQuestReward)
        {
            return HasCompletableQuestsForNpc();
        }

        if (choice.actionType == DialogueActionType.OpenQuest)
        {
            return HasAvailableQuestsForNpc();
        }

        return true;
    }

    private void HideAllChoices()
    {
        foreach (var choiceText in dialogueChoiceText)
        {
            choiceText.gameObject.SetActive(false);
        }
    }

    public void NavigateChoice(int direction)
    {
        if (!usingDynamicChoices)
        {
            // Legacy navigation
            if (currentChoices == null || currentChoices.Length <= 1)
                return;

            selectedChoiceIndex += direction;
            selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length - 1);
            ShowChoicesLegacy();
            return;
        }

        // Dynamic choice navigation
        if (dynamicChoices.Count <= 1) return;

        selectedChoiceIndex += direction;

        // Wrap around
        if (selectedChoiceIndex < 0)
            selectedChoiceIndex = dynamicChoices.Count - 1;
        if (selectedChoiceIndex >= dynamicChoices.Count)
            selectedChoiceIndex = 0;

        DisplayDynamicChoices();
    }

    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        typeTextCo = null;

        if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
        {
            waitingToConfirm = true;
            ShowContinueIndicator();
        }
        else
        {
            yield return new WaitForSeconds(0.15f);
            HandleNextAction();
        }
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }

    private void ShowContinueIndicator()
    {
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(true);
        }
    }

    private void HideContinueIndicator()
    {
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(false);
        }
    }

    private void CloseDialogue()
    {
        var enemyDialogue = FindFirstObjectByType<Enemy_Dialogue>();

        usingDynamicChoices = false;
        dynamicChoices.Clear();

        if (ui != null)
        {
            ui.SwitchToInGameUI();
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (enemyDialogue != null && enemyDialogue.IsInDialogue)
        {
            enemyDialogue.EndDialogue();
        }
    }

    public void ForceClose()
    {
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
            typeTextCo = null;
        }

        CloseDialogue();
    }
}