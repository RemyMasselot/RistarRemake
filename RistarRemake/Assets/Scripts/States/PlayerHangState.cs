using DG.Tweening;
using TMPro;
using UnityEngine;
using static ArmDetection;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool snapHands = false;
    private float starHandleAngle = 0;
    private bool canGoMeteorStrike = false;
    private float meteorStrikeChargeTimer = 0;
    private Vector2 meteorStrikeDirection;
    private Vector2 meteorStrikeStartPoint;
    private Vector2 outHandleDirection;
    private Vector2 outHandleStartPoint;

    public override void EnterState()
    {
        //Debug.Log("ENTER HANG");

        // CAMERA BEHAVIOR
        _player.CameraTargetOverride = _player.ArmDetection.SnapPosHand;

        snapHands = false;
        canGoMeteorStrike = false;
        _player.StarHandleCurrentValue = 0;
        starHandleAngle = -1.5f;
        _player.StarHandleCurrentRayon = _player.StarHandleRayonMin;
        meteorStrikeChargeTimer = 0;
        meteorStrikeStartPoint = Vector2.zero;
        outHandleStartPoint = Vector2.zero;

        _player.PlayerRigidbody.velocity = Vector2.zero;
        _player.GrabScript.NewStateFromGrab = null;

        // Move Left Arm
        _player.IkArmLeft.transform.DOMove(_player.ArmDetection.SnapPosHand, 0.2f);
        // Move Right Arm
        _player.IkArmRight.transform.DOMove(_player.ArmDetection.SnapPosHand, 0.2f).OnComplete(()=>
        {
            snapHands = true;
        });

        if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.StarHandle)
        {
            _player.StarHandleCentre = _player.ArmDetection.SnapPosHand;
            float x = _player.StarHandleCentre.x + Mathf.Cos(starHandleAngle) * _player.StarHandleCurrentRayon;
            float y = _player.StarHandleCentre.y + Mathf.Sin(starHandleAngle) * _player.StarHandleCurrentRayon;
            _player.transform.DOMove(new Vector2(x, y), 0.3f);
        }
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

        KeepHandPositionToSnapPoint();

        CheckSwitchStates();
    }

    private void KeepHandPositionToSnapPoint()
    {
        if (snapHands == true)
        {
            // Move Left Arm
            _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionL = (Vector2)(_player.IkArmLeft.position - _player.ShoulderLeft.position);
            float angleL = Mathf.Atan2(directionL.y, directionL.x) * Mathf.Rad2Deg;

            // Move Right Arm
            _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHand;
            // Hands rotation
            Vector2 directionR = (Vector2)(_player.IkArmRight.position - _player.ShoulderRight.position);
            float angleR = Mathf.Atan2(directionR.y, directionR.x) * Mathf.Rad2Deg;
        }
    }

    public override void FixedUpdateState() 
    {
        MovementHangingEnemy();

        ChargingMeteorStrike();
    }

    private void MovementHangingEnemy()
    {
        
    }
    
    private void TimerMeteorStrikeCharge()
    {
        meteorStrikeChargeTimer += Time.deltaTime;
    }

    private void ChargingMeteorStrike()
    {
        if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.StarHandle)
        {
            if (snapHands)
            {
                TimerMeteorStrikeCharge();

                //Tourner autour du Star Handle
                if (_player.IsPlayerTurnToLeft == true)
                {
                    starHandleAngle -= _player.StarHandleCurrentSpeed * Time.deltaTime;
                }
                else
                {
                    starHandleAngle += _player.StarHandleCurrentSpeed * Time.deltaTime;
                }

                float x = _player.StarHandleCentre.x + Mathf.Cos(starHandleAngle) * _player.StarHandleCurrentRayon;
                float y = _player.StarHandleCentre.y + Mathf.Sin(starHandleAngle) * _player.StarHandleCurrentRayon;
                _player.transform.position = new Vector2(x, y);

                if (_player.IsPlayerTurnToLeft == true)
                {
                    // Body Rotation
                    Vector2 direction = _player.transform.position - _player.ArmDetection.SnapPosHand;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    _player.transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                else
                {
                    // Body Rotation
                    Vector2 direction = _player.ArmDetection.SnapPosHand - _player.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    _player.transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                if (meteorStrikeChargeTimer >= _player.TimeToChargeMeteorStrike)
                {
                    meteorStrikeChargeTimer = _player.TimeToChargeMeteorStrike;
                    canGoMeteorStrike = true;
                }

                float percent = meteorStrikeChargeTimer / _player.TimeToChargeMeteorStrike * 100f;
                _player.StarHandleCurrentRayon = _player.StarHandleRayonMin + (_player.StarHandleRayonMax - _player.StarHandleRayonMin) * (percent / 100f);
                _player.StarHandleCurrentSpeed = _player.StarHandleMinSpeed + (_player.StarHandleMaxSpeed - _player.StarHandleMinSpeed) * (percent / 100f);

                float distanceToMeteorStrikeStartPoint = Vector2.Distance(_player.transform.position, meteorStrikeStartPoint);
                if (distanceToMeteorStrikeStartPoint <= 0.5f)
                {
                    SwitchState(_factory.MeteorStrike());
                }

                float distanceToOutHandleStartPoint = Vector2.Distance(_player.transform.position, outHandleStartPoint);
                if (distanceToOutHandleStartPoint <= 0.5f)
                {
                    // Move Left Arm
                    _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
                    // Move Right Arm
                    _player.IkArmRight.transform.position = _player.DefaultPosRight.position;

                    Vector2 moveDir = new Vector2(_player.MoveH.ReadValue<float>(), _player.MoveV.ReadValue<float>());

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
    }

    public override void ExitState() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (_player.Grab.WasReleasedThisFrame())
        {
            if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Enemy)
            {
                SwitchState(_factory.Headbutt());
            }
            else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.StarHandle)
            {
                _player.transform.rotation = Quaternion.Euler(0, 0, 0);

                if (canGoMeteorStrike == true)
                {
                    meteorStrikeDirection = new Vector2(_player.MoveH.ReadValue<float>(), _player.MoveV.ReadValue<float>()).normalized * _player.StarHandleCurrentRayon;
                    meteorStrikeStartPoint = new Vector2(_player.StarHandleCentre.x + meteorStrikeDirection.x, _player.StarHandleCentre.y + meteorStrikeDirection.y);
                    //Debug.Log("Meteor Strike Direction : " + meteorStrikeDirection);
                    if (meteorStrikeDirection == Vector2.zero)
                    {
                        SwitchState(_factory.MeteorStrike());
                    }
                }
                else
                {
                    outHandleDirection = new Vector2(_player.MoveH.ReadValue<float>(), _player.MoveV.ReadValue<float>()).normalized * _player.StarHandleCurrentRayon;
                    outHandleStartPoint = new Vector2(_player.StarHandleCentre.x + outHandleDirection.x, _player.StarHandleCentre.y + outHandleDirection.y);
                    //Debug.Log("Meteor Strike Direction : " + outHandleDirection);
                    if (outHandleDirection == Vector2.zero)
                    {
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
        }
    }
   
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }
    }

    public override void OnCollisionStay2D(Collision2D collision) { }
}