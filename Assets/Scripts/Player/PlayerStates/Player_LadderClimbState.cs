using UnityEngine;

public class Player_LadderClimbState : PlayerState
{
    private Ladder currentLadder;
    private float originalGravity;
    private bool isClimbing;
    private bool isTransitioning;

    public Player_LadderClimbState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("=== LADDER ENTER ===");

        currentLadder = player.GetCurrentLadder();
        Debug.Log($"Current Ladder: {(currentLadder != null ? currentLadder.name : "NULL")}");

        if (currentLadder == null)
        {
            Debug.LogError("LADDER IS NULL! Going to idle");
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Check if player is ABOVE the ladder top - don't allow grab
        Vector2 ladderTop = currentLadder.GetTopPosition();
        if (player.transform.position.y > ladderTop.y + 0.3f)
        {
            Debug.LogWarning("PLAYER ABOVE LADDER TOP - can't grab from above");
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // Store and disable gravity
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        Debug.Log($"Gravity disabled. Was: {originalGravity}, Now: {rb.gravityScale}");

        // Stop all velocity
        rb.linearVelocity = Vector2.zero;

        isClimbing = false;
        isTransitioning = false;

        // Snap player to ladder X position for clean climbing
        SnapToLadder();
        Debug.Log($"Snapped to ladder at X: {player.transform.position.x}");
        Debug.Log("=== LADDER ENTER COMPLETE ===");
    }

    public override void Update()
    {
        base.Update();

        // Safety check
        if (currentLadder == null || !player.IsOnLadder())
        {
            Debug.LogWarning($"EXITING: currentLadder={currentLadder}, IsOnLadder={player.IsOnLadder()}");
            ExitLadder();
            return;
        }

        float verticalInput = player.moveInput.y;
        float horizontalInput = player.moveInput.x;

        // Handle climbing movement
        HandleClimbing(verticalInput);

        // Handle jump off ladder
        if (input.PlayerCharacter.Jump.WasPressedThisFrame())
        {
            Debug.Log("JUMP pressed - jumping off ladder");
            JumpOffLadder(horizontalInput);
            return;
        }

        // Handle reaching top/bottom
        HandleLadderBounds();

        // Update animation speed based on movement
        UpdateAnimationSpeed(verticalInput);
    }

    public override void Exit()
    {
        base.Exit();

        // Restore gravity
        rb.gravityScale = originalGravity;

        // Reset animation speed
        anim.speed = 1f;

        currentLadder = null;
        isClimbing = false;
        isTransitioning = false;
    }

    private void HandleClimbing(float verticalInput)
    {
        if (isTransitioning)
            return;

        if (currentLadder == null)
            return;

        float climbSpeed = currentLadder.GetClimbSpeed();

        // Allow very slight horizontal adjustment for centering
        float horizontalAdjustment = 0f;
        float ladderCenterX = currentLadder.transform.position.x;
        float distanceFromCenter = Mathf.Abs(player.transform.position.x - ladderCenterX);

        if (distanceFromCenter > 0.1f)
        {
            horizontalAdjustment = (ladderCenterX - player.transform.position.x) * 2f;
            horizontalAdjustment = Mathf.Clamp(horizontalAdjustment, -1f, 1f);
        }

        // Set velocity to climb speed * input (this prevents sliding)
        rb.linearVelocity = new Vector2(horizontalAdjustment, verticalInput * climbSpeed);

        isClimbing = Mathf.Abs(verticalInput) > 0.1f;

        if (isClimbing)
        {
            Debug.Log($"CLIMBING: Input={verticalInput:F2}, Velocity={rb.linearVelocity.y:F2}, Speed={climbSpeed}");
        }
    }

    private void HandleLadderBounds()
    {
        // Safety checks
        if (currentLadder == null || player == null || player.transform == null)
        {
            Debug.LogWarning("HandleLadderBounds: NULL detected, exiting");
            ExitLadder();
            return;
        }

        // Exit when grounded AND climbing down (not up!)
        if (player.groundDetected && player.moveInput.y < -0.1f)
        {
            Debug.Log("GROUNDED + CLIMBING DOWN - exiting to idle");
            ExitLadder();
            return;
        }

        // Check if reached top - snap to top and exit to idle
        if (currentLadder.IsPlayerNearTop(player.transform, 0.1f))
        {
            Debug.Log($"NEAR TOP: Input Y={player.moveInput.y}, Transitioning={isTransitioning}");
            if (!isTransitioning)
            {
                // Start the snap + exit coroutine (clears animation flags, nudges player out of ladder zone,
                // restores gravity and then changes to idle on the next frame)
                var topPos = currentLadder.GetTopPosition();
                player.StartCoroutine(SnapAndExitCo(topPos));
                return;
            }
        }
    }

    private System.Collections.IEnumerator SnapAndExitCo(Vector2 topPos)
    {
        if (currentLadder == null || player == null)
            yield break;

        isTransitioning = true;

        // Snap horizontally to ladder center and nudge slightly ABOVE the top so player is outside ladder trigger
        float ladderX = currentLadder.transform.position.x;
        const float topNudge = 0.12f; // tweak in inspector if needed
        player.transform.position = new Vector2(ladderX, topPos.y + topNudge);

        // Ensure zero velocity and restore gravity
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravity;

        // Clear ladder state so DetectLadder will begin ignore window (Player.SetLadderState sets ignore)
        player.SetLadderState(false, null);

        // Clear ladder animation parameter and reset speed to ensure Idle animation can play
        anim.SetBool("LadderClimb", false);
        anim.speed = 1f;

        Debug.Log("REACHED LADDER TOP - snapped above top, restoring gravity and exiting to idle next frame");

        // Wait one frame so physics/overlap and animator have time to update before changing state
        yield return null;

        // Final safety: ensure we're not immediately re-grabbed
        isTransitioning = false;

        stateMachine.ChangeState(player.idleState);
    }

    private void ClimbOffTop()
    {
        // NOT USED ANYMORE - just exit to idle
        ExitLadder();
    }

    private System.Collections.IEnumerator ExitAfterFrame()
    {
        // NOT USED ANYMORE
        yield return null;
    }

    private void JumpOffLadder(float horizontalInput)
    {
        // Determine jump direction
        int jumpDir = horizontalInput != 0 ? (int)Mathf.Sign(horizontalInput) : player.facingDir;

        // Apply jump force
        Vector2 jumpForce = new Vector2(jumpDir * player.wallJumpForce.x * 0.7f, player.jumpForce * 0.8f);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = jumpForce;

        player.SetLadderState(false, null);
        stateMachine.ChangeState(player.jumpState);
    }

    private void ExitLadder()
    {
        player.SetLadderState(false, null);

        // Restore gravity and zero velocity
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;

        Debug.Log("EXITED LADDER - going to idle");

        // Go to idle - player stays where they are
        stateMachine.ChangeState(player.idleState);
    }

    private void SnapToLadder()
    {
        if (currentLadder == null)
            return;

        float ladderX = currentLadder.transform.position.x;
        player.transform.position = new Vector2(ladderX, player.transform.position.y);
    }

    private void UpdateAnimationSpeed(float verticalInput)
    {
        // Pause animation when not moving, play at normal speed when moving
        float animSpeed = Mathf.Abs(verticalInput) > 0.1f ? 1f : 0f;
        anim.speed = animSpeed;
    }

    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();
        anim.SetBool("LadderClimb", true);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}