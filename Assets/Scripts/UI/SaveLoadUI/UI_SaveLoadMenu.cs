using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SaveLoadMenu : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Save Slots - Drag your 5 slots here in order")]
    [SerializeField] private UI_SaveSlot[] saveSlots;

    [Header("Buttons")]
    [SerializeField] private Button backButton;

    private bool isSaveMode = false;

    public event Action OnBackPressed;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void Start()
    {
        // Make sure each slot knows its index
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null)
                saveSlots[i].SetSlotIndex(i);
        }
    }

    private void OnEnable()
    {
        RefreshAllSlots();
    }

    public void RefreshAllSlots()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("SaveManager.instance is null!");
            return;
        }

        SaveManager.instance.RefreshSlotMetadata();
        SaveSlotData[] slotsData = SaveManager.instance.SaveSlots;

        if (slotsData == null)
        {
            Debug.LogError("SaveSlots data is null!");
            return;
        }

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null)
            {
                saveSlots[i].SetSlotIndex(i);

                if (i < slotsData.Length)
                    saveSlots[i].UpdateDisplay(slotsData[i]);
                else
                    saveSlots[i].UpdateDisplay(null);
            }
        }

        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (titleText != null)
            titleText.text = isSaveMode ? "SAVE GAME" : "LOAD GAME";
    }

    public void SetMode(bool saveMode)
    {
        isSaveMode = saveMode;
        RefreshAllSlots();
    }

    // Called by UI_SaveSlot when slot button is clicked
    public void OnSlotClicked(int slotIndex, bool isEmpty)
    {
        Debug.Log($"OnSlotClicked: slot {slotIndex}, isEmpty: {isEmpty}, isSaveMode: {isSaveMode}");

        if (isSaveMode)
        {
            // Save mode - save to this slot
            SaveToSlot(slotIndex);
        }
        else
        {
            // Load mode
            if (isEmpty)
            {
                // Empty slot - start new game
                StartNewGame(slotIndex);
            }
            else
            {
                // Has save - load it
                LoadGame(slotIndex);
            }
        }
    }

    // Called by UI_SaveSlot when delete button is clicked
    public void OnDeleteClicked(int slotIndex)
    {
        Debug.Log($"OnDeleteClicked: slot {slotIndex}");
        DeleteSave(slotIndex);
    }

    private void SaveToSlot(int slotIndex)
    {
        Debug.Log($"Saving to slot {slotIndex}...");
        SaveManager.instance.SaveGame(slotIndex);
        RefreshAllSlots();
    }

    private void LoadGame(int slotIndex)
    {
        Debug.Log($"Loading from slot {slotIndex}...");
        SaveManager.instance.SetCurrentSlot(slotIndex);
        SaveManager.instance.LoadGame(slotIndex);
        GameManager.instance.ContinuePlay();
    }

    private void StartNewGame(int slotIndex)
    {
        Debug.Log($"Starting new game in slot {slotIndex}...");
        SaveManager.instance.StartNewGame(slotIndex);
        GameManager.instance.ChangeScene("Demo_Level_0", RespawnType.Enter);
    }

    private void DeleteSave(int slotIndex)
    {
        Debug.Log($"Deleting slot {slotIndex}...");
        SaveManager.instance.DeleteSlot(slotIndex);
        RefreshAllSlots();
    }

    private void OnBackButtonClicked()
    {
        OnBackPressed?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveAllListeners();
    }
}