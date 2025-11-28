using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Settings")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private BoxCollider2D ladderZone;

    [Header("Grab Points")]
    [SerializeField] private Transform topGrabPoint;
    [SerializeField] private Transform bottomGrabPoint;

    private void Awake()
    {
        if (ladderZone == null)
            ladderZone = GetComponent<BoxCollider2D>();

        // Make sure the ladder zone is a trigger
        if (ladderZone != null)
            ladderZone.isTrigger = true;
    }

    public float GetClimbSpeed() => climbSpeed;

    public Vector2 GetTopPosition()
    {
        if (topGrabPoint != null)
            return topGrabPoint.position;

        if (ladderZone != null)
            return (Vector2)transform.position + Vector2.up * (ladderZone.size.y / 2f);

        return transform.position + Vector3.up * 2f; // Fallback
    }

    public Vector2 GetBottomPosition()
    {
        if (bottomGrabPoint != null)
            return bottomGrabPoint.position;

        if (ladderZone != null)
            return (Vector2)transform.position - Vector2.up * (ladderZone.size.y / 2f);

        return transform.position - Vector3.up * 2f; // Fallback
    }

    public bool IsPlayerInZone(Transform player)
    {
        if (ladderZone == null || player == null)
            return false;

        // Check if player position is within ladder bounds with a small margin
        Bounds bounds = ladderZone.bounds;
        Vector2 playerPos = player.position;

        // Add small margin for better feel
        float margin = 0.2f;
        return playerPos.x >= bounds.min.x - margin &&
               playerPos.x <= bounds.max.x + margin &&
               playerPos.y >= bounds.min.y - margin &&
               playerPos.y <= bounds.max.y + margin;
    }

    public bool IsPlayerNearTop(Transform player, float threshold = 0.5f)
    {
        if (player == null)
            return false;

        float distanceToTop = Mathf.Abs(player.position.y - GetTopPosition().y);
        return distanceToTop < threshold;
    }

    public bool IsPlayerNearBottom(Transform player, float threshold = 0.5f)
    {
        if (player == null)
            return false;

        float distanceToBottom = Mathf.Abs(player.position.y - GetBottomPosition().y);
        return distanceToBottom < threshold;
    }

    private void OnDrawGizmos()
    {
        if (ladderZone == null)
            ladderZone = GetComponent<BoxCollider2D>();

        if (ladderZone == null)
            return;

        // Draw ladder zone
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, ladderZone.size);

        // Draw border
        Gizmos.color = Color.green;
        Vector3 size = ladderZone.size;
        Vector3 pos = transform.position;

        Vector3 topLeft = pos + new Vector3(-size.x / 2, size.y / 2);
        Vector3 topRight = pos + new Vector3(size.x / 2, size.y / 2);
        Vector3 bottomLeft = pos + new Vector3(-size.x / 2, -size.y / 2);
        Vector3 bottomRight = pos + new Vector3(size.x / 2, -size.y / 2);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Draw grab points
        Gizmos.color = Color.yellow;
        Vector2 topPos = GetTopPosition();
        Vector2 bottomPos = GetBottomPosition();

        Gizmos.DrawWireSphere(topPos, 0.2f);
        Gizmos.DrawWireSphere(bottomPos, 0.2f);

        // Draw labels
#if UNITY_EDITOR
        UnityEditor.Handles.Label(topPos + Vector2.up * 0.3f, "TOP");
        UnityEditor.Handles.Label(bottomPos + Vector2.down * 0.3f, "BOTTOM");
#endif
    }
}