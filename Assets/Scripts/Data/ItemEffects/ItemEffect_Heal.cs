using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Heal", menuName = "RPG Setup/Item data/Item effect/Heal effect")]
public class ItemEffect_Heal : ItemEffectDataSO
{
    [SerializeField] private float healPercent = .24f;

    public override void ExecuteEffect()
    {
        Player player = FindFirstObjectByType<Player>();

        float healAmount = player.stats.GetMaxHealth() * healPercent;

        player.health.increaseHealth(healAmount);
    }
}
