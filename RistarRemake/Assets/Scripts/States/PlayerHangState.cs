using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private int _starHandleCurrentValue = 0;
    private bool SnapHands = false;

    public override void EnterState()
    {
        Debug.Log("ENTER HANG");
        _ctx.ArmDetection.gameObject.SetActive(false);
        SnapHands = false;
        _starHandleCurrentValue = 0;
        _ctx.Rb.velocity = Vector2.zero;
        _ctx.UpdateAnim("Hang");

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.4f);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.4f).OnComplete(()=>
        {
            SnapHands = true;
        });
    }
    public override void UpdateState()
    {
        // Position des mains
        if (SnapHands == true)
        {
            // Move Left Arm
            _ctx.IkArmLeft.transform.position = _ctx.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionL = (Vector2)(_ctx.IkArmLeft.position - _ctx.ShoulderLeft.position);
            float angleL = Mathf.Atan2(directionL.y, directionL.x) * Mathf.Rad2Deg;
            _ctx.IkArmLeft.rotation = Quaternion.Euler(0, 0, angleL);
            
            // Move Right Arm
            _ctx.IkArmRight.transform.position = _ctx.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionR = (Vector2)(_ctx.IkArmRight.position - _ctx.ShoulderRight.position);
            float angleR = Mathf.Atan2(directionR.y, directionR.x) * Mathf.Rad2Deg;
            _ctx.IkArmRight.rotation = Quaternion.Euler(0, 0, angleR);

        }

        if (_ctx.UseSpine == false)
        {
            // Draw Line Arm
            _ctx.LineArmLeft.SetPosition(0, _ctx.ShoulderLeft.position);
            _ctx.LineArmLeft.SetPosition(1, _ctx.IkArmLeft.position);
            _ctx.LineArmRight.SetPosition(0, _ctx.ShoulderRight.position);
            _ctx.LineArmRight.SetPosition(1, _ctx.IkArmRight.position);
        }

        if (_ctx.Grab.WasReleasedThisFrame() && _ctx.ArmDetection.ObjectDetected == 2)
        {
            SwitchState(_factory.Headbutt());
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
                if (_ctx.UseSpine == false)
                {
                    _ctx.Arms.gameObject.SetActive(false);
                }
                SwitchState(_factory.MeteorStrike());
            }
        }
    }
}