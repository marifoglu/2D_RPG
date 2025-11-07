using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity entity;

    [Header("Taking Damage VFX")]
    [SerializeField] private Material onDamageMaterial;
    [SerializeField] private float onDamageVfxDuration = 0.2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    [Header("Doing Damage VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [SerializeField] private GameObject hitVfx;
    [SerializeField] private GameObject critHitVfx;


    [Header("Counter Attack VFX")]
    [SerializeField] private GameObject counterAttackVFX; // for counter attack effects

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            originalMaterial = sr.material;

        entity = GetComponent<Entity>();
    }

    public void CreateOnHitVFX(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        if (hitPrefab == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Hit VFX prefab is not assigned!");
            return;
        }

        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);

        // Check if the instantiated VFX has a SpriteRenderer before trying to access it
        SpriteRenderer vfxRenderer = vfx.GetComponent<SpriteRenderer>();
        if (vfxRenderer != null)
        {
            vfxRenderer.color = hitVfxColor;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] VFX prefab '{hitPrefab.name}' doesn't have a SpriteRenderer component!");
        }

        if (entity != null && entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0f, 180f, 0);
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {
        if (sr == null)
            yield break;

        Color originalColor = sr.color;
        sr.color = Color.paleVioletRed;

        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.color = originalColor;

        //sr.material = onDamageMaterial;
        //sr.material = originalMaterial;
        //yield return new WaitForSeconds(onDamageVfxDuration);
    }

    public void CreateCounterAttackVFX(Transform target)
    {
        if (counterAttackVFX != null)
        {
            Instantiate(counterAttackVFX, target.position, Quaternion.identity);
        }
    }
}