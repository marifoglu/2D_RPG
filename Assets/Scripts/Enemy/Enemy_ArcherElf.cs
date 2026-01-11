using UnityEngine;

public class Enemy_ArcherElf : Enemy, ICounterable
{
    public Enemy_ArchElf_BattleState elfBattleState { get; set; }

    [Header("Archer Elf Specifics")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowStartPoint;
    [SerializeField] private float arrowSpeed = 8;
    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
     
        elfBattleState = new Enemy_ArchElf_BattleState(this, stateMachine, "battle");
        battleState = elfBattleState;

    }
    override protected void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public override void SpecialAttack()
    {
        GameObject newArrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
        newArrow.GetComponent<Enemy_ArchElf_Arrow>().SetupArrow(arrowSpeed * facingDir, combat);
    }
}