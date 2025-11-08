using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; }
    public bool canChangeState;

    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        currentState = startState;
        currentState?.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false || newState == null)
            return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void UpdateActiveState()
    {
        // Check if state machine is active AND currentState is not null
        if (!canChangeState || currentState == null)
            return;

        currentState.Update();
    }

    public void switchOffStateMachine() => canChangeState = false;
}