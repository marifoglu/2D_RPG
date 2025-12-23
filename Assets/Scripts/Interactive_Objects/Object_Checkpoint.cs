using UnityEngine;

public class Object_Checkpoint : MonoBehaviour, ISaveable
{
    private Object_Checkpoint[] allCheckpoints;
    private Animator anim;
    private bool isActive = false; // Track if this checkpoint is currently active

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
        allCheckpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);

        if (anim == null)
            Debug.LogWarning($"Animator not found on checkpoint: {gameObject.name}");
    }

    public void ActivateCheckpoint(bool active)
    {
        isActive = active; // Store the active state

        if (anim == null)
        {
            Debug.LogWarning($"Cannot activate checkpoint: Animator is null on {gameObject.name}");
            return;
        }

        anim.SetBool("isActive", active);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SaveManager.instance == null || SaveManager.instance.GetGameData() == null)
        {
            Debug.LogWarning("Cannot activate checkpoint: SaveManager or GameData is not initialized");
            return;
        }

        foreach (var point in allCheckpoints)
        {
            if (point != null)
                point.ActivateCheckpoint(false);
        }

        SaveManager.instance.GetGameData().savedCheckpoint = transform.position;
        ActivateCheckpoint(true);
    }

    public void SaveData(ref GameData gameData)
    {
        // CRITICAL FIX: Only save position if this checkpoint is the active one
        if (isActive)
        {
            gameData.savedCheckpoint = transform.position;
        }
    }

    public void LoadData(GameData gameData)
    {
        // Use Vector3.Distance to compare positions with a small tolerance
        bool active = Vector3.Distance(gameData.savedCheckpoint, transform.position) < 0.01f;
        ActivateCheckpoint(active);

        if (active && Player.instance != null)
            Player.instance.TeleportPlayer(transform.position);
    }
}