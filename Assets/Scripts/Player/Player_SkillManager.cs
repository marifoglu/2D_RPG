using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash dash { get; private set; }
    public Skill_Shard shard { get; private set; }
    public Skill_SwordThrow swordThrow { get; private set; }
    public Skill_TimeEcho timeEcho { get; private set; }
    public Skill_DomainExpansion domainExpansion { get; private set; }
    public Skill_Backstab backstab { get; private set; }
    public Skill_Base[] allSkills { get; private set; }


    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        shard = GetComponentInChildren<Skill_Shard>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();
        timeEcho = GetComponentInChildren<Skill_TimeEcho>();
        backstab = GetComponentInChildren<Skill_Backstab>();
        domainExpansion = GetComponentInChildren<Skill_DomainExpansion>();

        allSkills = new Skill_Base[] { dash, shard, swordThrow, timeEcho, backstab, domainExpansion };

    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch(type)
        {
            case SkillType.Dash: return dash;
            case SkillType.TimeShard: return shard;
            case SkillType.SwordThrow: return swordThrow;
            case SkillType.TimeEcho: return timeEcho;
            case SkillType.Backstab: return backstab;
            case SkillType.DomainExpansion: return domainExpansion;

            default:
                Debug.Log($"Skill type {type} is not implented yet.");
                return null;

        }
        ;
    }
}
