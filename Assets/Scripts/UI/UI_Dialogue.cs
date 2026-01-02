//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UI_Dialogue : MonoBehaviour
//{
//    private UI ui;
//    private DialogueNPCData npcData;
//    private Player_QuestManager questManager;

//    [SerializeField] private Image speakerPortrait;
//    [SerializeField] private TextMeshProUGUI speakerName;
//    [SerializeField] private TextMeshProUGUI dialogueText;
//    [SerializeField] private TextMeshProUGUI[] dialogueChoiceText;

//    [Space]
//    [SerializeField] private float textSpeed = 0.1f;
//    private string fullTextToShow;
//    private Coroutine typeTextCo;

//    private DialogueLineSO currentLine;
//    private DialogueLineSO[] currentChoices;
//    private DialogueLineSO selectedChoice;
//    private int selectedChoiceIndex;

//    private bool waitingToConfirm;
//    private bool canInteract;

//    private void Awake()
//    {
//        ui = GetComponentInParent<UI>();
//        questManager = Player.instance.questManager;
//    }

//    public void SetupNpcData(DialogueNPCData npcData) => this.npcData = npcData;

//    public void PlayDialogueLine(DialogueLineSO line)
//    {
//        currentLine = line;
//        currentChoices = line.choiceLines;
//        canInteract = false;
//        selectedChoice = null;
//        selectedChoiceIndex = 0;

//        HideAllChoices();

//        speakerPortrait.sprite = line.speaker.speakerPortrait;
//        speakerName.text = line.speaker.speakerName;

//        fullTextToShow = line.actionType == DialogueActionType.None || line.actionType == DialogueActionType.PlayerMakeChoice ?
//            line.GetRandomLine() : line.actionLine;

//        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));
//        StartCoroutine(EnableInteractionCo());
//    }

//    private void HandleNextAction()
//    {
//        switch (currentLine.actionType)
//        {
//            case DialogueActionType.OpenShop:
//                ui.SwitchToInGameUI();
//                ui.OpenMerchantUI(true);
//                break;
//            case DialogueActionType.PlayerMakeChoice:
//                if (selectedChoice == null)
//                {
//                    ShowChoices();
//                }
//                else
//                {
//                    DialogueLineSO selectedChoice = currentChoices[selectedChoiceIndex];
//                    PlayDialogueLine(selectedChoice);
//                }
//                break;
//            case DialogueActionType.OpenQuest:
//                ui.SwitchToInGameUI();
//                ui.OpenQuestUI(npcData.quests);
//                break;
//            case DialogueActionType.GetQuestReward:
//                ui.SwitchToInGameUI();
//                questManager.TryGetRewardFrom(npcData.npcRewardType);
//                break;
//            case DialogueActionType.OpenCraft:
//                ui.SwitchToInGameUI();
//                ui.OpenCraftUI(true);
//                break;
//            case DialogueActionType.CloseDialogue:
//                ui.SwitchToInGameUI();
//                break;
//        }
//    }

//    public void DialogueInteraction()
//    {
//        if (canInteract == false)
//            return;

//        if (typeTextCo != null)
//        {
//            CompleteTyping();

//            if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
//                waitingToConfirm = true;
//            else
//                HandleNextAction();

//            return;
//        }

//        if (waitingToConfirm || selectedChoice != null)
//        {
//            waitingToConfirm = false;
//            HandleNextAction();
//        }
//    }

//    private void CompleteTyping()
//    {
//        if (typeTextCo != null)
//        {
//            StopCoroutine(typeTextCo);
//            dialogueText.text = fullTextToShow;
//            typeTextCo = null;
//        }
//    }

//    private void ShowChoices()
//    {
//        //HideAllChoices();

//        for (int i = 0; i < dialogueChoiceText.Length; i++)
//        {
//            if(i < currentChoices.Length)
//            {
//                DialogueLineSO choice = currentChoices[i];
//                string choiceText = choice.playerChoiceAnswer;

//                dialogueChoiceText[i].gameObject.SetActive(true);
//                dialogueChoiceText[i].text = selectedChoiceIndex == i ? 
//                    $"<color=yellow> {i + 1}) {choiceText}" : 
//                    $"{i + 1}) {choiceText}";

