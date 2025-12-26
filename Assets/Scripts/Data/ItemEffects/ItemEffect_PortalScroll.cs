using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Item effect data - Portal Scroll", menuName = "RPG Setup/Item data/Item effect/Portal scroll")]
public class ItemEffect_PortalScroll : ItemEffectDataSO
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        if(SceneManager.GetActiveScene().name == "Demo_Level_0")
        {
            Debug.Log("Can't open portal in town");
            return;
        }

        Player player = Player.instance;
        Vector3 portalPosition = player.transform.position + new Vector3(player.facingDir * 1.5f, 0);

        Object_Portal.instance.ActivatePortal(portalPosition, player.facingDir);
    }
}
