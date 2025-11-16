using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    [Header("General Details")]
    [SerializeField] private SkillType skillType;
    [SerializeField] private SkillUpgradeType upgradeType;
    [SerializeField] private float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        lastTimeUsed -= cooldown;
    }

    public void SetSkillUpgrade(SkillUpgradeType upgrade)
    {
        upgradeType = upgrade;
    }
    public bool CanUseSkill()
    {
        if (OnCoolDown())
        {
            Debug.Log("Skill on Cooldown");
            return false;
        }

        return true;
    }

    private bool OnCoolDown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;
    public void ResetCooldown() => lastTimeUsed = Time.time;

}
