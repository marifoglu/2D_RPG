using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{

    private Skill_TimeEcho echoManager;

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
    }
    private void Update()
    {
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
}
