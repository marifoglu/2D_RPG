using UnityEngine;

[CreateAssetMenu(fileName = "Skill data - ", menuName = "RPG Setup/Skill data")]
public class Skill_DataSO : ScriptableObject
{
    public int cost;
    public SkillType skillType;
    public SkillUpgradeType upgradeType;

    [Header("Skill description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
}
