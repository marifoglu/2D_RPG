using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{

    private Vector2 originalPosition;
    private float originalGravity;
    private float maxDistanceToGoUp;

    private bool isLevitating;
    private bool createdDomain;

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalPosition = player.transform.position;
        originalGravity = player.rb.gravityScale;
        maxDistanceToGoUp = GetAvailableRiseDistance();

        // Disable gravity immediately to prevent falling
        rb.gravityScale = 0f;

        player.SetVelocity(0, player.riseSpeed);
        player.health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        // Calculate current distance traveled
        float currentDistance = Vector2.Distance(originalPosition, player.transform.position);

        if (currentDistance >= maxDistanceToGoUp && isLevitating == false)
        {
            Lavitate();
        }
        else if (!isLevitating)
        {
            // Continue rising but check if we need to slow down near the max distance
            // This prevents overshooting
            float remainingDistance = maxDistanceToGoUp - currentDistance;

            if (remainingDistance < 0.5f)
            {
                // Slow down when very close to max distance
                player.SetVelocity(0, rb.linearVelocity.y * 0.5f);
            }
            else
            {
                // Maintain rise speed
                player.SetVelocity(0, player.riseSpeed);
            }
        }

        if (isLevitating)
        {
            skillManager.domainExpansion.DoSpellCasting();

            if (stateTimer < 0)
            {
                isLevitating = false;
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    private void Lavitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        stateTimer = skillManager.domainExpansion.GetDomainDuration();

        if (createdDomain == false)
        {
            createdDomain = true;
            skillManager.domainExpansion.CreateDomain();
        }
    }

    private float GetAvailableRiseDistance()
    {
        // Use a small origin offset (so raycast isn't starting inside the ground/ceiling)
        Vector2 origin = (Vector2)player.transform.position + Vector2.up * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, player.riseMaxDistance + 0.1f, player.whatIsGround);

        if (hit.collider != null)
        {
            float available = hit.distance - 0.5f;

            available = Mathf.Clamp(available, 0.1f, player.riseMaxDistance);

            return available;
        }
        return player.riseMaxDistance;
    }

    public override void Exit()
    {
        base.Exit();
        //rb.gravityScale = originalGravity;
        createdDomain = false;
        player.health.SetCanTakeDamage(true);

    }
}