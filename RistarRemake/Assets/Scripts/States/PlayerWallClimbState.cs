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
            _ctx.Rb.velocity = new Vector2(0, moveValueV * _ctx.WalkSpeed * Time.deltaTime);
        }
        else
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            _ctx.Rb.velocity = new Vector2(moveValueH * _ctx.WalkSpeed * Time.deltaTime, 0);
        }     
    }
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates()
    {
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
                    SwitchState(_factory.Jump());
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

    public override void OnCollision(Collision2D collision) { }
}