//                if(choice.actionType == DialogueActionType.GetQuestReward && questManager.HasComplatedQuest () == false)
//                {
//                    dialogueChoiceText[i].gameObject.SetActive(false);
//                }
//            }
//            else
//            {
//                dialogueChoiceText[i].gameObject.SetActive(false);
//            }
//        }
//        selectedChoice = currentChoices[selectedChoiceIndex];
//    }

//    private void HideAllChoices()
//    {
//        foreach (var obj in dialogueChoiceText)
//            obj.gameObject.SetActive(false);
//    }

//    public void NavigateChoice(int direction)
//    {
//        if(currentChoices == null || currentChoices.Length <= 1)
//            return;

//        selectedChoiceIndex += direction;
//        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length -1);

//        ShowChoices();
//    }

//    private IEnumerator TypeTextCo(string text)
//    {
//        dialogueText.text = "";

//        foreach (char letter in text)
//        {
//            dialogueText.text += letter;
//            yield return new WaitForSeconds(textSpeed);
//        }
//        if(currentLine.actionType != DialogueActionType.PlayerMakeChoice)
//        {
//            waitingToConfirm = true;
//        }
//        else
//        {
//            yield return new WaitForSeconds(0.2f);
//            selectedChoice = null;
//            HandleNextAction();
//        }
//        typeTextCo = null;
//    }

//    private IEnumerator EnableInteractionCo()
//    {
//        yield return null;
//        canInteract = true;
//    }
//}








//using System.Collections;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UI_Dialogue : MonoBehaviour
//{
//    private UI ui;
//    private DialogueNPCData npcData;
//    private Player_QuestManager questManager;

//    [SerializeField] private Image speakerPortrait;
//    [SerializeField] private TextMeshProUGUI speakerName;
//    [SerializeField] private TextMeshProUGUI dialogueText;
//    [SerializeField] private TextMeshProUGUI[] dialogueChoiceText;

//    [Space]
//    [SerializeField] private float textSpeed = 0.1f;
//    private string fullTextToShow;
//    private Coroutine typeTextCo;

//    private DialogueLineSO currentLine;
//    private DialogueLineSO[] currentChoices;
//    private DialogueLineSO selectedChoice;
//    private int selectedChoiceIndex;

//    private bool waitingToConfirm;
//    private bool canInteract;

//    private void Awake()
//    {
//        ui = GetComponentInParent<UI>();
//        questManager = Player.instance.questManager;
//    }

//    public void SetupNpcData(DialogueNPCData npcData) => this.npcData = npcData;

//    public void PlayDialogueLine(DialogueLineSO line)
//    {
//        if (line == null)
//        {
//            Debug.LogError("Cannot play dialogue line: line is null!");
//            return;
//        }

//        if (line.speaker == null)
//        {
//            Debug.LogError($"Cannot play dialogue line: speaker is null in line '{line.name}'!");
//            return;
//        }

//        currentLine = line;
//        currentChoices = line.choiceLines;
//        canInteract = false;
//        selectedChoice = null;
//        selectedChoiceIndex = 0;

//        HideAllChoices();

//        speakerPortrait.sprite = line.speaker.speakerPortrait;
//        speakerName.text = line.speaker.speakerName;

//        fullTextToShow = line.actionType == DialogueActionType.None || line.actionType == DialogueActionType.PlayerMakeChoice ?
//            line.GetRandomLine() : line.actionLine;

//        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));
//        StartCoroutine(EnableInteractionCo());
//    }

//    private void HandleNextAction()
//    {
//        switch (currentLine.actionType)
//        {
//            case DialogueActionType.OpenShop:
//                ui.SwitchToInGameUI();
//                ui.OpenMerchantUI(true);
//                break;
//            case DialogueActionType.PlayerMakeChoice:
//                if (selectedChoice == null)
//                {
//                    ShowChoices();
//                }
//                else
//                {
//                    DialogueLineSO selectedChoice = currentChoices[selectedChoiceIndex];
//                    PlayDialogueLine(selectedChoice);
//                }
//                break;
//            case DialogueActionType.OpenQuest:
//                ui.SwitchToInGameUI();
//                ui.OpenQuestUI(npcData.quests);
//                break;
//            case DialogueActionType.GetQuestReward:
//                ui.SwitchToInGameUI();
//                questManager.TryGetRewardFrom(npcData.npcRewardType);
//                break;
//            case DialogueActionType.OpenCraft:
//                ui.SwitchToInGameUI();
//                ui.OpenCraftUI(true);
//                break;
//            case DialogueActionType.CloseDialogue:
//                ui.SwitchToInGameUI();
//                break;
//        }
//    }

