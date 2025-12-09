using UnityEngine;

[CreateAssetMenu(fileName = "Item effect data - Heal on damage", menuName = "RPG Setup/Item data/Item effect/Heal on damage")]
public class ItemEffect_HealOnDamage : ItemEffectDataSO
{
    [SerializeField] private float percentHealedOnAttack;

    private void HealOnDoingDamage(float damage)
    {
        player.health.increaseHealth(damage * percentHealedOnAttack);
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.onDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.onDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }
}
