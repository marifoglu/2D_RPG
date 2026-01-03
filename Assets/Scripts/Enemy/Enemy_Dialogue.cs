using UnityEngine;

public class Enemy_Dialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private bool canUseDialogue = false;
    [SerializeField] private DialogueLineSO dialogueLine;
    [SerializeField] private Collider2D triggerZone;
    [SerializeField] private float delayBeforeAttack = 0.5f;

    [Header("Dialogue Pose")]
    [SerializeField] private string dialogueAnimName = "skeletonAttack";
    [SerializeField] private bool isTrigger = true;

    [Header("Display")]
    [SerializeField] private string enemyDisplayName;

    private Enemy enemy;
    private UI ui;
    private Transform player;
    private bool isInDialogue;
    private bool dialogueCompleted;

    public bool IsInDialogue => isInDialogue;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        ui = FindFirstObjectByType<UI>();

        if (string.IsNullOrEmpty(enemyDisplayName))
            enemyDisplayName = gameObject.name.Replace("(Clone)", "").Trim();
    }

    private void Update()
    {
        if (!canUseDialogue || dialogueCompleted || triggerZone == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            triggerZone.bounds.center,
            triggerZone.bounds.size,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit != null && hit.CompareTag("Player"))
        {
            player = hit.transform;
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogueLine == null || ui == null) return;

        isInDialogue = true;
        dialogueCompleted = true;

        // Stop enemy
        enemy.stateMachine.switchOffStateMachine();
        enemy.rb.linearVelocity = Vector2.zero;

        // Play dialogue animation
        if (!string.IsNullOrEmpty(dialogueAnimName))
        {
            if (isTrigger)
                enemy.anim.SetTrigger(dialogueAnimName);
            else
                enemy.anim.SetBool(dialogueAnimName, true);
        }

        DialogueNPCData data = new DialogueNPCData(
            gameObject.name,
            enemyDisplayName,
            RewardType.None,
            null,
            false
        );

        ui.OpenDialogueUI(dialogueLine, data);
    }

    public void EndDialogue()
    {
        isInDialogue = false;

        // Turn off bool animation if used
        if (!string.IsNullOrEmpty(dialogueAnimName) && !isTrigger)
            enemy.anim.SetBool(dialogueAnimName, false);

        // Resume enemy
        enemy.stateMachine.canChangeState = true;
        enemy.stateMachine.ChangeState(enemy.idleState);

        StartCoroutine(AttackAfterDelay());
    }

    private System.Collections.IEnumerator AttackAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeAttack);

        if (player != null)
            enemy.TryEnterBattleState(player);
    }
}