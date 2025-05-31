using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void EnterState() {
        //Debug.Log("ENTER WALK");
        _ctx.UpdateAnim("Walk");
    }
    public override void UpdateState() { 
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        // Déplacements du personnage
        float moveValue = _ctx.MoveH.ReadValue<float>();
        _ctx.Rb.velocity = new Vector2(moveValue * _ctx.WalkSpeed * Time.deltaTime, 0);

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
            //Rotation visuelle -- AVEC SPINE
            if (_ctx.Rb.velocity.x > 0)
            {
                _ctx.SkeletonAnimation.skeleton.FlipX = false;
            }
            if (_ctx.Rb.velocity.x < 0)
            {
                _ctx.SkeletonAnimation.skeleton.FlipX = true;
            }
        }

        // Vérification d'un sol ou non
        if (_ctx.GroundDetection.IsGroundDectected == false)
        {
            SwitchState(_factory.Fall());
        }
        //Debug.Log(_ctx.LayerDetection.IsLayerDectected);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Passage en state IDLE
        float moveValue = _ctx.MoveH.ReadValue<float>();
        if (moveValue == 0)
        {
            SwitchState(_factory.Idle());
        }

        // Passage en state JUMP
        if (_ctx.Jump.WasPerformedThisFrame())
        {
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
