using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("ENTER FALL");
        _ctx.UpdateAnim("Fall");
        _ctx.Fall = false;
        if (_ctx.ArmDetection.ObjectDetected == 4)
        {
            Debug.Log("FALL from star handle");
            Vector2 dir = (_ctx.transform.position - _ctx.ShCentre).normalized;

            float percent = (_ctx._starHandleCurrentValue - 0) / (_ctx.StarHandleTargetValue - 0) * 100f;
            _ctx.ShImpulseCurrent = _ctx.ShImpulseMin + (_ctx.ShImpulseMax - _ctx.ShImpulseMin) * (percent / 100f);

            _ctx.Rb.velocity = dir * _ctx.ShImpulseCurrent;
        }
        if (_ctx.LadderVDetectionL.IsLadderVDectectedL == 1)
        {
            _ctx.Rb.velocity = new Vector2(_ctx.JumpForceH / 2, -_ctx.JumpForceV / 2);
        }
        if (_ctx.LadderVDetectionR.IsLadderVDectectedR == 1)
        {
            _ctx.Rb.velocity = new Vector2(-_ctx.JumpForceH / 2, -_ctx.JumpForceV / 2);
        }
        _ctx.ArmDetection.ObjectDetected = 0;
        _ctx.Rb.gravityScale = 2;
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        // Air Control
        float moveValueH = _ctx.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_ctx.MoveV.ReadValue<float>(), _ctx.MoveDownFallValue, _ctx.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _ctx.Rb.velocity = new Vector2(moveValueH * _ctx.JumpForceH, _ctx.Rb.velocity.y + moveValueV);
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.Rb.velocity.y + moveValueV);
        }


        if (_ctx.UseSpine == false)
        {
            // Rotation visuelle -- SANS SPINE
            if (_ctx.Rb.velocity.x > 0)
            {
                _ctx.SpriteRenderer.flipX = false;
            }
            if (_ctx.Rb.velocity.x < 0)
            {
                _ctx.SpriteRenderer.flipX = true;
            }
        }
        else
        {
            // Rotation visuelle -- AVEC SPINE
            if (_ctx.Rb.velocity.x > 0)
            {
                _ctx.SkeletonAnimation.skeleton.FlipX = false;
            }
            if (_ctx.Rb.velocity.x < 0)
            {
                _ctx.SkeletonAnimation.skeleton.FlipX = true;
            }
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
       // Passage en state JUMP
       // Jump buffering : on m�morise l�entr�e
       if (_ctx.Jump.WasPerformedThisFrame())
       {
           _ctx.JumpBufferCounter = _ctx.JumpBufferTime;
       }
       else
       {
           _ctx.JumpBufferCounter -= Time.deltaTime;
       }
       
       // Ex�cution du saut
       if (_ctx.JumpBufferCounter > 0 && _ctx.CoyoteCounter > 0)
       {
           _ctx.JumpBufferCounter = 0f;
           SwitchState(_factory.Jump());
       }

        // V�rification d'un sol ou non
        if (_ctx.GroundDetection.IsGroundDectected == true)
        {
            // Mise � jour du coyote time
            _ctx.CoyoteCounter = _ctx.CoyoteTime;

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
        else
        {
            _ctx.CoyoteCounter -= Time.deltaTime;
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
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
            if (_ctx.LadderVDetectionL.IsLadderVDectectedL == LayerMask.NameToLayer("LadderV") || _ctx.LadderVDetectionR.IsLadderVDectectedR == LayerMask.NameToLayer("LadderV"))
            {
                _ctx.Animator.SetFloat("WallVH", 0);
                SwitchState(_factory.WallClimb());
            }
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            if (_ctx.LadderHDetection.IsLadderHDectected == true)
            {
                _ctx.Animator.SetFloat("WallVH", 1);
                SwitchState(_factory.WallClimb());
            }
        }
    }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
