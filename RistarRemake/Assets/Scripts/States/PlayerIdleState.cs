using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER IDLE");
        _ctx.UpdateAnim("Idle");
        _ctx.Rb.velocity = Vector2.zero;
        _ctx.PreviousState = _ctx.CurrentState;
        // Mise à jour du coyote time
        _ctx.CoyoteCounter = _ctx.CoyoteTime;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        
    }
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates()
    {
        // Enter DAMAGE STATE
        if (_ctx.Invincinbility.IsInvincible == false)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state WALK
        if (_ctx.MoveH.WasPerformedThisFrame())
        {
            SwitchState(_factory.Walk());
        }

        // Passage en state JUMP
        if (_ctx.Jump.WasPerformedThisFrame() || _ctx.JumpReady == true)
        {
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            _ctx.Grab.performed += ctx =>
            {
                var device = ctx.control.device;

                if (device is Mouse)
                {
                    _ctx.GamepadUsed = false;
                }
                else
                {
                    _ctx.GamepadUsed = true;
                }
            };
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
