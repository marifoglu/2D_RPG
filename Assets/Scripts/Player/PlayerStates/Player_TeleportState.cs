using UnityEngine;
using System;

public class Player_TeleportState : PlayerState
{
    public enum TeleportPhase
    {
        Starting,       // Player_TeleportStart
        Teleporting,    // Moving player
        Arriving,       // Player_TeleportEnd
        Complete
    }

    private TeleportPhase currentPhase;
    private Vector3 destinationPosition;
    private Object_TeleportShrine destinationShrine;
    private Action onTeleportComplete;

    private float teleportStartDuration = 0.6f;
    private float teleportEndDuration = 0.6f;
    private float originalGravity;

    public Player_TeleportState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName)
    {
    }

    public void SetupTeleport(Vector3 destination, Object_TeleportShrine shrine, Action onComplete = null)
    {
        destinationPosition = destination;
        destinationShrine = shrine;
        onTeleportComplete = onComplete;
    }

    public override void Enter()
    {
        base.Enter();

        currentPhase = TeleportPhase.Starting;
        triggerCalled = false;

        player.SetVelocity(0, 0);

        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        if (player.health != null)
            player.health.SetCanTakeDamage(false);

        SpawnStartVFX();

        // EXACT PARAMETERS: Teleport (bool), TeleportStart (trigger)
        anim.SetBool("Teleport", true);
        anim.SetTrigger("TeleportStart");

        stateTimer = teleportStartDuration;
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, 0);

        switch (currentPhase)
        {
            case TeleportPhase.Starting:
                if (triggerCalled || stateTimer <= 0)
                {
                    currentPhase = TeleportPhase.Teleporting;
                    triggerCalled = false;
                }
                break;

            case TeleportPhase.Teleporting:
                player.transform.position = destinationPosition;

                if (destinationShrine != null)
                    destinationShrine.OnPlayerArrived();

                SpawnArrivalVFX();

                anim.SetTrigger("TeleportEnd");
                stateTimer = teleportEndDuration;
                currentPhase = TeleportPhase.Arriving;

                break;

            case TeleportPhase.Arriving:
                if (triggerCalled || stateTimer <= 0)
                {
                    currentPhase = TeleportPhase.Complete;
                    triggerCalled = false;
                }
                break;

            case TeleportPhase.Complete:
                stateMachine.ChangeState(player.idleState);
                break;
        }
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("Teleport", false);
        anim.ResetTrigger("TeleportStart");
        anim.ResetTrigger("TeleportEnd");

        rb.gravityScale = originalGravity;

        if (player.health != null)
            player.health.SetCanTakeDamage(true);

        onTeleportComplete?.Invoke();

        destinationShrine = null;
        onTeleportComplete = null;
    }

    private void SpawnStartVFX()
    {
        GameObject vfx = player.GetTeleportStartVFX();
        if (vfx == null) return;

        Vector2 offset = player.GetTeleportStartVFXOffset();
        Vector3 pos = player.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject obj = UnityEngine.Object.Instantiate(vfx, pos, Quaternion.identity);
        UnityEngine.Object.Destroy(obj, 3f);
    }

    private void SpawnArrivalVFX()
    {
        GameObject vfx = player.GetTeleportArrivalVFX();
        if (vfx == null) return;

        Vector2 offset = player.GetTeleportArrivalVFXOffset();
        Vector3 pos = player.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject obj = UnityEngine.Object.Instantiate(vfx, pos, Quaternion.identity);
        UnityEngine.Object.Destroy(obj, 3f);
    }
}