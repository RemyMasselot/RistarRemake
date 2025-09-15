using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("JUMP ENTER");
        _ctx.UpdateAnim("Jump");
        _ctx.Leap = false;
        _ctx.Rb.gravityScale = 1;
        _ctx.Rb.velocity = new Vector2(_ctx.JumpForceH, _ctx.JumpForceV);
    }
    public override void UpdateState() 
    {
        // Rotation visuelle
        if (_ctx.Rb.velocity.x > 0)
        {
            _ctx.SpriteRenderer.flipX = false;
        }
        if (_ctx.Rb.velocity.x < 0)
        {
            _ctx.SpriteRenderer.flipX = true;
        }
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        float moveValue = _ctx.MoveH.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2 (_ctx.JumpForceH * moveValue, _ctx.Rb.velocity.y);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Enter DAMAGE STATE
        if (_ctx.Invincinbility.IsInvincible == false)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }

        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) 
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
    public override void OnCollisionStay2D(Collision2D collision) { }

}
