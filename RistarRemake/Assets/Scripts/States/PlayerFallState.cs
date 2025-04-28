using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        Debug.Log("ENTER FALL");
        _ctx.UpdateAnim("Fall");
        _ctx.Fall = false;
        _ctx.Rb.gravityScale = 1.0f;
    }
    public override void UpdateState() { }
    public override void FixedUpdateState() 
    {
        CheckSwitchStates();
        
        // Air Control
        float moveValue = _ctx.MoveH.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2 (_ctx.JumpForceH * moveValue, _ctx.Rb.velocity.y);

        // Rotation visuelle -- SANS SPINE
        //if (_ctx.Rb.velocity.x > 0)
        //{
        //    _ctx.SpriteRenderer.flipX = false;
        //}
        //if (_ctx.Rb.velocity.x < 0)
        //{
        //    _ctx.SpriteRenderer.flipX = true;
        //}
        if (_ctx.Rb.velocity.x > 0)
        {
            _ctx.SkeletonAnimation.skeleton.FlipX = false;
        }
        if (_ctx.Rb.velocity.x < 0)
        {
            _ctx.SkeletonAnimation.skeleton.FlipX = true;
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
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
