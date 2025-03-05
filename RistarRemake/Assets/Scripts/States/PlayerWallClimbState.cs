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
            // Vérification d'un Ladder ou non
            if (_ctx.LadderVDetection.IsLayerDectected == false)
            {
                SwitchState(_factory.Fall());
            }
            // Vérification d'un Ladder ou non
            if (_ctx.LadderVDetectionR.IsLayerDectected == false)
            {
                SwitchState(_factory.Fall());
            }
        }
        else
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            _ctx.Rb.velocity = new Vector2(moveValueH * _ctx.WalkSpeed * Time.deltaTime, 0);
            // Vérification d'un Ladder ou non
            if (_ctx.LadderHDetection.IsLayerDectected == false)
            {
                SwitchState(_factory.Fall());
            }
        }     
    }
    public override void ExitState(){}
    public override void InitializeSubState(){}
    public override void CheckSwitchStates()
    {
        // Passage en state WALL IDLE
        if (_ctx.Animator.GetFloat("WallVH") == 0)
        {
            float moveValueV = _ctx.MoveV.ReadValue<float>();
            if (Mathf.Abs(moveValueV) == 0)
            {
                SwitchState(_factory.WallIdle());
            }
        }
        else
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) == 0)
            {
                SwitchState(_factory.WallIdle());
            }
        }

        // Passage en state FALL
        if (_ctx.Back.WasPerformedThisFrame())
        {
            SwitchState(_factory.Fall());
        }
    }

    public override void OnCollision(Collision2D collision) { }
}