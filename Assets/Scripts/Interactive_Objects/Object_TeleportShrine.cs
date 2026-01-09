using UnityEngine;
using System.Collections;

public class Object_TeleportShrine : MonoBehaviour, IInteractable, ISaveable
{
    private Animator anim;
    private Player player;
    private AudioSource audioSource;

    [Header("Shrine Identity")]
    [SerializeField] private string shrineID;  // SET THIS MANUALLY IN INSPECTOR - DO NOT AUTO-GENERATE
    [SerializeField] private string shrineName = "Shrine";
    [SerializeField] private string sceneName;

    [Header("Teleport Settings")]
    [SerializeField] private Transform arrivalPoint;
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool canTeleportTo = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactTooltip;
    [SerializeField] private GameObject activeParticles;
    [SerializeField] private float tooltipFloatSpeed = 8f;
    [SerializeField] private float tooltipFloatRange = 0.1f;
    private Vector3 tooltipStartPosition;

    [Header("Shrine VFX")]
    [SerializeField] private GameObject shrineActivateVFX;
    [SerializeField] private GameObject playerArriveVFX;
    [SerializeField] private GameObject playerDepartVFX;

    [Header("Audio")]
    [SerializeField] private string activateSoundName = "ShrineActivate";
    [SerializeField] private string teleportSoundName = "ShrineTeleport";
    [SerializeField] private string arrivalSoundName = "ShrineArrival";

    // State
    private bool playerInRange = false;
    private bool isAnimating = false;

    // Events
    public System.Action<Object_TeleportShrine> OnShrineActivated;
    public System.Action<Object_TeleportShrine> OnPlayerArrival;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (arrivalPoint == null)
            arrivalPoint = transform;

        if (interactTooltip != null)
        {
            tooltipStartPosition = interactTooltip.transform.position;
            interactTooltip.SetActive(false);
        }

        // Only generate ID if not set - prefer manual IDs in Inspector!
        if (string.IsNullOrEmpty(shrineID))
        {
            shrineID = $"{gameObject.name}_{gameObject.GetInstanceID()}";
        }

