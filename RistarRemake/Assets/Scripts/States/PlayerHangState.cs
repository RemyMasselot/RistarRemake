using DG.Tweening;
using TMPro;
using UnityEngine;
using static ArmDetection;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerHangState : PlayerBaseState
{
    public PlayerHangState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    /// <summary>
    /// CHANGER LES CONTROLES :
    /// - MAINTENIR RIGHT SHOULDER POUR CHARGER LE METEOR STRIKE
    /// - DIRIGER LA DIRECTION DE PROPULSION AVEC LE JOYSTICK GAUCHE
    /// - RELACHER RIGHT SHOULDER POUR LANCER
    /// 
    /// RETIRER LE SLOW MOTION ET LE TRIGGER DE PROPULSION
    /// </summary>

    private bool snapHands = false;
    private float starHandleAngle = 0;
    private bool canGoMeteorStrike = false;
    private bool isGoingToMeteorStrike = false;
    private float meteorStrikeChargeTimer = 0;
    private Vector2 meteorStrikeDirection;

    public override void EnterState()
    {
        Debug.Log("ENTER HANG");

        // CAMERA BEHAVIOR
        _player.CameraTargetOverride = _player.ArmDetection.SnapPosHand;

        snapHands = false;
        canGoMeteorStrike = false;
        isGoingToMeteorStrike = false;
        _player.StarHandleCurrentValue = 0;
        starHandleAngle = -1.5f;
        _player.StarHandleCurrentRayon = _player.StarHandleRayonMin;
        meteorStrikeChargeTimer = 0;

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
                if (isGoingToMeteorStrike == false)
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
                }
                else if (UpdateRotateToPosition(_player.StarHandleCentre, _player.StarHandleCurrentRayon))
                {
                    SwitchState(_factory.MeteorStrike());
                    /*
                    Vector2 meteorStrikeStartPoint = new Vector2(_player.StarHandleCentre.x + meteorStrikeDirection.x, _player.StarHandleCentre.y + meteorStrikeDirection.y) * _player.StarHandleCurrentRayon;
                    float distanceToMeteorStrikeStartPoint = Vector2.Distance(_player.transform.position, meteorStrikeStartPoint);

                    Debug.Log("Distance to Meteor Strike Start Point: " + distanceToMeteorStrikeStartPoint);

                    if (distanceToMeteorStrikeStartPoint <= 30)
                    {
                        SwitchState(_factory.MeteorStrike());
                    }*/
                }
            }
        }
    }

    // Champs à ajouter dans la classe (private)
    private bool rotatingToPos = false;
    private float rotateDuration = 0f;
    private float rotateElapsed = 0f;
    private float rotateStartAngleDeg = 0f;
    private float rotateTargetAngleDeg = 0f;

    // Démarrer la rotation vers une position cible en un temps donné (seconds)
    private void StartRotateToPosition(Vector2 center, Vector2 targetPosition, float duration)
    {
        rotateDuration = Mathf.Max(0.0001f, duration);
        rotateElapsed = 0f;
        rotateStartAngleDeg = starHandleAngle * Mathf.Rad2Deg;
        rotateTargetAngleDeg = Mathf.Atan2(targetPosition.y - center.y, targetPosition.x - center.x) * Mathf.Rad2Deg;
        rotatingToPos = true;
    }

    private bool UpdateRotateToPosition(Vector2 center, float radius)
    {
        if (!rotatingToPos) return false;

        rotateElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(rotateElapsed / rotateDuration);
        float newDeg = Mathf.LerpAngle(rotateStartAngleDeg, rotateTargetAngleDeg, t);
        starHandleAngle = newDeg * Mathf.Deg2Rad;

        // Mettre à jour la position autour du centre
        _player.transform.position = center + new Vector2(Mathf.Cos(starHandleAngle), Mathf.Sin(starHandleAngle)) * radius;

        // Optionnel : orienter le corps pour regarder à l'extérieur du centre
        float bodyAngle = Mathf.Atan2(_player.transform.position.y - center.y, _player.transform.position.x - center.x) * Mathf.Rad2Deg + 180f;
        _player.transform.rotation = Quaternion.Euler(0f, 0f, bodyAngle);

        if (t >= 1f)
        {
            rotatingToPos = false;
            return true;
        }

        return false;
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
                    meteorStrikeDirection = new Vector2(_player.MoveH.ReadValue<float>(), _player.MoveV.ReadValue<float>()) * _player.StarHandleCurrentRayon;
                    Vector2 meteorStrikeStartPoint = new Vector2(_player.StarHandleCentre.x + meteorStrikeDirection.x, _player.StarHandleCentre.y + meteorStrikeDirection.y);
                    StartRotateToPosition(_player.StarHandleCentre, meteorStrikeStartPoint, 0.2f);
                    isGoingToMeteorStrike = true;
                }
                else
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
   
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SwitchState(_factory.Spin());
        }
    }

    public override void OnCollisionStay2D(Collision2D collision) { }
}