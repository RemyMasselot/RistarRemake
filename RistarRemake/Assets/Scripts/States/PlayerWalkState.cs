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

        // Vérification d'un sol ou non
        if (_ctx.GroundDetection.IsLayerDectected == false)
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

    public override void OnCollision(Collision2D collision) { }
}
