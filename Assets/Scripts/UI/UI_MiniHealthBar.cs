using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{

    private Entity entitiy;

    private void Awake()
    {
        entitiy = GetComponentInParent<Entity>();
    }
    private void OnEnable()
    {
        entitiy.OnFlipped += HandleFlip;
    }
    //private void HandleFlip() => transform.rotation = Quaternion.identity;

    private void OnDisabled()
    {
        entitiy.OnFlipped -= HandleFlip;
    }
    private void HandleFlip() => transform.rotation = Quaternion.identity;


}