//    public void DialogueInteraction()
//    {
//        if (canInteract == false)
//            return;

//        if (typeTextCo != null)
//        {
//            CompleteTyping();

//            if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
//                waitingToConfirm = true;
//            else
//                HandleNextAction();

//            return;
//        }

//        if (waitingToConfirm || selectedChoice != null)
//        {
//            waitingToConfirm = false;
//            HandleNextAction();
//        }
//    }

//    private void CompleteTyping()
//    {
//        if (typeTextCo != null)
//        {
//            StopCoroutine(typeTextCo);
//            dialogueText.text = fullTextToShow;
//            typeTextCo = null;
//        }
//    }

//    private void ShowChoices()
//    {
//        //HideAllChoices();

//        for (int i = 0; i < dialogueChoiceText.Length; i++)
//        {
//            if (i < currentChoices.Length)
//            {
//                DialogueLineSO choice = currentChoices[i];
//                string choiceText = choice.playerChoiceAnswer;

//                dialogueChoiceText[i].gameObject.SetActive(true);
//                dialogueChoiceText[i].text = selectedChoiceIndex == i ?
//                    $"<color=yellow> {i + 1}) {choiceText}" :
//                    $"{i + 1}) {choiceText}";

//                if (choice.actionType == DialogueActionType.GetQuestReward && questManager.HasComplatedQuest() == false)
//                {
//                    dialogueChoiceText[i].gameObject.SetActive(false);
//                }
//            }
//            else
//            {
//                dialogueChoiceText[i].gameObject.SetActive(false);
//            }
//        }
//        selectedChoice = currentChoices[selectedChoiceIndex];
//    }

//    private void HideAllChoices()
//    {
//        foreach (var obj in dialogueChoiceText)
//            obj.gameObject.SetActive(false);
//    }

//    public void NavigateChoice(int direction)
//    {
//        if (currentChoices == null || currentChoices.Length <= 1)
//            return;

//        selectedChoiceIndex += direction;
//        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoices.Length - 1);

//        ShowChoices();
//    }

//    private IEnumerator TypeTextCo(string text)
//    {
//        dialogueText.text = "";

//        foreach (char letter in text)
//        {
//            dialogueText.text += letter;
//            yield return new WaitForSeconds(textSpeed);
//        }
//        if (currentLine.actionType != DialogueActionType.PlayerMakeChoice)
//        {
//            waitingToConfirm = true;
//        }
//        else
//        {
//            yield return new WaitForSeconds(0.2f);
//            selectedChoice = null;
//            HandleNextAction();
//        }
//        typeTextCo = null;
//    }

//    private IEnumerator EnableInteractionCo()
//    {
//        yield return null;
//        canInteract = true;
//    }
//}




