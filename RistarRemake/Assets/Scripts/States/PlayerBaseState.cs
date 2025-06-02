using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    protected bool _isRootState = false;
    protected PlayerStateMachine _ctx;
    protected PlayerStateFactory _factory;
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    { 
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();
    protected void SwitchState(PlayerBaseState newState)
    {
        _ctx.PreviousState = _ctx.CurrentState;

        // current state exits state
        ExitState();

        // new state enters state
        newState.EnterState();

        // switch current state of context
        _ctx.CurrentState = newState;
    }
    public abstract void OnCollisionEnter2D(Collision2D collision);
    public abstract void OnCollisionStay2D(Collision2D collision);
    protected void SetSuperState() { }
    protected void SetSubState() { }
}
