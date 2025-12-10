using UnityEngine;

public class Object_NPC : MonoBehaviour
{
    protected Transform player;
    protected UI ui;

    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactToolTip;
    private bool facingRight = true;

    [Header("Floating Settings")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = 0.1f;
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactToolTip.transform.position;
        interactToolTip.SetActive(false);
    }

    protected virtual void Update()
    {
        HandleNpcFlip();
        HandleToolTipFloat();   
    }
    private void HandleToolTipFloat()
    {
        if (interactToolTip.activeSelf)
        {
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactToolTip.transform.position = startPosition + new Vector3(0, yOffset);
        } 
            return;
    }

    private void HandleNpcFlip()
    {
        if(player == null || npc == null) 
            return;

        if (player.position.x > npc.position.x && facingRight)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            facingRight = false;
        }
        else if (player.position.x < npc.position.x && !facingRight)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            facingRight = true;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       player = collision.transform;
        interactToolTip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        player = null;
        interactToolTip.SetActive(false);
    }
}
