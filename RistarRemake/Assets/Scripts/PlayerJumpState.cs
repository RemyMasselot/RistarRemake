using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        HandleJump();
    }
    public override void UpdateState() {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    void HandleJump() {
        Debug.Log("JUMP ENTER");
        _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.JumpForce);
    }

    public override void OnCollision(Collision2D collision)
    {
        Debug.Log("fef");
        if (collision.gameObject.CompareTag("Ground"))
        {
            SwitchState(_factory.Idle());
        }
    }
}
