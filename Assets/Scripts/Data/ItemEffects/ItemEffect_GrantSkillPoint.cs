using UnityEngine;


[CreateAssetMenu(fileName = "Item effect data - Grant skill point", menuName = "RPG Setup/Item data/Item effect/Grant skill point effect")]
public class ItemEffect_GrantSkillPoint : ItemEffectDataSO
{
    [SerializeField] private int pointsToAdd;

    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.AddSkillPoints(pointsToAdd);
    }
}
