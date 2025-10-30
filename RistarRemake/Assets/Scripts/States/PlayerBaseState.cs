using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    protected PlayerStateMachine _player;
    public PlayerStateFactory _factory;
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    { 
        _player = currentContext;
        _factory = playerStateFactory;
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        _player.PreviousState = _player.CurrentState;

        // current state exits state
        ExitState();

        // new state enters state
        newState.EnterState();

        // switch current state of context
        _player.CurrentState = newState;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public virtual void InitializeSubState() { }
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay2D(Collision2D collision) { }
}
