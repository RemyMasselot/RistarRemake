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
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, 0);
        }
        _ctx.ArmDetection.ObjectDetected = 0;
        _ctx.Rb.gravityScale = 2;
        _ctx.PreviousState = _ctx.CurrentState;
    }
    public override void UpdateState() { }
    public override void FixedUpdateState() 
    {
        // Air Control
        float moveValueH = _ctx.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_ctx.MoveV.ReadValue<float>(), _ctx.MoveDownFallValue, _ctx.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _ctx.Rb.velocity = new Vector2(moveValueH * _ctx.JumpForceH, _ctx.Rb.velocity.y + moveValueV);
        }
        if (moveValueH < 0)
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
        
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Vérification d'un sol ou non
        if (_ctx.GroundDetection.IsGroundDectected == true)
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
            if (_ctx.LadderVDetectionL.IsLadderVDectectedL == true || _ctx.LadderVDetectionR.IsLadderVDectectedR == true)
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
