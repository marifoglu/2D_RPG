using UnityEngine;

public class UI_MiniHealthBar : MonoBehaviour
{

    private Entitiy entitiy;

    private void Awake()
    {
        entitiy = GetComponentInParent<Entitiy>();
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
