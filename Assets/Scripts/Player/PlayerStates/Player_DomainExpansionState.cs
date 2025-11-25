//using UnityEngine;

//public class Player_DomainExpansionState : PlayerState
//{

//    private Vector2 originalPosition;
//    private float originalGravity;
//    private float maxDistanceToGoUp;

//    private bool isLevtating;
//    private bool createdDomain;

//    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
//    {
//    }

//    public override void Enter()
//    {
//        base.Enter();

//        originalPosition = player.transform.position;
//        originalGravity = player.rb.gravityScale;
//        maxDistanceToGoUp = GetAvailableRiseDistance();

//        player.SetVelocity(0, player.riseSpeed);
//    }

//    public override void Update()
//    {
//        base.Update();

//        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && isLevtating == false)
//            Lavitate();

//        if (isLevtating)
//        {
//            if (stateTimer < 0)
//                stateMachine.ChangeState(player.idleState);
//        }
//    }



//    private void Lavitate()
//    {
//        isLevtating = true;
//        rb.linearVelocity = Vector2.zero;
//        rb.gravityScale = 0;

//        stateTimer = 2;

//        if (createdDomain == false)
//        {
//            createdDomain = true;
//            Debug.Log("Domain Expansion Created!");
//        }
//    }

//    private float GetAvailableRiseDistance()
//    {
//        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.up, player.whatIsGround);

//        return hit.collider != null ? hit.distance - 1 : player.riseMaxDistance;
//    }

//    public override void Exit()
//    {
//        base.Exit();
//        rb.gravityScale = originalGravity;
//        isLevtating = false;
//        createdDomain = false;
//    }
//}

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

        Debug.Log($"[DomainExpansion] Enter - originalY={originalPosition.y:F2} maxRise={maxDistanceToGoUp:F2} riseSpeed={player.riseSpeed:F2}");

        // Disable gravity immediately to prevent falling
        rb.gravityScale = 0f;

        player.SetVelocity(0, player.riseSpeed);
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp && isLevitating == false)
            Lavitate();

        if (isLevitating)
        {
            if (stateTimer < 0)
                stateMachine.ChangeState(player.idleState);
        }
    }



    private void Lavitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        stateTimer = 2f;

        if (createdDomain == false)
        {
            createdDomain = true;
            Debug.Log("Domain Expansion Created!");
        }
    }

    private float GetAvailableRiseDistance()
    {
        // Use a small origin offset (so raycast isn't starting inside the ground/ceiling)
        Vector2 origin = (Vector2)player.transform.position + Vector2.up * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, player.riseMaxDistance + 0.1f, player.whatIsGround);

        if (hit.collider != null)
        {
            // Subtract a small buffer so the player doesn't try to intersect the ceiling.
            float available = hit.distance - 0.5f;

            // Ensure the available distance is sane: never negative and not larger than riseMaxDistance.
            available = Mathf.Clamp(available, 0.1f, player.riseMaxDistance);

            Debug.Log($"[DomainExpansion] Ceiling hit at {hit.distance:F2}, using availableRise={available:F2}");
            return available;
        }

        Debug.Log("[DomainExpansion] No ceiling hit, using full riseMaxDistance");
        return player.riseMaxDistance;
    }

    public override void Exit()
    {
        base.Exit();
        rb.gravityScale = originalGravity;
        isLevitating = false;
        createdDomain = false;
    }
}