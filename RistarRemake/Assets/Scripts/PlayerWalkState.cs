using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void EnterState() {
        //Debug.Log("ENTER WALK");
        _ctx.Animator.SetBool("Walk", true);
    }
    public override void UpdateState() {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        // Déplacements du personnage
        float moveValue = _ctx.Walk.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2(moveValue * _ctx.WalkSpeed * Time.deltaTime, 0);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Passage en state IDLE
        float moveValue = _ctx.Walk.ReadValue<float>();
        if (moveValue == 0)
        {
            _ctx.Animator.SetBool("Walk", false);
            SwitchState(_factory.Idle());
        }

        // Passage en state JUMP
        if (_ctx.Jump.WasPerformedThisFrame())
        {
        _ctx.Animator.SetBool("Walk", false);
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            //_ctx.Animator.SetBool("Idle", false);
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollision(Collision2D collision) { }

}
