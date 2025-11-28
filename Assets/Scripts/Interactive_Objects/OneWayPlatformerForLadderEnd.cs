using System.Collections;
using UnityEngine;

public class OneWayPlatformerForLadderEnd : MonoBehaviour
{
    [Header("Drop-Through Settings")]
    [SerializeField] private float dropThroughSpeed = 10f;
    [SerializeField] private float dropThroughInputThreshold = -0.5f;
    [SerializeField] private float collisionReEnableDelay = 0.08f;

    [Header("Landing Settings")]
    [SerializeField] private bool enableLandingFix = true;
    [SerializeField] private float landingVelocityThreshold = -1.5f;
    [SerializeField] private float landingGravityMultiplier = 2f;
    [SerializeField] private float landingFixDuration = 0.25f;

    [Header("Physics Settings")]
    [SerializeField] private float restoreVelocity = -6f;
    [SerializeField] private float dropTargetOffset = 0.5f;

    private Collider2D platformCollider;
    private bool isProcessingDropThrough = false;
    private Player currentPlayer = null;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (currentPlayer != null && !isProcessingDropThrough && !currentPlayer.isDroppingThroughPlatform)
        {
            Vector2 movement = currentPlayer.moveInput;

            // Just check for down input - no jump required
            if (movement.y < dropThroughInputThreshold)
            {
                var playerColliders = currentPlayer.GetComponentsInChildren<Collider2D>(includeInactive: true);
                StartCoroutine(DisablePlatformTemporarily(playerColliders, currentPlayer));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            var player = collision.collider.GetComponent<Player>();
            if (player == null)
                player = collision.collider.GetComponentInParent<Player>();

            if (player != null)
            {
                currentPlayer = player;

                if (enableLandingFix)
                {
                    var rb = player.GetComponent<Rigidbody2D>();
                    if (rb != null && rb.linearVelocity.y < landingVelocityThreshold)
                    {
                        StartCoroutine(FixSlowLanding(player, rb));
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            currentPlayer = null;
        }
    }

    private IEnumerator FixSlowLanding(Player player, Rigidbody2D rb)
    {
        float originalGravity = rb.gravityScale;
        bool wasDropping = player.isDroppingThroughPlatform;

        var playerCollider = player.GetComponent<Collider2D>();
        PhysicsMaterial2D originalMaterial = null;
        if (playerCollider != null)
        {
            originalMaterial = playerCollider.sharedMaterial;
        }

        player.isDroppingThroughPlatform = true;
        rb.gravityScale = originalGravity * landingGravityMultiplier;

        if (playerCollider != null)
        {
            var tempNoFriction = new PhysicsMaterial2D("TempLandingBoost")
            {
                friction = 0f,
                bounciness = 0f
            };
            playerCollider.sharedMaterial = tempNoFriction;
        }

        yield return new WaitForSeconds(landingFixDuration);

        if (rb != null)
        {
            rb.gravityScale = originalGravity;
        }

        if (player != null)
        {
            player.isDroppingThroughPlatform = wasDropping;
        }

        if (playerCollider != null && originalMaterial != null)
        {
            playerCollider.sharedMaterial = originalMaterial;
        }
    }

    private IEnumerator DisablePlatformTemporarily(Collider2D[] playerColliders, Player player)
    {
        isProcessingDropThrough = true;

        var rb = player.GetComponent<Rigidbody2D>();
        float platformBottom = platformCollider.bounds.min.y;

        foreach (var pc in playerColliders)
        {
            if (pc != null)
            {
                Physics2D.IgnoreCollision(platformCollider, pc, true);
            }
        }

        if (rb != null)
        {
            player.StartDropThrough();

            RigidbodyType2D originalBodyType = rb.bodyType;
            rb.bodyType = RigidbodyType2D.Kinematic;

            float startY = player.transform.position.y;
            float targetY = platformBottom - dropTargetOffset;
            float dropDistance = startY - targetY;
            float dropDuration = dropDistance / dropThroughSpeed;

            float elapsedTime = 0f;
            Vector3 startPos = player.transform.position;
            Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

            while (elapsedTime < dropDuration && player != null)
            {
                elapsedTime += Time.fixedDeltaTime;
                float progress = elapsedTime / dropDuration;

                float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
                Vector3 currentPos = Vector3.Lerp(startPos, targetPos, smoothProgress);
                player.transform.position = currentPos;

                yield return new WaitForFixedUpdate();
            }

            if (player != null && rb != null)
            {
                rb.bodyType = originalBodyType;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, restoreVelocity);
            }
        }

        yield return new WaitForSeconds(collisionReEnableDelay);

        foreach (var pc in playerColliders)
        {
            if (pc != null)
            {
                Physics2D.IgnoreCollision(platformCollider, pc, false);
            }
        }

        if (player != null)
        {
            player.EndDropThrough();
        }

        isProcessingDropThrough = false;
    }
}