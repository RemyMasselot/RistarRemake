using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState(){ }
    public override void UpdateState(){
        
    }
    public override void FixedUpdateState(){}
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates(){
        // Passage en state WALK
        float moveValue = _ctx.Walk.ReadValue<float>();
        if (Mathf.Abs(moveValue) > 0)
        {
            SwitchState(_factory.Walk());
        }
    }
}
