using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [Header("Global Shake Force")]
    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[CameraShakeManager] Initialized.");
        }
        else
        {
            Debug.LogWarning("[CameraShakeManager] Duplicate instance detected. Destroying...");
            Destroy(gameObject);
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        if (impulseSource == null)
        {
            Debug.LogError("[CameraShakeManager] No CinemachineImpulseSource passed!");
            return;
        }

        Debug.Log($"[CameraShakeManager] Triggering camera shake with force {globalShakeForce}.");
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

}
