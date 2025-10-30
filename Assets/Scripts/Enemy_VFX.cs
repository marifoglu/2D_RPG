using UnityEngine;

public class Enemy_VFX : Entity_VFX
{
    [Header("Counter Attack Window")]
    [SerializeField] private GameObject attackAllert;

    public void EnableAttackAlert(bool enable)
    {
        if (attackAllert == null)
            return;

        attackAllert.SetActive(enable);
    }

}
