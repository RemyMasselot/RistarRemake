using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base (currentContext, playerStateFactory){}
    
    public override void EnterState() {
        Debug.Log("ENTER GROUND");
    }
    public override void UpdateState() { 
        CheckSwitchStates();
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Passage en state JUMP
        if (_ctx.Jump.WasPerformedThisFrame())
        {
            SwitchState(_factory.Jump());
        }

        // Passage en state WALK
        float moveValue = _ctx.MoveH.ReadValue<float>();
        if (Mathf.Abs(moveValue) > 0)
        {
            SwitchState(_factory.Walk());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