using System.Collections;
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
    [SerializeField] private TextMeshProUGUI[] dialogueChoiceText;

    [Header("Typing Settings")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private bool skipTypingOnClick = true;

    [Header("Visual Indicators")]
    [SerializeField] private GameObject continueIndicator; // Arrow or "click to continue" indicator

    private string fullTextToShow;
    private Coroutine typeTextCo;

    private DialogueLineSO currentLine;
    private DialogueLineSO[] currentChoices;
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private bool waitingToConfirm;
    private bool canInteract;

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

        // Ensure quest manager is available
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
        selectedChoice = null;
        selectedChoiceIndex = 0;

        HideAllChoices();
        HideContinueIndicator();

        // Update speaker info
        speakerPortrait.sprite = line.speaker.speakerPortrait;
        speakerName.text = line.speaker.speakerName;

        // Determine text to show
        fullTextToShow = DetermineDialogueText(line);

        // Start typing
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
        }
        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));

        StartCoroutine(EnableInteractionCo());
    }

    private string DetermineDialogueText(DialogueLineSO line)
    {
        // Use action line for specific actions, otherwise use random line
        if (line.actionType != DialogueActionType.None &&
            line.actionType != DialogueActionType.PlayerMakeChoice)
        {
            return !string.IsNullOrEmpty(line.actionLine) ? line.actionLine : line.GetRandomLine();
        }

        return line.GetRandomLine();
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType)
        {
            case DialogueActionType.None:
                // If there are no choices, just close
                CloseDialogue();
                break;

            case DialogueActionType.OpenShop:
                ui.SwitchToInGameUI();
                ui.OpenMerchantUI(true);
                break;

            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                {
                    ShowChoices();
                }
                else
                {
                    DialogueLineSO nextLine = currentChoices[selectedChoiceIndex];
                    PlayDialogueLine(nextLine);
                }
                break;

            case DialogueActionType.OpenQuest:
                if (npcData != null && npcData.availableQuests != null)
                {
                    // Filter to only show available quests
                    var availableQuests = npcData.GetAcceptableQuests(questManager);
                    if (availableQuests.Length > 0)
                    {
                        ui.SwitchToInGameUI();
                        ui.OpenQuestUI(availableQuests);
                    }
                    else
                    {
                        // No quests available, close dialogue
                        CloseDialogue();
                    }
                }
                else
                {
                    CloseDialogue();
                }
                break;

            case DialogueActionType.GetQuestReward:
                ui.SwitchToInGameUI();

                // Try to turn in quests by NPC ID first
                if (npcData != null && !string.IsNullOrEmpty(npcData.npcID))
                {
                    questManager?.TryGetRewardFromNpc(npcData.npcID);
                }
                else if (npcData != null)
                {
                    questManager?.TryGetRewardFrom(npcData.npcRewardType);
                }
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

    public void DialogueInteraction()
    {
        if (!canInteract) return;

        // If still typing, complete it first
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

        // Handle next action when confirmed
        if (waitingToConfirm || selectedChoice != null)
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

    private void ShowChoices()
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

                // Check if this choice should be shown
                bool shouldShow = ShouldShowChoice(choice);

                dialogueChoiceText[i].gameObject.SetActive(shouldShow);

                if (shouldShow)
                {
                    string choiceText = choice.playerChoiceAnswer;

                    // Highlight selected choice
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

        // If selected index is hidden, find first visible
        if (visibleChoices > 0 && !dialogueChoiceText[selectedChoiceIndex].gameObject.activeSelf)
        {
            for (int i = 0; i < dialogueChoiceText.Length; i++)
            {
                if (dialogueChoiceText[i].gameObject.activeSelf)
                {
                    selectedChoiceIndex = i;
                    ShowChoices(); // Refresh to update highlight
                    break;
                }
            }
        }

        if (currentChoices.Length > selectedChoiceIndex)
        {
            selectedChoice = currentChoices[selectedChoiceIndex];
        }
    }

    /// <summary>
    /// Determine if a choice should be shown based on conditions
    /// </summary>
    private bool ShouldShowChoice(DialogueLineSO choice)
    {
        // Hide "Get Reward" option if no completable quests
        if (choice.actionType == DialogueActionType.GetQuestReward)
        {
            if (questManager == null) return false;

            // Check if player has any completable quests for this NPC
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

        // Hide "View Quests" if no quests available
        if (choice.actionType == DialogueActionType.OpenQuest)
        {
            if (npcData == null || npcData.availableQuests == null) return false;

            return npcData.GetAcceptableQuests(questManager).Length > 0;
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
        if (currentChoices == null || currentChoices.Length <= 1)
            return;

        int attempts = 0;
        int maxAttempts = currentChoices.Length;

        do
        {
            selectedChoiceIndex += direction;

            // Wrap around
            if (selectedChoiceIndex < 0) selectedChoiceIndex = currentChoices.Length - 1;
            if (selectedChoiceIndex >= currentChoices.Length) selectedChoiceIndex = 0;

            attempts++;

            // Check if this choice is visible
            if (selectedChoiceIndex < dialogueChoiceText.Length &&
                dialogueChoiceText[selectedChoiceIndex].gameObject.activeSelf)
            {
                break;
            }
        } while (attempts < maxAttempts);

        ShowChoices();
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
            selectedChoice = null;
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
        if (ui != null)
        {
            ui.SwitchToInGameUI();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Skip all dialogue and close (for debug/testing)
    /// </summary>
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