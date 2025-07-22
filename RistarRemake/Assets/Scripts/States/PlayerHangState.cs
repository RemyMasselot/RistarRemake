using DG.Tweening;
using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool SnapHands = false;
    private float _SHangle = 0;
    private bool _goMeteorStrike = false;

    public override void EnterState()
    {
        Debug.Log("ENTER HANG");
        _ctx.ArmDetection.gameObject.SetActive(false);
        _ctx.Rb.velocity = Vector2.zero;
        SnapHands = false;
        _goMeteorStrike = false;
        _ctx._starHandleCurrentValue = 0;
        _SHangle = -1.5f;
        _ctx.ShRayon = _ctx.ShRayonMin;

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.2f);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOMove(_ctx.ArmDetection.SnapPosHand, 0.2f).OnComplete(()=>
        {
            SnapHands = true;
        });

        if (_ctx.ArmDetection.ObjectDetected == 4)
        {
            _ctx.Animator.SetFloat("HangValue", 2);
            _ctx.ShCentre = _ctx.ArmDetection.SnapPosHand;
            float x = _ctx.ShCentre.x + Mathf.Cos(_SHangle) * _ctx.ShRayon;
            float y = _ctx.ShCentre.y + Mathf.Sin(_SHangle) * _ctx.ShRayon;
            _ctx.transform.DOMove(new Vector2(x, y), 0.3f);
        }
        else
        {
            _ctx.Animator.SetFloat("HangValue", 1);
        }

        _ctx.UpdateAnim("Hang");
    }
    public override void UpdateState()
    {
        // Enter DAMAGE STATE
        if (_ctx.ArmDetection.ObjectDetected == 4)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

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

    }

    public override void FixedUpdateState() 
    {
        if (_ctx.ArmDetection.ObjectDetected == 2)
        {
            if (_ctx.Grab.WasReleasedThisFrame())
            {
                // Move Left Arm
                _ctx.IkArmLeft.transform.position = _ctx.ArmDetection.SnapPosHand;
                // Move Right Arm
                _ctx.IkArmRight.transform.position = _ctx.ArmDetection.SnapPosHand;
                SnapHands = true;
                SwitchState(_factory.Headbutt());
            }
        }

        if (SnapHands == true)
        {
            if (_ctx.ArmDetection.ObjectDetected == 4)
            {
                //Tourner autour du Star Handle
                if (_ctx.SpriteRenderer.flipX == true)
                {
                    _SHangle += -_ctx.ShSpeed * Time.deltaTime;
                }
                else
                {
                    _SHangle += _ctx.ShSpeed * Time.deltaTime;
                }
                float x = _ctx.ShCentre.x + Mathf.Cos(_SHangle) * _ctx.ShRayon;
                float y = _ctx.ShCentre.y + Mathf.Sin(_SHangle) * _ctx.ShRayon;
                _ctx.transform.position = new Vector2(x, y);

                ChargingMeteorStrike();
            }
        }
        else
        {
            if (_ctx.Grab.WasReleasedThisFrame())
            {
                if (_ctx.UseSpine == false)
                {
                    _ctx.Arms.gameObject.SetActive(false);
                    // Move Left Arm
                    _ctx.IkArmLeft.transform.position = _ctx.DefaultPosLeft.position;
                    // Move Right Arm
                    _ctx.IkArmRight.transform.position = _ctx.DefaultPosRight.position;
                    //_ctx.ArmDetection.ObjectDetected = 0;
                    _ctx.transform.rotation = Quaternion.Euler(0, 0, 0);
                    if (_ctx.transform.position.y <= _ctx.ShCentre.y)
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
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }

        if (collision.gameObject == _ctx.TriggerGoToMeteorStrike)
        {
            SwitchState(_factory.MeteorStrike());
        }
    }

    private void ChargingMeteorStrike()
    {
        if (_ctx.SpriteRenderer.flipX == true)
        {
            // Body Rotation
            Vector2 bodyPos = new Vector2(_ctx.transform.position.x, _ctx.transform.position.y);
            Vector2 direction = bodyPos - _ctx.ArmDetection.SnapPosHand;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ctx.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (_ctx.MoveH.ReadValue<float>() < 0)
            {
                _ctx._starHandleCurrentValue++;
                //Debug.Log(_starHandleCurrentValue);
            }
            if (_ctx.MoveH.ReadValue<float>() > 0)
            {
                _ctx._starHandleCurrentValue--;
                //Debug.Log(_starHandleCurrentValue);
            }
        }
        else
        {
            // Body Rotation
            Vector2 bodyPos = new Vector2(_ctx.transform.position.x, _ctx.transform.position.y);
            Vector2 direction = _ctx.ArmDetection.SnapPosHand - bodyPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _ctx.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (_ctx.MoveH.ReadValue<float>() > 0)
            {
                _ctx._starHandleCurrentValue++;
                //Debug.Log(_starHandleCurrentValue);
            }
            if (_ctx.MoveH.ReadValue<float>() < 0)
            {
                _ctx._starHandleCurrentValue--;
                //Debug.Log(_starHandleCurrentValue);
            }
        }

        if (_ctx._starHandleCurrentValue <= 0)
        {
            _ctx._starHandleCurrentValue = 0;
        }
        if (_ctx._starHandleCurrentValue >= _ctx.StarHandleTargetValue)
        {
            _ctx._starHandleCurrentValue = _ctx.StarHandleTargetValue;
        }

        if (_ctx.Grab.WasReleasedThisFrame())
        {
            if (_ctx.UseSpine == false)
            {
                if (_ctx._starHandleCurrentValue >= _ctx.StarHandleTargetValue)
                {
                    _goMeteorStrike = true;
                }
                else
                {
                    _ctx.Arms.gameObject.SetActive(false);
                    // Move Left Arm
                    _ctx.IkArmLeft.transform.position = _ctx.DefaultPosLeft.position;
                    // Move Right Arm
                    _ctx.IkArmRight.transform.position = _ctx.DefaultPosRight.position;
                    //_ctx.ArmDetection.ObjectDetected = 0;
                    _ctx.transform.rotation = Quaternion.Euler(0, 0, 0);
                    if (_ctx.transform.position.y <= _ctx.ShCentre.y)
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

        float percent = (_ctx._starHandleCurrentValue - 0) / (_ctx.StarHandleTargetValue - 0) * 100f;
        _ctx.ShRayon = _ctx.ShRayonMin + (_ctx.ShRayonMax - _ctx.ShRayonMin) * (percent / 100f);
        if (_goMeteorStrike == false)
        {
             _ctx.ShSpeed = _ctx.ShMinSpeed + (_ctx.ShMaxSpeed - _ctx.ShMinSpeed) * (percent / 100f);
        }
        else if (_ctx.ShSpeed != _ctx.ShSpeedSlowMotion)
        {
            _ctx.ShSpeed = _ctx.ShSpeedSlowMotion;
            _ctx.TriggerGoToMeteorStrike.transform.position = _ctx.transform.position;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                _ctx.TriggerGoToMeteorStrike.SetActive(true);
            });
        }

        if (_ctx.Jump.WasReleasedThisFrame())
        {
            if (_goMeteorStrike == true)
            {
                SwitchState(_factory.MeteorStrike());
            }
        }
    }
    public override void OnCollisionStay2D(Collision2D collision) { }

}