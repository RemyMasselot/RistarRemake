using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadbuttState : PlayerBaseState
{
    public PlayerHeadbuttState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        Debug.Log("ENTER HEADBUTT");
        //_ctx.UpdateAnim("Grab");
    }
    public override void UpdateState()
    {
        _ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        _ctx.ArmDetection.ObjectDetected = 0;
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollision(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }
    }
}