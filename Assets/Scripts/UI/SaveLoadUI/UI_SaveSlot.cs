using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SaveSlot : MonoBehaviour
{
    [Header("Slot Index (0-4)")]
    [SerializeField] private int slotIndex;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI slotInfoText;

    [Header("Buttons")]
    [SerializeField] private Button slotButton;
    [SerializeField] private Button deleteButton;

    private bool isEmpty = true;
    private UI_SaveLoadMenu parentMenu;

    private void Awake()
    {
        parentMenu = GetComponentInParent<UI_SaveLoadMenu>();

        if (slotButton != null)
            slotButton.onClick.AddListener(OnSlotButtonClicked);

        if (deleteButton != null)
            deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public int GetSlotIndex() => slotIndex;

    public void UpdateDisplay(SaveSlotData slotData)
    {
        if (slotData == null)
        {
            isEmpty = true;
            UpdateEmptyState();
            return;
        }

        isEmpty = slotData.isEmpty;

        if (slotNameText != null)
        {
            if (isEmpty)
                slotNameText.text = $"Slot {slotIndex + 1} - Empty";
            else
                slotNameText.text = $"Slot {slotIndex + 1} - {slotData.lastSceneName}";
        }

        if (slotInfoText != null)
        {
            if (isEmpty)
                slotInfoText.text = "Click to start new game";
            else
                slotInfoText.text = $"{slotData.saveDateTime}  |  Gold: {slotData.gold}  |  Time: {FormatTime(slotData.playTime)}";
        }

        if (deleteButton != null)
            deleteButton.gameObject.SetActive(!isEmpty);
    }

    private void UpdateEmptyState()
    {
        if (slotNameText != null)
            slotNameText.text = $"Slot {slotIndex + 1} - Empty";

        if (slotInfoText != null)
            slotInfoText.text = "Click to start new game";

        if (deleteButton != null)
            deleteButton.gameObject.SetActive(false);
    }

    private string FormatTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        if (time.TotalHours >= 1)
            return $"{(int)time.TotalHours}h {time.Minutes}m";
        else
            return $"{time.Minutes}m {time.Seconds}s";
    }

    private void OnSlotButtonClicked()
    {
        Debug.Log($"Slot {slotIndex} clicked, isEmpty: {isEmpty}");

        if (parentMenu != null)
            parentMenu.OnSlotClicked(slotIndex, isEmpty);
    }

    private void OnDeleteButtonClicked()
    {
        Debug.Log($"Delete slot {slotIndex} clicked");

        if (parentMenu != null)
            parentMenu.OnDeleteClicked(slotIndex);
    }

    private void OnDestroy()
    {
        if (slotButton != null)
            slotButton.onClick.RemoveAllListeners();

        if (deleteButton != null)
            deleteButton.onClick.RemoveAllListeners();
    }
}