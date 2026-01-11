using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

[Serializable]
public class SceneAFKVFX
{
    [Tooltip("Scene name (must match exactly)")]
    public string sceneName;
    [Tooltip("VFX prefab to spawn in this scene")]
    public GameObject vfxPrefab;
    [Tooltip("Offset from player position")]
    public Vector3 spawnOffset = Vector3.zero;
    [Tooltip("Should VFX follow player or stay in place?")]
    public bool followPlayer = true;
}

public class AFKManager : MonoBehaviour
{
    public static AFKManager Instance { get; private set; }

    [Header("AFK Detection Settings")]
    [Tooltip("Time in seconds before player is considered AFK")]
    [SerializeField] private float afkTimeThreshold = 60f;

    [Header("Camera Zoom Settings")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [Tooltip("Your normal camera size (set this to your default zoom level)")]
    [SerializeField] private float originalCameraSize = 5f;
    [Tooltip("Target orthographic size when zoomed in (smaller = more zoomed)")]
    [SerializeField] private float zoomedInSize = 3f;
    [Tooltip("How fast the camera zooms in/out")]
    [SerializeField] private float zoomSpeed = 2f;
    [Tooltip("Vertical offset when zoomed in (positive = look higher, negative = look lower)")]
    [SerializeField] private float zoomedInVerticalOffset = 0.5f;

    [Header("AFK Idle Animation Settings")]
    [Tooltip("How long normal idle plays before a special idle")]
    [SerializeField] private float normalIdleDuration = 120f;  // 2 minutes

    [Header("AFK Scene VFX")]
    [Tooltip("Default VFX if no scene-specific VFX is found")]
    [SerializeField] private GameObject defaultAFKVfxPrefab;
    [Tooltip("Default offset from player")]
    [SerializeField] private Vector3 defaultVfxOffset = Vector3.zero;
    [Tooltip("Scene-specific VFX list")]
    [SerializeField] private SceneAFKVFX[] sceneVFXList;

    [Header("VFX Timing Settings")]
    [Tooltip("Delay before VFX appears (let camera zoom first)")]
    [SerializeField] private float vfxSpawnDelay = 1.5f;

    // Private variables
    private float lastInputTime;
    private bool isAFK = false;
    private bool isZooming = false;
    private Coroutine zoomCoroutine;
    private Coroutine afkIdleCoroutine;

    private Player player;
    private Animator playerAnimator;
    private PlayerInputSet playerInput;

    [Header("Original Camera Settings (auto-captured or set manually)")]
    [Tooltip("Original target offset - use context menu to capture")]
    [SerializeField] private Vector3 originalTrackedObjectOffset = Vector3.zero;
    [Tooltip("Original follow offset - use context menu to capture")]
    [SerializeField] private Vector3 originalFollowOffset = new Vector3(0, 2, -10);
    [Tooltip("Original dead zone size - use context menu to capture")]
    [SerializeField] private Vector2 originalDeadZoneSize = new Vector2(0.1f, 0.1f);

    // Camera components
    private CinemachinePositionComposer positionComposer;
    private CinemachineFollow followComponent;
    private float originalDamping;

    // AFK idle tracking
    private int lastSpecialIdleIndex = 0;  // Track which special idle played last
    private const int SPECIAL_IDLE_COUNT = 2;  // idle2 and idle3

    // VFX tracking
    private GameObject currentVFXInstance;
    private SceneAFKVFX currentSceneVFX;
    private Coroutine vfxFollowCoroutine;
    private Coroutine vfxSpawnCoroutine;

    public bool IsAFK => isAFK;
    public float AFKTimeThreshold => afkTimeThreshold;
    public float TimeSinceLastInput => Time.time - lastInputTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        player = Player.instance;

        if (player != null)
        {
            playerAnimator = player.anim;
            playerInput = player.input;
        }

        // Setup camera
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (cinemachineCamera != null)
        {
            CacheOriginalCameraSettings();
            Debug.Log($"[AFKManager] Camera ready. Original size: {originalCameraSize}");
        }
        else
        {
            Debug.LogWarning("[AFKManager] No CinemachineCamera found! Camera zoom will not work.");
        }

        ResetAFKTimer();
    }

