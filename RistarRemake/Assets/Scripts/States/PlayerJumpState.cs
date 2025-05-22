using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");
        _ctx.UpdateAnim("Jump");
        _ctx.Leap = false;
        _ctx.Rb.gravityScale = 1;
        _ctx.Rb.velocity = new Vector2(_ctx.JumpForceH, _ctx.JumpForceV);
        StartTimer();
    }
    void StartTimer()
    {
        _ctx.CurrentTimerValueJump = _ctx.MaxTimeJump;
        _ctx.IsTimerRunningJump = true;
    }
    public override void UpdateState() 
    {
        if (_ctx.IsTimerRunningJump == true)
        {
            _ctx.CurrentTimerValueJump -= Time.deltaTime;
            if (_ctx.CurrentTimerValueJump <= 0f)
            {
                _ctx.IsTimerRunningJump = false;
            }
        }

        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        //Debug.Log(_ctx.Rb.velocity.y);
        // Si on est a l'apex et que la touche est maintenu, on reste à l'apex
        if (_ctx.Rb.velocity.y < 0)
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, 0);
        }

        float moveValue = _ctx.MoveH.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2 (_ctx.JumpForceH * moveValue, _ctx.Rb.velocity.y);

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
        // Passage en state FALL
        if (_ctx.IsTimerRunningJump == false)
        {
            SwitchState(_factory.Fall());
        }
        if (_ctx.Jump.WasReleasedThisFrame())
        {
            _ctx.IsTimerRunningJump = false;
            SwitchState(_factory.Fall());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollision(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            _ctx.Animator.SetFloat("WallVH", 0);
            SwitchState(_factory.WallClimb());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            _ctx.Animator.SetFloat("WallVH", 1);
            SwitchState(_factory.WallClimb());
        }
    }
}
