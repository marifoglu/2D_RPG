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

        // Stop movement and disable collision
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        if (col != null) col.enabled = false;

        // Trigger your "isDead" animation in Animator
        anim.SetTrigger("dead");

        // Stop state updates
        stateMachine.switchOffStateMachine();

        // Remove enemy after animation finishes (e.g. 3 seconds)
        enemy.StartCoroutine(DestroyAfterDelay(3f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Object.Destroy(enemy.gameObject);
    }
}
