using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER IDLE");
        _player.UpdateAnim("Idle");
        _player.Rb.velocity = Vector2.zero;
        _player.PreviousState = _player.CurrentState;
        // Mise à jour du coyote time
        _player.CoyoteCounter = _player.CoyoteTime;
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
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state WALK
        if (_player.MoveH.WasPerformedThisFrame())
        {
            SwitchState(_factory.Walk());
        }

        // Passage en state JUMP
        if (_player.Jump.WasPerformedThisFrame() || _player.JumpReady == true)
        {
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            _player.Grab.performed += ctx =>
            {
                var device = ctx.control.device;

                if (device is Mouse)
                {
                    _player.GamepadUsed = false;
                }
                else
                {
                    _player.GamepadUsed = true;
                }
            };
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
