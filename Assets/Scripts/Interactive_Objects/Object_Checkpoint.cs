using Unity.VisualScripting;
using UnityEngine;

public class Object_Checkpoint : MonoBehaviour, ISaveable
{
    private Animator anim;
    [SerializeField] private string checkpointID;
    [SerializeField] private Transform respawnPoint;
    public bool IsActive { get; private set; }

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>(true);
    }

    public void ActivateCheckpoint(bool active)
    {
        IsActive = active;
        anim.SetBool("isActive", active);
    }

    public string GetCheckpointID() => checkpointID;

    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
            
        ActivateCheckpoint(true);
    }

    public void LoadData(GameData gameData)
    {
        bool active = gameData.unlockedCheckpoints.TryGetValue(checkpointID, out active);
        ActivateCheckpoint(active);
    }

    public void SaveData(ref GameData gameData)
    {
        if (IsActive == false)
            return;

        if (gameData.unlockedCheckpoints.ContainsKey(checkpointID) == false)
            gameData.unlockedCheckpoints.Add(checkpointID, true);
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointID))
        {
            checkpointID = System.Guid.NewGuid().ToString();
        }
#endif
    }
}