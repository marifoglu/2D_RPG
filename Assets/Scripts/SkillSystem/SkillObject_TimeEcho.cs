using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVFX;
    [SerializeField] private LayerMask whatIsGround;
    private Skill_TimeEcho echoManager;

    private void Update()
    {
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        StopHroizantalMovement();
    }

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;

        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration);
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void StopHroizantalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);

        if(hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }   
    }
}
