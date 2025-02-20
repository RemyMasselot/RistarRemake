using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("ENTER FALL");
        _ctx.UpdateAnim("Fall");
        _ctx.Rb.gravityScale = 1.0f;
    }
    public override void UpdateState() { }
    public override void FixedUpdateState() {
        CheckSwitchStates();
        
        // Air Control
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

        // Vérification d'un sol ou non
        if (_ctx.GroundDetection.IsLayerDectected == true)
        {
            float moveValue = _ctx.MoveH.ReadValue<float>();
            if (moveValue != 0)
            {
                // Passage en state WALK
                SwitchState(_factory.Walk());
            }
            else
            {
                // Passage en state IDLE
                SwitchState(_factory.Idle());
            }
        }
    }
    public override void OnCollision(Collision2D collision) { }
}
