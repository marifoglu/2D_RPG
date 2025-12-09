using UnityEngine;

public class Object_Buff : MonoBehaviour

{
    private Player_Stats statToModify;

    [Header("Buff Settings")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName = "Damage Buff";
    [SerializeField] private float buffDuration = 5.0f;

    [Header("Floating Settings")] 
    [SerializeField] private float floatSpeed = 1.0f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    private void Awake()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        transform.position = startPosition + new Vector3(0, yOffset);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        statToModify = collision.GetComponent<Player_Stats>();

        if (statToModify.CanApplyBuffOf(buffName))
        {
            statToModify.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }
    }

}
