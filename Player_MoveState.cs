using UnityEngine;

public class Player_MoveState : PlayerGroundedState
{



    override public void Update()
    {
        base.Update();

        if (player.moveInput.x == 0)
            stateMachine.ChangeState(player.idleState);

        player.SetVelocity(player.moveInput.x * player.moveSpeed, player.rb.linearVelocity.y);

        //// Flip the player in the direction they are moving
        //if (player.moveInput.x > 0)
        //    player.transform.localScale = new Vector3(1, 1, 1);
        //else if (player.moveInput.x < 0)
        //    player.transform.localScale = new Vector3(-1, 1, 1);
    }

}
