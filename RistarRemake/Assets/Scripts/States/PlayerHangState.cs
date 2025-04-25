using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private int _starHandleCurrentValue = 0;
    public override void EnterState()
    {
        Debug.Log("ENTER HANG");
        _starHandleCurrentValue = 0;
        //_ctx.UpdateAnim("Grab");
    }
    public override void UpdateState()
    {
        if (_ctx.Grab.WasReleasedThisFrame() && _ctx.ArmDetection.ObjectDetected == 2)
        {
            SwitchState(_factory.Headbutt());
            //Debug.Log("ENTER HEADBUTT");
        }
        if (_ctx.Grab.WasReleasedThisFrame() && _ctx.ArmDetection.ObjectDetected == 4)
        {
            if (_starHandleCurrentValue < _ctx.StarHandleTargetValue)
            {
                if (_ctx.Rb.velocity.y < 0)
                {
                    SwitchState(_factory.Fall());
                }
                else
                {
                    SwitchState(_factory.Jump());
                }
            }
        }
    }
    public override void FixedUpdateState() 
    {
        ChargingMeteorStrike();
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollision(Collision2D collision) { }

    private void ChargingMeteorStrike()
    {
        if (_ctx.ArmDetection.ObjectDetected == 4)
        {
            if (_ctx.SpriteRenderer.flipX == true)
            {
                if (_ctx.MoveH.ReadValue<float>() < 0)
                {
                    _starHandleCurrentValue++;
                    //Debug.Log(_chargingMeteorStrike);
                }
                if (_ctx.MoveH.ReadValue<float>() > 0)
                {
                    _starHandleCurrentValue--;
                    //Debug.Log(_chargingMeteorStrike);
                }
            }
            else
            {
                if (_ctx.MoveH.ReadValue<float>() > 0)
                {
                    _starHandleCurrentValue++;
                    //Debug.Log(_chargingMeteorStrike);
                }
                if (_ctx.MoveH.ReadValue<float>() < 0)
                {
                    _starHandleCurrentValue--;
                    //Debug.Log(_chargingMeteorStrike);
                }
            }

            if (_starHandleCurrentValue <= 0)
            {
                _starHandleCurrentValue = 0;
            }

            if (_ctx.Grab.WasReleasedThisFrame() && _starHandleCurrentValue >= _ctx.StarHandleTargetValue)
            {
                SwitchState(_factory.MeteorStrike());
            }
        }
    }
}