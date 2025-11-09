using UnityEngine;
using System.Collections;

public class Enemy_DeadState : EnemyState
{
    private Collider2D col;

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();

        // Stop all movement immediately
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        // Optionally freeze the rigidbody completely to prevent any physics
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Disable collision
        if (col != null)
            col.enabled = false;

        // Stop state updates
        stateMachine.switchOffStateMachine();

        // Remove enemy after animation finishes (e.g. 2-3 seconds)
        enemy.StartCoroutine(DestroyAfterDelay(2.5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Object.Destroy(enemy.gameObject);
    }
}