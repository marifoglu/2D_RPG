using UnityEngine;

[CreateAssetMenu(fileName = "Experience Config", menuName = "RPG Setup/Experience Configuration")]
public class ExperienceConfigSO : ScriptableObject
{
    [Header("Experience Values by Enemy Rarity")]
    [Tooltip("Experience gained from killing a Common enemy")]
    public int commonEnemyExp = 10;

    [Tooltip("Experience gained from killing an Uncommon enemy")]
    public int uncommonEnemyExp = 25;

    [Tooltip("Experience gained from killing a Rare enemy")]
    public int rareEnemyExp = 50;

    [Tooltip("Experience gained from killing an Epic enemy")]
    public int epicEnemyExp = 100;

    [Tooltip("Experience gained from killing a Legendary enemy")]
    public int legendaryEnemyExp = 200;

    [Header("Skill Point Settings")]
    [Tooltip("Experience required to earn 1 skill point")]
    public int expPerSkillPoint = 100;

    public int GetExperienceForRarity(EnemyRarity rarity)
    {
        switch (rarity)
        {
            case EnemyRarity.Common:
                return commonEnemyExp;
            case EnemyRarity.Uncommon:
                return uncommonEnemyExp;
            case EnemyRarity.Rare:
                return rareEnemyExp;
            case EnemyRarity.Epic:
                return epicEnemyExp;
            case EnemyRarity.Legendary:
                return legendaryEnemyExp;
            default:
                return commonEnemyExp;
        }
    }

    public Color GetRarityColor(EnemyRarity rarity)
    {
        switch (rarity)
        {
            case EnemyRarity.Common:
                return Color.white;
            case EnemyRarity.Uncommon:
                return Color.green;
            case EnemyRarity.Rare:
                return Color.blue;
            case EnemyRarity.Epic:
                return new Color(0.6f, 0f, 1f); // Purple
            case EnemyRarity.Legendary:
                return new Color(1f, 0.5f, 0f); // Orange
            default:
                return Color.white;
        }
    }
}