using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Refund all skills", menuName = "RPG Setup/Item data/Item effect/Refund all skills")]
public class ItemEffect_RefundAllSkills : ItemEffectDataSO
{
    override public void ExecuteEffect()
    {
        //UI_SkillTree skillTree = FindFirstObjectByType<UI_SkillTree>(FindObjectsInactive.Include);
        //skillTree.RefundAllSkills();

        UI ui = FindFirstObjectByType<UI>();
        ui.skillTree.RefundAllSkills();
    }
}
