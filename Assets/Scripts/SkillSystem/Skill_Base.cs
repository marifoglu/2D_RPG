using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player player { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public DamageScaleData damageScaleData { get; protected set; }

    [Header("General Details")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        player = GetComponentInParent<Player>();
        skillManager = GetComponentInParent<Player_SkillManager>(); 
        lastTimeUsed -= cooldown;
        damageScaleData = new DamageScaleData();
    }

    public virtual void TryUseSkill()
    {
    }

    public void SetSkillUpgrade(UpgradeData upgrade)
    {
        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;
    }
    public virtual bool CanUseSkill()
    {
        if(upgradeType == SkillUpgradeType.None)
            return false;

        if (OnCoolDown())
        {
            Debug.Log("Skill on Cooldown");
            return false;
        }
        return true;
    }


    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;
    protected bool OnCoolDown() => Time.time < lastTimeUsed + cooldown;
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;
    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;
    public void ResetCooldown() => lastTimeUsed = Time.time;

}
