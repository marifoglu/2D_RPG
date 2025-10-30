using UnityEngine;
public class Enemy_GroundedState : EnemyState
{
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (enemy.PlayerDetected() == true)
            stateMachine.ChangeState(enemy.battleState);
    }

}

//using UnityEngine;

//public class Enemy_GroundedState : EnemyState
//{
//    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
//    {
//    }

//    public override void Update()
//    {
//        base.Update();

//        var hit = enemy.PlayerDetected();
//        if (hit && hit.transform != null)
//        {
//            var playerHealth = hit.transform.GetComponent<Entity_Health>();
//            if (playerHealth != null && !playerHealth.isDead)
//            {
//                stateMachine.ChangeState(enemy.battleState);
//            }
//        }
//    }
//}
