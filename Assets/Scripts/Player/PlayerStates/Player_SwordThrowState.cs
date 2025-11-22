using UnityEngine;

public class Player_SwordThrowState : PlayerState
{
    private Camera mainCamera;
    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        skillManager.swordThrow.EnableDots(true);

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    public override void Update()
    {
        base.Update();

        Vector2 direction = DirectionToMouse();

        player.SetVelocity(0f, rb.linearVelocity.y);
        player.HandleFlip(direction.x);
        skillManager.swordThrow.predictTrajectory(direction);
        
        if (input.PlayerCharacter.Attack.WasPressedThisFrame())
        {
            anim.SetBool("SwordThrowPerformed", true);

            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(direction);
        }

        if(input.PlayerCharacter.RangeAttack.WasReleasedThisFrame() || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("SwordThrowPerformed", false);
        skillManager.swordThrow.EnableDots(false);
    }

    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);
        Vector2 direction = worldMousePosition - playerPosition;

        return direction.normalized;
    }

}
