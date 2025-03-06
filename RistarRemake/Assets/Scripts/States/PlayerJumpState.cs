using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("JUMP ENTER");
        _ctx.UpdateAnim("Jump");
        _ctx.Leap = false;
        _ctx.Rb.gravityScale = 1;
        _ctx.Rb.velocity = new Vector2(_ctx.JumpForceH, _ctx.JumpForceV);
    }
    public override void UpdateState() {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        float moveValue = _ctx.MoveH.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2 (_ctx.JumpForceH * moveValue, _ctx.Rb.velocity.y);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }

        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollision(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            _ctx.Animator.SetFloat("WallVH", 0);
            SwitchState(_factory.WallClimb());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            _ctx.Animator.SetFloat("WallVH", 1);
            SwitchState(_factory.WallClimb());
        }
    }
}
