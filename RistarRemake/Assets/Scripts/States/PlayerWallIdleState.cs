using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallIdleState : PlayerBaseState
{
    public PlayerWallIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER WALL IDLE");
        _player.UpdateAnim("WallIdle");
        _player.Rb.velocity = Vector2.zero;
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
    }
    public override void UpdateState()
    {
        CheckSwitchStates();

        //CAMERA BEHAVIOR
        //Vector2 aimValue = _ctx.Aim.ReadValue<Vector2>();
        //if (_ctx.Animator.GetFloat("WallVH") == 1) //Echelle Horizontale
        //{
        //    // Rotation visuelle
        //    if (aimValue.x > 0)
        //    {
        //        _ctx.SpriteRenderer.flipX = false;
        //        DOTween.To(() => _ctx.MainCameraBehavior.CameraPositionFallOff.x, x => _ctx.MainCameraBehavior.CameraPositionFallOff.x = x, _ctx.MainCameraBehavior.PosWalkX, 2f);
        //    }
        //    if (aimValue.x < 0)
        //    {
        //        _ctx.SpriteRenderer.flipX = true;
        //        DOTween.To(() => _ctx.MainCameraBehavior.CameraPositionFallOff.x, x => _ctx.MainCameraBehavior.CameraPositionFallOff.x = x, -_ctx.MainCameraBehavior.PosWalkX, 2f);
        //    }
        //}
    }
    public override void FixedUpdateState(){ }
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

        // Passage en state WALL CLIMB
        if (_player.Animator.GetFloat("WallVH") == 0) //Echelle Vertical
        {
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (Mathf.Abs(moveValueV) != 0)
            {
                SwitchState(_factory.WallClimb());
            }
            if (_player.Jump.WasPerformedThisFrame())
            {
                if (Mathf.Abs(moveValueV) > 0) // Passage en state WALL JUMP
                {
                    SwitchState(_factory.WallJump());
                }
                else // Passage en state FALL
                {
                    _player.SpriteRenderer.flipX = !_player.SpriteRenderer.flipX;
                    SwitchState(_factory.Fall());
                }
            }
        }
        else //Echelle Horizontal
        {
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) != 0)
            {
                SwitchState(_factory.WallClimb());
            }
            if (_player.Jump.WasPerformedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}