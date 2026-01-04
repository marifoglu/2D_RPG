//using UnityEngine;

//public class Player_Health : Entity_Health
//{
//    private Player player;

//    protected override void Awake()
//    {
//        base.Awake();
//        player = GetComponent<Player>();
//    }
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.B))
//            Die();
//    }

//    protected override void Die()
//    {
//        base.Die();

//        player.ui.OpenDeathScreenUI();
//    }
//}
using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.B))
    //        Die();
    //}

    protected override void Die()
    {
        base.Die();

        player.ui.OpenDeathScreenUI();
    }
}