using UnityEngine;

/// <summary>
/// Optional debug helper - attach to Player to see ladder state in real-time
/// Remove this script once everything works properly
/// </summary>
public class LadderDebugHelper : MonoBehaviour
{
    private Player player;

    [Header("Debug Info (Read Only)")]
    [SerializeField] private bool isOnLadder;
    [SerializeField] private bool canGrabLadder;
    [SerializeField] private string currentState;
    [SerializeField] private float currentGravity;
    [SerializeField] private Vector2 currentVelocity;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (player == null) return;

        isOnLadder = player.IsOnLadder();
        canGrabLadder = player.CanGrabLadder();

        // Get current state name by checking which state is active
        currentState = GetCurrentStateName();

        currentGravity = player.rb.gravityScale;
        currentVelocity = player.rb.linearVelocity;
    }

    private string GetCurrentStateName()
    {
        // Check each state to determine which one is active
        if (player.idleState != null && IsAnimatorInState("Idle"))
            return "Idle";
        if (player.moveState != null && IsAnimatorInState("Move"))
            return "Move";
        if (player.jumpState != null && IsAnimatorInState("JumpFall"))
            return "Jump";
        if (player.fallState != null && IsAnimatorInState("JumpFall"))
            return "Fall";
        if (player.ladderClimbState != null && IsAnimatorInState("LadderClimb"))
            return "LadderClimb";
        if (player.dashState != null && IsAnimatorInState("Dash"))
            return "Dash";
        if (player.wallSlideState != null && IsAnimatorInState("WallSlide"))
            return "WallSlide";
        if (player.basicAttackState != null && IsAnimatorInState("BasicAttack"))
            return "BasicAttack";
        if (player.counterAttackState != null && IsAnimatorInState("CounterAttack"))
            return "CounterAttack";
        if (player.deadState != null && IsAnimatorInState("Dead"))
            return "Dead";

        return "Unknown";
    }

    private bool IsAnimatorInState(string stateName)
    {
        if (player.anim == null) return false;

        // Check base layer
        AnimatorStateInfo stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(stateName))
            return true;

        // Check other layers
        for (int i = 1; i < player.anim.layerCount; i++)
        {
            stateInfo = player.anim.GetCurrentAnimatorStateInfo(i);
            if (stateInfo.IsName(stateName))
                return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if (player == null) return;

        // Create a semi-transparent background
        GUI.Box(new Rect(10, 10, 300, 200), "");

        GUILayout.BeginArea(new Rect(15, 15, 290, 190));

        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 14;
        headerStyle.fontStyle = FontStyle.Bold;
        GUILayout.Label("Ladder Debug Info", headerStyle);

        GUILayout.Space(5);

        GUILayout.Label($"On Ladder: {(isOnLadder ? "YES" : "NO")}");
        GUILayout.Label($"Can Grab: {(canGrabLadder ? "YES" : "NO")}");
        GUILayout.Label($"State: {currentState}");
        GUILayout.Label($"Gravity: {currentGravity:F2}");
        GUILayout.Label($"Velocity: ({currentVelocity.x:F2}, {currentVelocity.y:F2})");
        GUILayout.Label($"Y Input: {player.moveInput.y:F2}");

        if (player.GetCurrentLadder() != null)
        {
            GUILayout.Label($"Ladder: {player.GetCurrentLadder().name}");
        }
        else
        {
            GUILayout.Label("Ladder: None");
        }

        GUILayout.EndArea();
    }
}