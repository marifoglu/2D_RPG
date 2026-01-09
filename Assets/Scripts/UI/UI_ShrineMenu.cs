using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShrineMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject shrineButtonPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI titleText;

    [Header("Settings")]
    [SerializeField] private string menuTitle = "Select Shrine";

    private Action<Object_TeleportShrine> onShrineSelected;
    private List<GameObject> spawnedButtons = new List<GameObject>();
    private bool isMenuOpen = false;

    private void Awake()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideMenu);
    }

    private void Update()
    {
        if (isMenuOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            HideMenu();
        }
    }

    public void ShowMenu(List<Object_TeleportShrine> shrines, Action<Object_TeleportShrine> onSelected)
    {
        if (shrines == null || shrines.Count == 0)
        {
            Debug.Log("[UI_ShrineMenu] No shrines to show!");
            return;
        }

        onShrineSelected = onSelected;

        // Clear old buttons
        ClearButtons();

        // Set title
        if (titleText != null)
            titleText.text = menuTitle;

        // Create buttons for each shrine
        foreach (var shrine in shrines)
        {
            CreateShrineButton(shrine);
        }

        // Show panel
        if (menuPanel != null)
            menuPanel.SetActive(true);

        isMenuOpen = true;

        // Pause game
        Time.timeScale = 0f;

        // Disable player input
        if (Player.instance != null && Player.instance.input != null)
            Player.instance.input.Disable();

        Debug.Log($"[UI_ShrineMenu] Showing {shrines.Count} shrines");
    }

    public void HideMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        isMenuOpen = false;

        // Unpause game
        Time.timeScale = 1f;

        // Re-enable player input
        if (Player.instance != null && Player.instance.input != null)
            Player.instance.input.Enable();

        ClearButtons();
    }

    private void CreateShrineButton(Object_TeleportShrine shrine)
    {
        if (shrineButtonPrefab == null || buttonContainer == null)
        {
            Debug.LogWarning("[UI_ShrineMenu] Missing button prefab or container!");
            return;
        }

        GameObject buttonObj = Instantiate(shrineButtonPrefab, buttonContainer);
        spawnedButtons.Add(buttonObj);

        // Set button text
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = shrine.GetShrineName();
        }
        else
        {
            Text legacyText = buttonObj.GetComponentInChildren<Text>();
            if (legacyText != null)
                legacyText.text = shrine.GetShrineName();
        }

        // Add click handler
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            Object_TeleportShrine targetShrine = shrine; // Capture for lambda
            button.onClick.AddListener(() => OnButtonClicked(targetShrine));
        }
    }

    private void OnButtonClicked(Object_TeleportShrine shrine)
    {
        HideMenu();
        onShrineSelected?.Invoke(shrine);
    }

    private void ClearButtons()
    {
        foreach (var btn in spawnedButtons)
        {
            if (btn != null)
                Destroy(btn);
        }
        spawnedButtons.Clear();
    }
}