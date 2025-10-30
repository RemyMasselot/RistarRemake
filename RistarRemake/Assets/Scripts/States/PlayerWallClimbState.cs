using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallClimbState : PlayerBaseState
{
    public PlayerWallClimbState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER WALL CLIMB");
        _player.UpdateAnim("WallClimb");
        _player.Rb.gravityScale = 0;
        if (_player.Animator.GetFloat("WallVH") == 0)
        {
            if (_player.LadderVDetectionL.IsLadderVDectectedL == 1)
            {
                _player.SpriteRenderer.flipX = true;
            }
            if (_player.LadderVDetectionR.IsLadderVDectectedR == 1)
            {
                _player.SpriteRenderer.flipX = false;
            }
        }
        // CAMERA BEHAVIOR
        //_ctx.MainCameraBehavior.CurrentState = "CLIMB";
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        // Déplacements du personnage
        if (_player.Animator.GetFloat("WallVH") == 0)
        {
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (moveValueV > 0)
            {
                _player.Rb.velocity = new Vector2(0, _player.WalkSpeed * Time.deltaTime);
            }
            if (moveValueV < 0)
            {
                _player.Rb.velocity = new Vector2(0, -_player.WalkSpeed * Time.deltaTime);
            }
        }
        else
        {
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (moveValueH > 0)
            {
                _player.SpriteRenderer.flipX = false;
                _player.Rb.velocity = new Vector2(_player.WalkSpeed * Time.deltaTime, 0);
            }
            if (moveValueH < 0)
            {
                _player.SpriteRenderer.flipX = true;
                _player.Rb.velocity = new Vector2(-_player.WalkSpeed * Time.deltaTime, 0);
            }
        }     
    }
    public override void ExitState(){}
    public override void CheckSwitchStates()
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.Damage());
            }
        }

        // Vertical or Horizontal
        if (_player.Animator.GetFloat("WallVH") == 0) // VERTICAL
        {
            // Passage en state WALL IDLE
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (moveValueV == 0)
            {
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.WallIdle());
            }
            if (moveValueV > 0)
            {
                // Passage en state JUMP
                if (_player.Jump.WasPerformedThisFrame())
                {
                    //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                    SwitchState(_factory.WallJump());
                }
            }
            if (moveValueV < 0)
            {
                // Passage en state FALL
                if (_player.Jump.WasPerformedThisFrame())
                {
                    //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                    SwitchState(_factory.Fall());
                }
            }
        }
        else // HORIZONTAL
        {
            // Passage en state WALL IDLE
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) == 0)
            {
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.WallIdle());
            }

            // Passage en state FALL
            if (_player.Jump.WasPerformedThisFrame())
            {
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.Fall());
            }
        }

        // Passage en state FALL
        if (_player.Fall == true)
        {
            //_ctx.MainCameraBehavior.CurrentState = "OTHER";
            SwitchState(_factory.Fall());
        }
        
        // Passage en state LEAP
        if (_player.Leap == true)
        {
            //_ctx.MainCameraBehavior.CurrentState = "OTHER";
            SwitchState(_factory.Leap());
        }

    }
}