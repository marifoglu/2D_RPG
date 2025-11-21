using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    protected SpriteRenderer sr;
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

    [Header("Element VFX")]
    [SerializeField] private Color chillVfx = Color.cyan;
    [SerializeField] private Color burnVfx = Color.red;
    [SerializeField] private Color electricVfx = Color.yellow;
    private Color originalHitVfxColor;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

  

    public void PlayStatusVfx(float duration, ElementType elementType)
    {
        if (elementType == ElementType.Ice)
            StartCoroutine(PlayStatusVfxCo(duration, chillVfx));

        if (elementType == ElementType.Fire)
            StartCoroutine(PlayStatusVfxCo(duration, burnVfx));

        if(elementType == ElementType.Lighting)
            StartCoroutine(PlayStatusVfxCo(duration, electricVfx));
    }

    public void StopAllVFX()
    {
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }
    private IEnumerator PlayStatusVfxCo(float duration, Color effectColor)
    {
        float tickInterval = .25f;
        float timeHasPassed = 0f;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * .9f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            yield return new WaitForSeconds(tickInterval);

            timeHasPassed += tickInterval;
        }

        sr.color = Color.white;

    }
    public void CreateOnHitVFX(Transform target, bool isCrit, ElementType elementType)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        //vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(elementType);

        if (entity != null && entity.facingDir == -1 && isCrit)
            vfx.transform.Rotate(0f, 180f, 0);
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxCoroutine != null)
            StopCoroutine(onDamageVfxCoroutine);

        onDamageVfxCoroutine = StartCoroutine(OnDamageVfxCo());
    }

    public Color GetElementColor(ElementType elementType)
    {

        switch(elementType)
        {
            case ElementType.Ice:
                return chillVfx;
            case ElementType.Fire:
                return burnVfx;
            case ElementType.Lighting:
                return electricVfx;

            default:
                return Color.white;
        }
      
    }
    private IEnumerator OnDamageVfxCo()
    {
        sr.material = onDamageMaterial;
        yield return new WaitForSeconds(onDamageVfxDuration);
        sr.material = originalMaterial;
    }

    public void CreateCounterAttackVFX(Transform target)
    {
        if (counterAttackVFX != null)
        {
            Instantiate(counterAttackVFX, target.position, Quaternion.identity);
        }
    }
}