using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        _player.UpdateAnim("Fall");
        _player.Fall = false;
        _player.JumpReady = false;

        if (_player.ArmDetection.ObjectDetected == 4)
        {
            Debug.Log("FALL from star handle");
            Vector2 dir = (_player.transform.position - _player.ShCentre).normalized;

            float percent = (_player._starHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.ShImpulseCurrent = _player.ShImpulseMin + (_player.ShImpulseMax - _player.ShImpulseMin) * (percent / 100f);

            _player.Rb.velocity = dir * _player.ShImpulseCurrent;
        }

        PushAwayFromLadder();

        _player.ArmDetection.ObjectDetected = 0;
        _player.Rb.gravityScale = 2;

        // CAMERA BEHAVIOR
        //_ctx.MainCameraBehavior.CurrentState = "FALL";
        //_ctx.MainCameraBehavior.CorrectPosY();
        //_ctx.Camera.DOOrthoSize(_ctx.MainCameraBehavior.SizeDefault, 0.8f);
        //DOTween.To(() => _ctx.MainCameraBehavior.CameraPositionFallOff.y, x => _ctx.MainCameraBehavior.CameraPositionFallOff.y = x, _ctx.MainCameraBehavior.PosFallY, 0.8f);
    }
    public override void ExitState()
    {
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        // CAMERA BEHAVIOR
        //_ctx.MainCameraBehavior.CorrectPosY();
        //DOTween.To(() => _ctx.MainCameraBehavior.CameraPositionFallOff.y, x => _ctx.MainCameraBehavior.CameraPositionFallOff.y = x, _ctx.MainCameraBehavior.newYDown, 2f);

        // Air Control
        float moveValueH = _player.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.MoveDownFallValue, _player.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _player.Rb.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.Rb.velocity.y + moveValueV);
        }
        else
        {
            _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, _player.Rb.velocity.y + moveValueV);
        }

        if (_player.UseSpine == false)
        {
            // Rotation visuelle -- SANS SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SpriteRenderer.flipX = false;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SpriteRenderer.flipX = true;
            }
        }
        else
        {
            // Rotation visuelle -- AVEC SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = 1;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = -1;
            }
        }
    }
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

        // Exécution du saut
        if (_player.Jump.WasPerformedThisFrame())
        {
            // Coyote Time 
            if (_player.CoyoteCounter > 0)
            {
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.Jump());
            }

            // Jump Buffering
            if (_player.JumpBufferingDetection.IsGroundDectected == true)
            {
                if (_player.Rb.velocity.y <= 0)
                {
                    _player.JumpReady = true;
                }
            }
        }

        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == true)
        {
            float moveValue = _player.MoveH.ReadValue<float>();
            if (moveValue != 0)
            {
                // Passage en state WALK
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.Walk());
            }
            else
            {
                // Passage en state IDLE
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.Idle());
            }
        }
        else
        {
            _player.CoyoteCounter -= Time.deltaTime;
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            //_ctx.MainCameraBehavior.CurrentState = "OTHER";
            SwitchState(_factory.Grab());
        }

        //// Passage en state CLIMB
        //if (_ctx.LadderVDetectionL.IsLadderVDectectedL == true || _ctx.LadderVDetectionR.IsLadderVDectectedR == true)
        //{
        //    _ctx.Animator.SetFloat("WallVH", 0);
        //    SwitchState(_factory.WallClimb());
        //}
        //if (_ctx.LadderHDetection.IsLadderHDectected == true)
        //{
        //    _ctx.Animator.SetFloat("WallVH", 1);
        //    SwitchState(_factory.WallClimb());
        //}
    }
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            if (_player.LadderVDetectionL.IsLadderVDectectedL == LayerMask.NameToLayer("LadderV") || _player.LadderVDetectionR.IsLadderVDectectedR == LayerMask.NameToLayer("LadderV"))
            {
                _player.IsCurrentLadderHorizontal = false;
                _player.Animator.SetFloat("WallVH", 0);
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.WallClimb());
            }
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                _player.IsCurrentLadderHorizontal = true;
                _player.Animator.SetFloat("WallVH", 1);
                //_ctx.MainCameraBehavior.CurrentState = "OTHER";
                SwitchState(_factory.WallClimb());
            }
        }
    }

    private void PushAwayFromLadder()
    {
        if (_player.LadderVDetectionL.IsLadderVDectectedL == 1)
        {
            _player.Rb.velocity = new Vector2(_player.JumpForceH / 2, -_player.JumpForceV / 2);
        }
        if (_player.LadderVDetectionR.IsLadderVDectectedR == 1)
        {
            _player.Rb.velocity = new Vector2(-_player.JumpForceH / 2, -_player.JumpForceV / 2);
        }
    }   
}
