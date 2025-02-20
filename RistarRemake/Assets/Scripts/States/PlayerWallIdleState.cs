using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallIdleState : PlayerBaseState
{
    public PlayerWallIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState(){
        //Debug.Log("ENTER WALL IDLE");
        _ctx.UpdateAnim("WallIdle");
        _ctx.Rb.velocity = Vector2.zero;
        _ctx.Rb.gravityScale = 0;
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){ }
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates(){
        // Passage en state WALL CLIMB
        if (_ctx.Animator.GetFloat("WallVH") == 0)
        {
            float moveValueV = _ctx.MoveV.ReadValue<float>();
            if (Mathf.Abs(moveValueV) > 0)
            {
                SwitchState(_factory.WallClimb());
            }
        }
        else
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) > 0)
            {
                SwitchState(_factory.WallClimb());
            }
        }

        // Passage en state FALL
        if (_ctx.Back.WasPerformedThisFrame())
        {
            SwitchState(_factory.Fall());
        }
    }

    public override void OnCollision(Collision2D collision) { }
}