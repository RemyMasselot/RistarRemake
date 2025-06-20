using DG.Tweening;
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
        _ctx.UpdateAnim("Headbutt");
        // Move Left Arm
        _ctx.IkArmLeft.transform.DOPause();
        //_ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationExtendGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOPause();
        //_ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationExtendGrab);
    }
    public override void UpdateState()
    {
        // Move Left Arm
        _ctx.IkArmLeft.transform.position = _ctx.ArmDetection.SnapPosHand;
        // Move Right Arm
        _ctx.IkArmRight.transform.position = _ctx.ArmDetection.SnapPosHand;

        if (_ctx.UseSpine == false)
        {
            // Draw Line Arm
            _ctx.LineArmLeft.SetPosition(0, _ctx.ShoulderLeft.position);
            _ctx.LineArmLeft.SetPosition(1, _ctx.IkArmLeft.position);
            _ctx.LineArmRight.SetPosition(0, _ctx.ShoulderRight.position);
            _ctx.LineArmRight.SetPosition(1, _ctx.IkArmRight.position);
        }

        _ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }
    }
}