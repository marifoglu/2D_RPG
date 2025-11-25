using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVFX;
    [SerializeField] private LayerMask whatIsGround;
    private Skill_TimeEcho echoManager;
    public int maxAttacks { get; private set; }


    private void Update()
    {
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        StopHroizantalMovement();
    }
    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        entityStats = echoManager.player.stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();

        FlipToTarget();
        anim.SetBool("canAttack", maxAttacks > 0);

        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
    }
    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1);

        if (targetGotHit == false)
            return;

        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 1f : -1f;

        if (canDuplicate)
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0));
    }

    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();

        if(target != null && target.position.x < transform.position.x)
            transform.Rotate(0f, 180f, 0f);
        
    }



    private void StopHroizantalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);

        if(hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }   
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
