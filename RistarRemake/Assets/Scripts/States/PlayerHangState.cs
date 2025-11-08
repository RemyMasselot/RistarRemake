using DG.Tweening;
using UnityEngine;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    /// <summary>
    /// CHANGER LES CONTROLES :
    /// - MAINTENIR RIHGT SHOULDER POUR CHARGER LE METEOR STRIKE
    /// - DIRIGER LA DIRECTION DE PROPULSION AVEC LE JOYSTICK GAUCHE
    /// - RELACHER RIGHT SHOULDER POUR LANCER
    /// 
    /// RETIRER LE SLOW MOTION ET LE TRIGGER DE PROPULSION
    /// </summary>

    private bool SnapHands = false;
    private float _SHangle = 0;
    private bool _goMeteorStrike = false;

    public override void EnterState()
    {
        Debug.Log("ENTER HANG");
        _player.ArmDetection.gameObject.SetActive(false);

        // CAMERA BEHAVIOR
        _player.CameraTargetOverride = _player.ArmDetection.SnapPosHand;

        _player.PlayerRigidbody.velocity = Vector2.zero;
        SnapHands = false;
        _goMeteorStrike = false;
        _player.StarHandleCurrentValue = 0;
        _SHangle = -1.5f;
        _player.StarHandleCurrentRayon = _player.StarHandleRayonMin;

        // Move Left Arm
        _player.IkArmLeft.transform.DOMove(_player.ArmDetection.SnapPosHand, 0.2f);
        // Move Right Arm
        _player.IkArmRight.transform.DOMove(_player.ArmDetection.SnapPosHand, 0.2f).OnComplete(()=>
        {
            SnapHands = true;
        });

        if (_player.ArmDetection.ObjectDetected == 4)
        {
            //_player.Animator.SetFloat("HangValue", 2);
            _player.StarHandleCentre = _player.ArmDetection.SnapPosHand;
            float x = _player.StarHandleCentre.x + Mathf.Cos(_SHangle) * _player.StarHandleCurrentRayon;
            float y = _player.StarHandleCentre.y + Mathf.Sin(_SHangle) * _player.StarHandleCurrentRayon;
            _player.transform.DOMove(new Vector2(x, y), 0.3f);
        }
        //else
        //{
        //    _player.Animator.SetFloat("HangValue", 1);
        //}

        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("Hang");
        //}
    }
    public override void UpdateState()
    {
        //// Enter DAMAGE STATE
        //if (_ctx.ArmDetection.ObjectDetected == 4)
        //{
        //    if (_ctx.Invincinbility.IsInvincible == false)
        //    {
        //        if (_ctx.EnemyDetection.IsGroundDectected == true)
        //        {
        //            SwitchState(_factory.Damage());
        //        }
        //    }
        //}

        // Position des mains
        if (SnapHands == true)
        {
            // Move Left Arm
            _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionL = (Vector2)(_player.IkArmLeft.position - _player.ShoulderLeft.position);
            float angleL = Mathf.Atan2(directionL.y, directionL.x) * Mathf.Rad2Deg;
            _player.IkArmLeft.rotation = Quaternion.Euler(0, 0, angleL);
            
            // Move Right Arm
            _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionR = (Vector2)(_player.IkArmRight.position - _player.ShoulderRight.position);
            float angleR = Mathf.Atan2(directionR.y, directionR.x) * Mathf.Rad2Deg;
            _player.IkArmRight.rotation = Quaternion.Euler(0, 0, angleR);
        }
    }

    public override void FixedUpdateState() 
    {
        if (_player.ArmDetection.ObjectDetected == 2)
        {
            if (_player.Grab.WasReleasedThisFrame())
            {
                // Move Left Arm
                _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHand;
                // Move Right Arm
                _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHand;
                SnapHands = true;
                SwitchState(_factory.Headbutt());
            }
        }

        if (SnapHands == true)
        {
            if (_player.ArmDetection.ObjectDetected == 4)
            {
                //Tourner autour du Star Handle
                if (_player.IsPlayerTurnToLeft == true)
                {
                    _SHangle += -_player.StarHandleCurrentSpeed * Time.deltaTime;
                }
                else
                {
                    _SHangle += _player.StarHandleCurrentSpeed * Time.deltaTime;
                }
                float x = _player.StarHandleCentre.x + Mathf.Cos(_SHangle) * _player.StarHandleCurrentRayon;
                float y = _player.StarHandleCentre.y + Mathf.Sin(_SHangle) * _player.StarHandleCurrentRayon;
                _player.transform.position = new Vector2(x, y);

                ChargingMeteorStrike();
            }
        }
        else
        {
            if (_player.Grab.WasReleasedThisFrame())
            {
                // Move Left Arm
                _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
                // Move Right Arm
                _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
                _player.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (_player.transform.position.y <= _player.StarHandleCentre.y)
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
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }

        if (collision.gameObject == _player.TriggerGoToMeteorStrike)
        {
            SwitchState(_factory.MeteorStrike());
        }
    }

    private void ChargingMeteorStrike()
    {
        if (_player.IsPlayerTurnToLeft == true)
        {
            // Body Rotation
            Vector2 bodyPos = new Vector2(_player.transform.position.x, _player.transform.position.y);
            Vector2 direction = bodyPos - _player.ArmDetection.SnapPosHand;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _player.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (_player.MoveH.ReadValue<float>() < 0)
            {
                _player.StarHandleCurrentValue++;
            }
            if (_player.MoveH.ReadValue<float>() > 0)
            {
                _player.StarHandleCurrentValue--;
            }
        }
        else
        {
            // Body Rotation
            Vector2 bodyPos = new Vector2(_player.transform.position.x, _player.transform.position.y);
            Vector2 direction = _player.ArmDetection.SnapPosHand - bodyPos;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _player.transform.rotation = Quaternion.Euler(0, 0, angle);
            if (_player.MoveH.ReadValue<float>() > 0)
            {
                _player.StarHandleCurrentValue++;
            }
            if (_player.MoveH.ReadValue<float>() < 0)
            {
                _player.StarHandleCurrentValue--;
            }
        }

        if (_player.StarHandleCurrentValue <= 0)
        {
            _player.StarHandleCurrentValue = 0;
        }
        if (_player.StarHandleCurrentValue >= _player.StarHandleTargetValue)
        {
            _player.StarHandleCurrentValue = _player.StarHandleTargetValue;
        }

        if (_player.Grab.WasReleasedThisFrame())
        {
            if (_player.StarHandleCurrentValue >= _player.StarHandleTargetValue)
            {
                _goMeteorStrike = true;
            }
            else
            {
                // Move Left Arm
                _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
                // Move Right Arm
                _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
                _player.transform.rotation = Quaternion.Euler(0, 0, 0);
                if (_player.transform.position.y <= _player.StarHandleCentre.y)
                {
                    SwitchState(_factory.Fall());
                }
                else
                {
                    SwitchState(_factory.Jump());
                }
            }
        }

        float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
        _player.StarHandleCurrentRayon = _player.StarHandleRayonMin + (_player.StarHandleRayonMax - _player.StarHandleRayonMin) * (percent / 100f);
        if (_goMeteorStrike == false)
        {
             _player.StarHandleCurrentSpeed = _player.StarHandleMinSpeed + (_player.StarHandleMaxSpeed - _player.StarHandleMinSpeed) * (percent / 100f);
        }
        else if (_player.StarHandleCurrentSpeed != _player.StarHandleSpeedSlowMotion)
        {
            _player.StarHandleCurrentSpeed = _player.StarHandleSpeedSlowMotion;
            _player.TriggerGoToMeteorStrike.transform.position = _player.transform.position;
            DOVirtual.DelayedCall(0.5f, () =>
            {
                _player.TriggerGoToMeteorStrike.SetActive(true);
            });
        }

        if (_player.Jump.WasPressedThisFrame())
        {
            if (_goMeteorStrike == true)
            {
                SwitchState(_factory.MeteorStrike());
            }
        }
    }
    public override void OnCollisionStay2D(Collision2D collision) { }

}