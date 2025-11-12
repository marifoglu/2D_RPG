using System.Collections;
using UnityEngine;

[System.Serializable]
public class Buff
{
    public StatType type;
    public float value;
}

public class Object_Buff : MonoBehaviour

{
    private SpriteRenderer sr;
    private Entity_Stats statToModify;

    [Header("Buff Settings")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName = "Damage Buff";
    //[SerializeField] private string buffDescription = "Increases damage by 5 for a short duration.";
    [SerializeField] private float buffDuration = 5.0f;
    [SerializeField] private bool canBeUsed = false;

    [Header("Floating Settings")] 
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(canBeUsed == false)
            return; 

        statToModify = collision.GetComponent<Entity_Stats>();

        // Start Coroutine or logic to extinguish the candle
        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;
        sr.color = Color.clear; // Make the buff invisible or indicate it's used

        foreach (var buff in buffs)
        {
            statToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
        }

        yield return new WaitForSeconds(duration);

        Debug.Log("Buff Ended");
        foreach (var buff in buffs)
        {
            statToModify.GetStatByType(buff.type).RemoveModifier(buffName);
        }
        Destroy(gameObject);
    }

    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
                statToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            else
                statToModify.GetStatByType(buff.type).RemoveModifier(buffName);
           
            
        }
    }

}
