using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("JUMP ENTER");
        _ctx.Animator.SetBool("Jump", true);
        HandleJump();
    }
    public override void UpdateState() {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        float moveValue = _ctx.Walk.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2 (_ctx.JumpForceH * moveValue, _ctx.Rb.velocity.y);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            //_ctx.Animator.SetBool("Idle", false);
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            _ctx.Animator.SetBool("Jump", false);
            SwitchState(_factory.Idle());
        }
    }

    void HandleJump() {
        _ctx.Rb.velocity = new Vector2(_ctx.JumpForceH, _ctx.JumpForceV);
    }
}
