using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("ENTER FALL");
        _ctx.Animator.SetBool("Fall", true);
        _ctx.Rb.gravityScale = 1.0f;
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
            //_ctx.Animator.SetBool("Idle", false);
            _ctx.Animator.SetBool("Fall", false);
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            // Passage en state IDLE
            _ctx.Animator.SetBool("Fall", false);
            SwitchState(_factory.Idle());

            // Passage en state WALK
            float moveValue = _ctx.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValue) > 0)
            {
                _ctx.Animator.SetBool("Fall", false);
                SwitchState(_factory.Walk());
            }
        }
    }
}
