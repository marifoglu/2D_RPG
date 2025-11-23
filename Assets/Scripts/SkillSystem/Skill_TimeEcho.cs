using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;

        CreateTimeEcho();
    }

    public void CreateTimeEcho()
    {
        GameObject timeEcho = Instantiate(timeEchoPrefab, transform.position, transform.rotation);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
