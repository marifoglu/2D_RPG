using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{
    private MonoBehaviour entityBase;

    private void Awake()
    {
        // Try to get Entity (player)
        entityBase = GetComponentInParent<Entity>();
        if (entityBase == null)
            entityBase = GetComponentInParent<Entity_Enemy>();
    }

    private void OnEnable()
    {
        if (entityBase is Entity e)
            e.OnFlipped += HandleFlip;
        else if (entityBase is Entity_Enemy en)
            en.OnFlipped += HandleFlip;
    }

    private void OnDisable()
    {
        if (entityBase is Entity e)
            e.OnFlipped -= HandleFlip;
        else if (entityBase is Entity_Enemy en)
            en.OnFlipped -= HandleFlip;
    }

    private void HandleFlip() => transform.rotation = Quaternion.identity;
}
