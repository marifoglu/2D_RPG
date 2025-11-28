using System.Collections;
using UnityEngine;

public class Player_LadderClimbState : PlayerState
{
    private Ladder currentLadder;
    private float originalGravity;
    private bool isClimbing;
    private bool isExitingTop;
    private Vector2 exitStartPosition;
    private Vector2 exitEndPosition;
    //private float exitProgress;
    private float exitDuration = 0.5f;

    public Player_LadderClimbState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        currentLadder = player.GetCurrentLadder();
        if (currentLadder == null)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        Vector2 ladderTop = currentLadder.GetTopPosition();
        if (player.transform.position.y > ladderTop.y + 0.3f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        isClimbing = false;
        isExitingTop = false;

        SnapToLadder();
    }

    public override void Update()
    {
        base.Update();

        if (isExitingTop)
            return;

        if (currentLadder == null || !player.IsOnLadder())
        {
            ExitLadder();
            return;
        }

        float verticalInput = player.moveInput.y;
        float horizontalInput = player.moveInput.x;

        HandleClimbing(verticalInput);

        if (input.PlayerCharacter.Jump.WasPressedThisFrame())
        {
            JumpOffLadder(horizontalInput);
            return;
        }

        HandleLadderBounds();
        UpdateAnimationSpeed(verticalInput);
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = originalGravity;
        anim.speed = 1f;
        anim.SetBool("LadderClimbEnd", false);

        currentLadder = null;
        isClimbing = false;
        isExitingTop = false;
    }

    private void HandleClimbing(float verticalInput)
    {
        if (isExitingTop || currentLadder == null)
            return;

        float climbSpeed = currentLadder.GetClimbSpeed();
        rb.linearVelocity = new Vector2(0f, verticalInput * climbSpeed);

        isClimbing = Mathf.Abs(verticalInput) > 0.1f;
    }

    private void HandleLadderBounds()
    {
        if (currentLadder == null || player == null)
        {
            ExitLadder();
            return;
        }

        if (player.groundDetected && player.moveInput.y < -0.1f)
        {
            ExitLadder();
            return;
        }

        if (currentLadder.IsPlayerNearTop(player.transform, 0.5f) && player.moveInput.y > 0.1f)
        {
            if (!isExitingTop)
            {
                StartTopExit();
                return;
            }
        }
    }

    private void StartTopExit()
    {
        isExitingTop = true;
        player.SetLadderState(false, null);
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        Vector2 topPos = currentLadder.GetTopPosition();
        float ladderX = currentLadder.transform.position.x;

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        float playerHalfHeight = playerCollider != null ? playerCollider.bounds.extents.y : 1f;

        exitStartPosition = player.transform.position;
        exitEndPosition = new Vector2(ladderX, topPos.y + playerHalfHeight + 0.3f);
        //exitProgress = 0f;

        player.StartCoroutine(TopExitCoroutine());
    }

    private IEnumerator TopExitCoroutine()
    {
        const string climbEndParam = "LadderClimbEnd";
        anim.SetBool(climbEndParam, true);
        anim.speed = 1f;

        yield return null;

        float clipLength = 0f;
        var controller = anim.runtimeAnimatorController;
        if (controller != null)
        {
            var clips = controller.animationClips;
            if (clips != null && clips.Length > 0)
            {
                for (int i = 0; i < clips.Length; i++)
                {
                    if (clips[i].name == "LadderClimbEnd")
                    {
                        clipLength = clips[i].length;
                        break;
                    }
                }
                if (clipLength <= 0f)
                {
                    for (int i = 0; i < clips.Length; i++)
                    {
                        if (clips[i].name.Contains("LadderClimbEnd"))
                        {
                            clipLength = clips[i].length;
                            break;
                        }
                    }
                }
            }
        }

        float duration = clipLength > 0f ? (clipLength / (anim.speed == 0f ? 1f : anim.speed)) : exitDuration;
        duration = Mathf.Max(0.01f, duration);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (player == null) yield break;

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            player.transform.position = Vector2.Lerp(exitStartPosition, exitEndPosition, t);
            yield return null;
        }

        if (player != null)
            player.transform.position = exitEndPosition;

        anim.SetBool(climbEndParam, false);
        isExitingTop = false;
        stateMachine.ChangeState(player.idleState);
    }

    private void JumpOffLadder(float horizontalInput)
    {
        int jumpDir = horizontalInput != 0 ? (int)Mathf.Sign(horizontalInput) : player.facingDir;
        Vector2 jumpForce = new Vector2(jumpDir * player.wallJumpForce.x * 0.7f, player.jumpForce * 0.8f);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = jumpForce;

        player.SetLadderState(false, null);
        stateMachine.ChangeState(player.jumpState);
    }

    private void ExitLadder()
    {
        player.SetLadderState(false, null);
        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;
        stateMachine.ChangeState(player.idleState);
    }

    private void SnapToLadder()
    {
        if (currentLadder == null) return;

        float ladderX = currentLadder.transform.position.x;
        player.transform.position = new Vector2(ladderX, player.transform.position.y);
    }

    private void UpdateAnimationSpeed(float verticalInput)
    {
        if (isExitingTop) return;
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