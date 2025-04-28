using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private int _starHandleCurrentValue = 0;
    private bool SnapHands = true;

    public override void EnterState()
    {
        Debug.Log("ENTER HANG");
        SnapHands = true;
        _ctx.ArmDetection.gameObject.SetActive(false);
        _starHandleCurrentValue = 0;
        _ctx.Rb.velocity = Vector2.zero;
        _ctx.UpdateAnim("Hang");

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.4f);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.4f);
    }
    public override void UpdateState()
    {
        // Position des mains
        if (SnapHands == true)
        {
            // Move Left Arm
            _ctx.IkArmLeft.transform.position = _ctx.ArmDetection.SnapPosHand;
            // Move Right Arm
            _ctx.IkArmRight.transform.position = _ctx.ArmDetection.SnapPosHand;
        }

        if (_ctx.Grab.WasReleasedThisFrame() && _ctx.ArmDetection.ObjectDetected == 2)
        {
            ShortenArms();
            //SwitchState(_factory.Headbutt());
            //Debug.Log("ENTER HEADBUTT");
        }

        if (_ctx.Grab.WasReleasedThisFrame() && _ctx.ArmDetection.ObjectDetected == 4)
        {
            if (_starHandleCurrentValue < _ctx.StarHandleTargetValue)
            {
                if (_ctx.Rb.velocity.y <= 0)
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
    public void ShortenArms()
    {
        SnapHands = false;
        // Move Left Arm
        _ctx.IkArmLeft.transform.DOPause();
        _ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOPause();
        _ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationGrab).OnComplete(() =>
        {
            SwitchState(_factory.Headbutt());
        });
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