    private void CacheOriginalCameraSettings()
    {
        if (cinemachineCamera == null) return;

        // Get component references
        positionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        if (positionComposer != null)
        {
            originalDamping = positionComposer.Damping.x;
            Debug.Log("[AFKManager] Found CinemachinePositionComposer");
        }

        followComponent = cinemachineCamera.GetComponent<CinemachineFollow>();
        if (followComponent != null)
        {
            Debug.Log("[AFKManager] Found CinemachineFollow");
        }
    }

    private void Update()
    {
        CheckForInput();
        CheckAFKStatus();
    }

    private void CheckForInput()
    {
        // Check for any input that should reset AFK timer
        bool hasInput = false;

        // Check keyboard/gamepad movement
        if (player != null && player.moveInput != Vector2.zero)
        {
            hasInput = true;
        }

        // Check mouse movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            hasInput = true;
        }

        // Check any key press
        if (Input.anyKeyDown)
        {
            hasInput = true;
        }

        // Check mouse clicks
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            hasInput = true;
        }

        if (hasInput)
        {
            ResetAFKTimer();
        }
    }

    private void CheckAFKStatus()
    {
        float timeSinceInput = Time.time - lastInputTime;

        if (!isAFK && timeSinceInput >= afkTimeThreshold)
        {
            EnterAFKMode();
        }
    }

    private void ResetAFKTimer()
    {
        lastInputTime = Time.time;

        if (isAFK)
        {
            ExitAFKMode();
        }
    }

    //private void EnterAFKMode()
    //{
    //    if (isAFK) return;

    //    isAFK = true;
    //    Debug.Log("[AFKManager] Player is now AFK");

    //    // Capture current camera size BEFORE zooming
    //    if (cinemachineCamera != null)
    //    {
    //        originalCameraSize = cinemachineCamera.Lens.OrthographicSize;
    //        Debug.Log($"[AFKManager] Captured current camera size: {originalCameraSize}");
    //    }

    //    // Start camera zoom in and center on player
    //    StartCameraZoom(zoomedInSize, true);

    //    // Start AFK idle animation cycle
    //    StartAFKIdleAnimations();

    //    // Spawn scene VFX
    //    SpawnAFKVFX();
    //}

    private void EnterAFKMode()
    {
        if (isAFK) return;

        isAFK = true;

        if (cinemachineCamera != null)
        {
            float currentSize = cinemachineCamera.Lens.OrthographicSize;
            if (!Mathf.Approximately(currentSize, zoomedInSize))
            {
                originalCameraSize = currentSize;
            }
        }

        StartCameraZoom(zoomedInSize, true);

        // Start AFK idle animation cycle
        StartAFKIdleAnimations();

        // Spawn scene VFX
        SpawnAFKVFX();
    }

    private void ExitAFKMode()
    {
        if (!isAFK) return;

        isAFK = false;
        Debug.Log("[AFKManager] Player returned from AFK");

        // Start camera zoom out to original and restore settings
        StartCameraZoom(originalCameraSize, false);

        // Stop AFK idle animations and return to normal
        StopAFKIdleAnimations();

        // Remove scene VFX
        DespawnAFKVFX();
    }

    #region Camera Zoom

    private void StartCameraZoom(float targetSize, bool centerOnPlayer)
    {
        if (cinemachineCamera == null) return;

        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }

        zoomCoroutine = StartCoroutine(ZoomCameraCoroutine(targetSize, centerOnPlayer));
    }

    private IEnumerator ZoomCameraCoroutine(float targetSize, bool centerOnPlayer)
    {
        isZooming = true;
        float startSize = cinemachineCamera.Lens.OrthographicSize;
        float elapsed = 0f;
        float duration = Mathf.Abs(targetSize - startSize) / zoomSpeed;
        duration = Mathf.Max(duration, 0.5f); // Minimum duration

        // Store starting values for lerping
        Vector3 startOffset = Vector3.zero;
        Vector3 targetOffset = Vector3.zero;
        Vector2 startDeadZoneSize = Vector2.zero;
        Vector2 targetDeadZoneSize = Vector2.zero;

        if (positionComposer != null)
        {
            startOffset = positionComposer.TargetOffset;
            startDeadZoneSize = positionComposer.Composition.DeadZone.Size;

            if (centerOnPlayer)
            {
                // Zoom in: center on player, remove dead zone
                targetOffset = new Vector3(0f, zoomedInVerticalOffset, 0f);
                targetDeadZoneSize = Vector2.zero;
            }
            else
            {
                // Zoom out: restore original settings
                targetOffset = originalTrackedObjectOffset;
                targetDeadZoneSize = originalDeadZoneSize;
            }
        }

        Vector3 startFollowOffset = Vector3.zero;
        Vector3 targetFollowOffset = Vector3.zero;

        if (followComponent != null)
        {
            startFollowOffset = followComponent.FollowOffset;

            if (centerOnPlayer)
            {
                targetFollowOffset = new Vector3(0f, zoomedInVerticalOffset, followComponent.FollowOffset.z);
            }
            else
            {
                targetFollowOffset = originalFollowOffset;
            }
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0f, 1f, t); // Smooth easing

            // Zoom
            var lens = cinemachineCamera.Lens;
            lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t);
            cinemachineCamera.Lens = lens;

            // Center/Uncenter with Position Composer
            if (positionComposer != null)
            {
                positionComposer.TargetOffset = Vector3.Lerp(startOffset, targetOffset, t);

                var composition = positionComposer.Composition;
                composition.DeadZone.Size = Vector2.Lerp(startDeadZoneSize, targetDeadZoneSize, t);
                positionComposer.Composition = composition;
            }

            // Center/Uncenter with Follow component
            if (followComponent != null)
            {
                followComponent.FollowOffset = Vector3.Lerp(startFollowOffset, targetFollowOffset, t);
            }

            yield return null;
        }

        // Ensure we hit exact targets
        var finalLens = cinemachineCamera.Lens;
        finalLens.OrthographicSize = targetSize;
        cinemachineCamera.Lens = finalLens;

        if (positionComposer != null)
        {
            positionComposer.TargetOffset = targetOffset;
            var finalComp = positionComposer.Composition;
            finalComp.DeadZone.Size = targetDeadZoneSize;
            positionComposer.Composition = finalComp;
        }

        if (followComponent != null)
        {
            followComponent.FollowOffset = targetFollowOffset;
        }

        isZooming = false;
        zoomCoroutine = null;
    }

    #endregion

    #region AFK Idle Animations

    private void StartAFKIdleAnimations()
    {
        if (playerAnimator == null) return;

        // Set AFK mode - character stays on normal idle
        playerAnimator.SetBool("IsAFK", true);
        playerAnimator.SetInteger("SpecialIdleIndex", 0);  // 0 = normal idle

        // Start the special idle timer
        if (afkIdleCoroutine != null)
        {
            StopCoroutine(afkIdleCoroutine);
        }
        afkIdleCoroutine = StartCoroutine(AFKSpecialIdleCoroutine());
    }

    private void StopAFKIdleAnimations()
    {
        if (playerAnimator == null) return;

        // Exit AFK mode
        playerAnimator.SetBool("IsAFK", false);
        playerAnimator.SetInteger("SpecialIdleIndex", 0);

        if (afkIdleCoroutine != null)
        {
            StopCoroutine(afkIdleCoroutine);
            afkIdleCoroutine = null;
        }
    }

    private IEnumerator AFKSpecialIdleCoroutine()
    {
        while (isAFK)
        {
            // Wait in normal idle for configured duration
            yield return new WaitForSeconds(normalIdleDuration);

            if (!isAFK) yield break;

            // Pick special idle (alternate between 1 and 2)
            lastSpecialIdleIndex = (lastSpecialIdleIndex % SPECIAL_IDLE_COUNT) + 1;

            // Trigger special idle
            playerAnimator.SetInteger("SpecialIdleIndex", lastSpecialIdleIndex);
            Debug.Log($"[AFKManager] Playing Special Idle {lastSpecialIdleIndex + 1}");

            // Wait one frame for animator to start the animation
            yield return null;

            // Wait for the special idle animation to finish
            yield return new WaitUntil(() => {
                if (!isAFK) return true;

                AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
                // Check if we're in a special idle state and it's finished (normalizedTime >= 1)
                bool isInSpecialIdle = stateInfo.IsTag("SpecialIdle");
                bool isFinished = stateInfo.normalizedTime >= 1f;

                return !isInSpecialIdle || isFinished;
            });

            // Return to normal idle
            if (isAFK)
            {
                playerAnimator.SetInteger("SpecialIdleIndex", 0);
                Debug.Log("[AFKManager] Returning to normal idle");
            }
        }
    }

    #endregion

    #region AFK VFX

    private void SpawnAFKVFX()
    {
        if (player == null) return;

        // Cancel any pending spawn
        if (vfxSpawnCoroutine != null)
        {
            StopCoroutine(vfxSpawnCoroutine);
        }

        vfxSpawnCoroutine = StartCoroutine(SpawnAFKVFXDelayed());
    }

    private IEnumerator SpawnAFKVFXDelayed()
    {
        // Wait for camera to zoom in first
        yield return new WaitForSeconds(vfxSpawnDelay);

        // Check if still AFK (player might have returned during delay)
        if (!isAFK || player == null)
        {
            yield break;
        }

        // Find scene-specific VFX
        currentSceneVFX = GetVFXForCurrentScene();

        GameObject prefabToSpawn = null;
        Vector3 offset = defaultVfxOffset;
        bool shouldFollow = true;

        if (currentSceneVFX != null && currentSceneVFX.vfxPrefab != null)
        {
            prefabToSpawn = currentSceneVFX.vfxPrefab;
            offset = currentSceneVFX.spawnOffset;
            shouldFollow = currentSceneVFX.followPlayer;
            Debug.Log($"[AFKManager] Using scene-specific VFX for '{currentSceneVFX.sceneName}'");
        }
        else if (defaultAFKVfxPrefab != null)
        {
            prefabToSpawn = defaultAFKVfxPrefab;
            Debug.Log("[AFKManager] Using default AFK VFX");
        }

        if (prefabToSpawn == null)
        {
            Debug.Log("[AFKManager] No AFK VFX configured for this scene");
            yield break;
        }

        // Spawn VFX
        Vector3 spawnPosition = player.transform.position + offset;
        currentVFXInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        currentVFXInstance.name = "AFK_VFX_Instance";

        // Make sure all particle systems start fresh
        ParticleSystem[] particleSystems = currentVFXInstance.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            ps.Clear();
            ps.Play();
        }

        // Start following player if needed
        if (shouldFollow)
        {
            if (vfxFollowCoroutine != null) StopCoroutine(vfxFollowCoroutine);
            vfxFollowCoroutine = StartCoroutine(VFXFollowPlayerCoroutine(offset));
        }

        Debug.Log("[AFKManager] AFK VFX spawned");
        vfxSpawnCoroutine = null;
    }

    private void DespawnAFKVFX()
    {
        // Cancel pending spawn
        if (vfxSpawnCoroutine != null)
        {
            StopCoroutine(vfxSpawnCoroutine);
            vfxSpawnCoroutine = null;
        }

        // Stop following
        if (vfxFollowCoroutine != null)
        {
            StopCoroutine(vfxFollowCoroutine);
            vfxFollowCoroutine = null;
        }

        if (currentVFXInstance == null) return;

        // Stop particles gracefully (let existing particles finish)
        ParticleSystem[] particleSystems = currentVFXInstance.GetComponentsInChildren<ParticleSystem>();
        float longestDuration = 0f;

        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            // Find the longest particle lifetime
            float lifetime = ps.main.startLifetime.constantMax;
            if (lifetime > longestDuration)
            {
                longestDuration = lifetime;
            }
        }

        // Destroy after particles finish
        Destroy(currentVFXInstance, longestDuration + 0.1f);
        currentVFXInstance = null;

        Debug.Log("[AFKManager] AFK VFX stopping gracefully");
    }

    private SceneAFKVFX GetVFXForCurrentScene()
    {
        if (sceneVFXList == null || sceneVFXList.Length == 0)
            return null;

        string currentSceneName = SceneManager.GetActiveScene().name;

        foreach (var sceneVFX in sceneVFXList)
        {
            if (sceneVFX != null && sceneVFX.sceneName == currentSceneName)
            {
                return sceneVFX;
            }
        }

        return null;
    }

    private IEnumerator VFXFollowPlayerCoroutine(Vector3 offset)
    {
        while (currentVFXInstance != null && player != null && isAFK)
        {
            currentVFXInstance.transform.position = player.transform.position + offset;
            yield return null;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Manually reset the AFK timer (call this from other scripts if needed)
    /// </summary>
    public void ForceResetAFK()
    {
        ResetAFKTimer();
    }

    /// <summary>
    /// Get the remaining time before AFK triggers
    /// </summary>
    public float GetTimeUntilAFK()
    {
        float remaining = afkTimeThreshold - TimeSinceLastInput;
        return Mathf.Max(0f, remaining);
    }

    /// <summary>
    /// Set a new AFK threshold at runtime
    /// </summary>
    public void SetAFKThreshold(float newThreshold)
    {
        afkTimeThreshold = Mathf.Max(1f, newThreshold);
    }

    /// <summary>
    /// Set the zoom level for AFK camera
    /// </summary>
    public void SetZoomLevel(float newZoomSize)
    {
        zoomedInSize = Mathf.Max(1f, newZoomSize);

        // If already AFK, update the zoom
        if (isAFK && !isZooming)
        {
            StartCameraZoom(zoomedInSize, true);
        }
    }

    #endregion

    private void OnDestroy()
    {
        // Clean up VFX
        if (currentVFXInstance != null)
        {
            Destroy(currentVFXInstance);
            currentVFXInstance = null;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnValidate()
    {
        // Ensure valid values in inspector
        afkTimeThreshold = Mathf.Max(1f, afkTimeThreshold);
        originalCameraSize = Mathf.Max(0.5f, originalCameraSize);
        zoomedInSize = Mathf.Max(0.5f, zoomedInSize);
        zoomSpeed = Mathf.Max(0.1f, zoomSpeed);
        normalIdleDuration = Mathf.Max(5f, normalIdleDuration);
        vfxSpawnDelay = Mathf.Max(0f, vfxSpawnDelay);
    }

    /// <summary>
    /// Call this from Inspector context menu to capture current camera settings.
    /// Right-click AFKManager component → "Capture Current Camera Settings"
    /// </summary>
    [ContextMenu("Capture Current Camera Settings")]
    private void CaptureCurrentCameraSettings()
    {
        CinemachineCamera cam = cinemachineCamera;
        if (cam == null)
        {
            cam = FindFirstObjectByType<CinemachineCamera>();
        }

        if (cam == null)
        {
            Debug.LogError("[AFKManager] No CinemachineCamera found!");
            return;
        }

        // Capture camera size
        originalCameraSize = cam.Lens.OrthographicSize;
        Debug.Log($"[AFKManager] Captured OrthographicSize: {originalCameraSize}");

        // Capture Position Composer settings
        var composer = cam.GetComponent<CinemachinePositionComposer>();
        if (composer != null)
        {
            originalTrackedObjectOffset = composer.TargetOffset;
            originalDeadZoneSize = composer.Composition.DeadZone.Size;
            Debug.Log($"[AFKManager] Captured TargetOffset: {originalTrackedObjectOffset}");
            Debug.Log($"[AFKManager] Captured DeadZone: {originalDeadZoneSize}");
        }

        // Capture Follow settings
        var follow = cam.GetComponent<CinemachineFollow>();
        if (follow != null)
        {
            originalFollowOffset = follow.FollowOffset;
            Debug.Log($"[AFKManager] Captured FollowOffset: {originalFollowOffset}");
        }

        Debug.Log("[AFKManager] All camera settings captured! Remember to save the scene.");

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}