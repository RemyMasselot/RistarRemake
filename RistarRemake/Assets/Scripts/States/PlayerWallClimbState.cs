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
        _ctx.UpdateAnim("WallClimb");
        _ctx.Rb.gravityScale = 0;
        if (_ctx.Animator.GetFloat("WallVH") == 0)
        {
            if (_ctx.LadderVDetectionL.IsLadderVDectectedL == 1)
            {
                _ctx.SpriteRenderer.flipX = true;
            }
            if (_ctx.LadderVDetectionR.IsLadderVDectectedR == 1)
            {
                _ctx.SpriteRenderer.flipX = false;
            }
        }
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        // Déplacements du personnage
        if (_ctx.Animator.GetFloat("WallVH") == 0)
        {
            float moveValueV = _ctx.MoveV.ReadValue<float>();
            if (moveValueV > 0)
            {
                _ctx.Rb.velocity = new Vector2(0, _ctx.WalkSpeed * Time.deltaTime);
            }
            if (moveValueV < 0)
            {
                _ctx.Rb.velocity = new Vector2(0, -_ctx.WalkSpeed * Time.deltaTime);
            }
        }
        else
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            if (moveValueH > 0)
            {
                _ctx.Rb.velocity = new Vector2(_ctx.WalkSpeed * Time.deltaTime, 0);
            }
            if (moveValueH < 0)
            {
                _ctx.Rb.velocity = new Vector2(-_ctx.WalkSpeed * Time.deltaTime, 0);
            }
        }     
    }
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates()
    {
        // Enter DAMAGE STATE
        if (_ctx.Invincinbility.IsInvincible == false)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Vertical or Horizontal
        if (_ctx.Animator.GetFloat("WallVH") == 0) // VERTICAL
        {
            // Passage en state WALL IDLE
            float moveValueV = _ctx.MoveV.ReadValue<float>();
            if (moveValueV == 0)
            {
                SwitchState(_factory.WallIdle());
            }
            if (moveValueV > 0)
            {
                // Passage en state JUMP
                if (_ctx.Jump.WasPerformedThisFrame())
                {
                    SwitchState(_factory.WallJump());
                }
            }
            if (moveValueV < 0)
            {
                // Passage en state FALL
                if (_ctx.Jump.WasPerformedThisFrame())
                {
                    SwitchState(_factory.Fall());
                }
            }
        }
        else // HORIZONTAL
        {
            // Passage en state WALL IDLE
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) == 0)
            {
                SwitchState(_factory.WallIdle());
            }

            // Passage en state FALL
            if (_ctx.Jump.WasPerformedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }

        // Passage en state FALL
        if (_ctx.Fall == true)
        {
            SwitchState(_factory.Fall());
        }
        
        // Passage en state LEAP
        if (_ctx.Leap == true)
        {
            SwitchState(_factory.Leap());
        }

    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}