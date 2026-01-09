using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineManager : MonoBehaviour
{
    public static ShrineManager instance;

    [Header("Settings")]
    [SerializeField] private bool useFadeEffect = true;
    [SerializeField] private float fadeDuration = 0.3f;

    private Player player;
    private Object_TeleportShrine currentShrine;
    private List<Object_TeleportShrine> allShrines = new List<Object_TeleportShrine>();
    private bool isTeleporting = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        RefreshShrineList();
        player = Player.instance;
    }

    public void RefreshShrineList()
    {
        allShrines.Clear();
        allShrines.AddRange(FindObjectsByType<Object_TeleportShrine>(FindObjectsSortMode.None));
        Debug.Log($"[ShrineManager] Found {allShrines.Count} shrines");
    }

    public void RegisterShrine(Object_TeleportShrine shrine)
    {
        if (shrine != null && !allShrines.Contains(shrine))
        {
            allShrines.Add(shrine);
        }
    }

    public void UnregisterShrine(Object_TeleportShrine shrine)
    {
        allShrines.Remove(shrine);
    }

    public List<Object_TeleportShrine> GetAvailableShrines(Object_TeleportShrine excludeShrine = null)
    {
        List<Object_TeleportShrine> available = new List<Object_TeleportShrine>();

        foreach (var shrine in allShrines)
        {
            if (shrine == null) continue;
            if (shrine == excludeShrine) continue;
            if (!shrine.CanTeleportTo) continue;

            available.Add(shrine);
        }

        return available;
    }

    public void TeleportToShrine(Object_TeleportShrine targetShrine)
    {
        if (targetShrine == null || isTeleporting)
            return;

        StartCoroutine(TeleportSequence(targetShrine));
    }

    private IEnumerator TeleportSequence(Object_TeleportShrine targetShrine)
    {
        isTeleporting = true;

        if (player == null)
            player = Player.instance;

        if (player == null)
        {
            Debug.LogError("[ShrineManager] Player not found!");
            isTeleporting = false;
            yield break;
        }

        Debug.Log($"[ShrineManager] Teleporting to {targetShrine.GetShrineName()}");

        // Notify departure
        if (currentShrine != null)
            currentShrine.OnPlayerDeparting();

        Vector3 destination = targetShrine.GetArrivalPosition();

        // Use teleport state
        if (player.teleportState != null)
        {
            player.teleportState.SetupTeleport(destination, targetShrine, OnTeleportComplete);
            player.stateMachine.ChangeState(player.teleportState);
        }
        else
        {
            yield return StartCoroutine(SimpleTeleport(destination, targetShrine));
        }
    }

    private IEnumerator SimpleTeleport(Vector3 destination, Object_TeleportShrine targetShrine)
    {
        player.SetVelocity(0, 0);

        UI_FadeScreen fadeScreen = null;
        if (useFadeEffect)
        {
            fadeScreen = FindFirstObjectByType<UI_FadeScreen>();
            if (fadeScreen != null)
            {
                fadeScreen.DoFadeOut(fadeDuration);
                yield return new WaitForSeconds(fadeDuration);
            }
        }

        player.transform.position = destination;
        targetShrine.OnPlayerArrived();

        if (fadeScreen != null && useFadeEffect)
        {
            fadeScreen.DoFadeIn(fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
        }

        OnTeleportComplete();
    }

    private void OnTeleportComplete()
    {
        isTeleporting = false;
        currentShrine = null;
    }

    public bool IsTeleporting => isTeleporting;
    public int ActiveShrineCount => GetAvailableShrines().Count;
}