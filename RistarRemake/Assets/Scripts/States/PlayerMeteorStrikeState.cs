using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeteorStrikeState : PlayerBaseState
{
    public PlayerMeteorStrikeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        Debug.Log("ENTER METEOR STRIKE");
        //_ctx.UpdateAnim("Grab");
    }
    public override void UpdateState()
    {
        
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollision(Collision2D collision) { }
}