        if (string.IsNullOrEmpty(sceneName))
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        if (ShrineManager.instance != null)
            ShrineManager.instance.RegisterShrine(this);
    }

    private void Update()
    {
        if (interactTooltip != null && interactTooltip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * tooltipFloatSpeed) * tooltipFloatRange;
            interactTooltip.transform.position = tooltipStartPosition + new Vector3(0, yOffset, 0);
        }
    }
    #region Activation

    public void Activate()
    {
        if (isActive || isAnimating)
            return;

        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        isAnimating = true;

        if (shrineActivateVFX != null)
        {
            GameObject vfx = Instantiate(shrineActivateVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        if (anim != null)
            anim.SetTrigger("Activate");

        PlaySound(activateSoundName);

        yield return new WaitForSeconds(0.5f);

        isActive = true;

        if (anim != null)
            anim.SetBool("isActive", true);

        if (activeParticles != null)
            activeParticles.SetActive(true);

        isAnimating = false;

        // ALWAYS set this shrine as last activated when player activates it
        SetAsLastActivatedShrine();
        SaveActivationState();

        OnShrineActivated?.Invoke(this);
    }

    private void SetAsLastActivatedShrine()
    {
        if (SaveManager.instance == null)
            return;

        GameData data = SaveManager.instance.GetGameData();
        data.lastActivatedShrineID = shrineID;
        data.lastActivatedShrineScene = sceneName;
        data.lastActivatedShrinePosX = GetArrivalPosition().x;
        data.lastActivatedShrinePosY = GetArrivalPosition().y;
        data.lastActivatedShrinePosZ = GetArrivalPosition().z;
    }
    private void SetAsCurrentShrine()
    {
        if (SaveManager.instance == null)
            return;

        GameData data = SaveManager.instance.GetGameData();

        // If there's already a current shrine and it's not THIS one, save it as previous
        if (!string.IsNullOrEmpty(data.lastActivatedShrineID) && data.lastActivatedShrineID != shrineID)
        {
            data.previousShrineID = data.lastActivatedShrineID;
        }

        // This shrine is now current
        data.lastActivatedShrineID = shrineID;
        data.lastActivatedShrineScene = sceneName;
        data.lastActivatedShrinePosX = GetArrivalPosition().x;
        data.lastActivatedShrinePosY = GetArrivalPosition().y;
        data.lastActivatedShrinePosZ = GetArrivalPosition().z;
    }
    private void SaveActivationState()
    {
        if (SaveManager.instance == null)
            return;

        GameData data = SaveManager.instance.GetGameData();

        data.activatedShrines[shrineID] = true;

        SaveManager.instance.SaveGame();
    }
    #endregion

    #region Interaction
    public void Interact()
    {
        // Require player to be in range to interact
        if (!playerInRange)
            return;
       
        if (isAnimating)
            return;

        player = Player.instance;
        if (player == null)
            return;

        // Only activate if not already active
        if (!isActive)
            Activate();
    }
    #endregion

    #region Teleportation
    public void OnPlayerArrived()
    {
        StartCoroutine(PlayerArrivalSequence());
    }

    private IEnumerator PlayerArrivalSequence()
    {
        if (playerArriveVFX != null)
        {
            GameObject vfx = Instantiate(playerArriveVFX, GetArrivalPosition(), Quaternion.identity);
            Destroy(vfx, 3f);
        }

        if (anim != null)
            anim.SetTrigger("PlayerArrived");

        PlaySound(arrivalSoundName);

        yield return new WaitForSeconds(0.3f);

        OnPlayerArrival?.Invoke(this);
    }

    public void OnPlayerDeparting()
    {
        SetAsLastActivatedShrine();
        SaveManager.instance?.SaveGame();

        if (playerDepartVFX != null)
        {
            Vector3 pos = Player.instance != null ? Player.instance.transform.position : transform.position;
            GameObject vfx = Instantiate(playerDepartVFX, pos, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        PlaySound(teleportSoundName);
    }

    public Vector3 GetArrivalPosition()
    {
        return arrivalPoint != null ? arrivalPoint.position : transform.position;
    }
    #endregion

    #region Trigger Detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = true;
        player = collision.GetComponent<Player>();

        if (interactTooltip != null)
            interactTooltip.SetActive(true);

        if (isActive)
        {
            SetAsLastActivatedShrine();
            SaveManager.instance?.SaveGame();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactTooltip != null)
            interactTooltip.SetActive(false);

        player = null;
    }
    #endregion

    #region Helpers
    private void UpdateVisualState()
    {
        if (anim != null)
        {
            anim.SetBool("isActive", isActive);

            // Force the animator to the correct state immediately
            if (isActive)
            {
                anim.Play("Active", 0, 0);
            }
        }
        if (activeParticles != null)
            activeParticles.SetActive(isActive);
    }

    private void PlaySound(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
            return;

        if (AudioManager.instance != null)
        {
            if (audioSource != null)
                AudioManager.instance.PlaySFX(soundName, audioSource);
            else
                AudioManager.instance.PlayGlobalSFX(soundName);
        }
    }
    #endregion

    #region Getters
    public string GetShrineID() => shrineID;
    public string GetShrineName() => shrineName;
    public string GetSceneName() => sceneName;
    public bool IsActive => isActive;
    public bool CanTeleportTo => canTeleportTo && isActive;
    public bool IsPlayerInRange => playerInRange;
    #endregion

    #region Save/Load
    public void SaveData(ref GameData gameData)
    {
        if (isActive == false)
            return;

        gameData.activatedShrines[shrineID] = isActive;
    }

    public void LoadData(GameData gameData)
    {
        if (gameData.activatedShrines.TryGetValue(shrineID, out bool savedActive))
        {
            isActive = savedActive;

            // Defer visual update to ensure animator is ready
            if (isActive)
            {
                StartCoroutine(ForceActiveStateDelayed());
            }
        }
        else
        {
            Debug.Log($"[Shrine] LoadData: {shrineID} NOT FOUND in save data");
        }
    }

    private IEnumerator ForceActiveStateDelayed()
    {
        // Wait one frame so Animator is initialized
        yield return null;

        if (anim != null)
        {
            anim.SetBool("isActive", true);

            int layerIndex = 0;
            int stateHash = Animator.StringToHash("isActive");
            if (anim.HasState(layerIndex, stateHash))
            {
                anim.Play(stateHash, layerIndex, 0f);
                anim.Update(0f);
            }
            else
            {
                Debug.LogWarning($"[Shrine] Animator state 'Active' not found on '{gameObject.name}'. Set bool only.");
            }
        }

        if (activeParticles != null)
            activeParticles.SetActive(true);
    }
    #endregion

    #region Editor

    private void OnValidate()
    {
#if UNITY_EDITOR

        if (string.IsNullOrEmpty(sceneName) && gameObject.scene.IsValid())
            sceneName = gameObject.scene.name;
#endif
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = arrivalPoint != null ? arrivalPoint.position : transform.position;
        Gizmos.color = isActive ? Color.green : Color.gray;
        Gizmos.DrawWireSphere(pos, 0.5f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(pos + Vector3.up, $"{shrineName}\nID: {shrineID}\n{(isActive ? "ACTIVE" : "INACTIVE")}");
#endif
    }

    #endregion
}