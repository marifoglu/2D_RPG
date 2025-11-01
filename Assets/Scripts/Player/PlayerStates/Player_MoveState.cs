using UnityEngine;

public class Player_MoveState : PlayerState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    { }

    public override void Update()
    {
        base.Update();

        float x = player.moveInput.x;

        // ✅ Handle Jump
        if (input.PlayerCharacter.Jump.WasPressedThisFrame() && player.groundDetected)
        {
            stateMachine.ChangeState(player.jumpState);
            return;
        }

        // ✅ If no longer grounded, fall
        if (!player.groundDetected)
        {
            stateMachine.ChangeState(player.fallState);
            return;
        }

        // ✅ Stop moving ONLY if x == 0
        if (x == 0f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // ✅ Wall check: if grounded + wallDetected and still moving TOWARD wall, stop movement but stay in MoveState
        if (player.wallDetected && player.moveInput.x == player.facingDir)
        {
            // Cancel X velocity (like a solid wall), but stay in MoveState to allow turn or jump
            player.SetVelocity(0f, rb.linearVelocity.y);
            return;
        }

        // ✅ Normal movement
        player.SetVelocity(x * player.moveSpeed, rb.linearVelocity.y);

        // ✅ Flip sprite if needed
        if (x != 0 && Mathf.Sign(x) != player.facingDir)
            player.Flip();
    }
}
