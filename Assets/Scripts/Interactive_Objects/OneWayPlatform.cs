using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        // Get the Player component, NOT PlayerInputSet!
        var player = collision.collider.GetComponent<Player>();
        if (player == null)
            return;

        // Access input through the Player
        Vector2 movement = player.input.PlayerCharacter.Movement.ReadValue<Vector2>();
        if (movement.y < -0.5f) // pressed DOWN
        {
            StartCoroutine(DisablePlatformTemporarily(collision.collider));
        }
    }

    private System.Collections.IEnumerator DisablePlatformTemporarily(Collider2D playerCollider)
    {
        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);

        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
    }
}