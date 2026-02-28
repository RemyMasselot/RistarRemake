using DG.Tweening;
using UnityEngine;
using static ArmDetection;
using static PlayerStateMachine;

public class PlayerHeadbuttState : PlayerBaseState
{
    public PlayerHeadbuttState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool headbuttIsMoving = false;
    private Vector2 headbuttMoveStart;
    private Vector2 headbuttMoveTarget;
    private float headbuttMoveDuration = 0f;
    private float headbuttMoveElapsed = 0f;
    private AnimationCurve headbuttAccelerationCurve;

    public override void EnterState()
    {
        //Debug.Log("ENTER HEADBUTT");

        _player.IsLadder = (int)LadderIs.Nothing;

        if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Enemy)
        {
            headbuttAccelerationCurve = _player.HeadbuttAccelerationCurveEnnemy;
        }
        else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Floor)
        {
            headbuttAccelerationCurve = _player.HeadbuttAccelerationCurveFloor;
        }
        else
        {
            headbuttAccelerationCurve = _player.HeadbuttAccelerationCurvePlatform;
        }

        // CAMERA BEHAVIOR
        _player.CameraTargetOverride = _player.ArmDetection.SnapPosHand;

        _player.GrabScript.NewStateFromGrab = null;

        _player.HeadbuttDirection = _player.ArmDetection.SnapPosHand - _player.transform.position;

        DOTween.Kill(_player.IkArmLeft);
        DOTween.Kill(_player.IkArmRight);
    }
    public override void UpdateState()
    {
        _player.LadderVerif();

        SetHandPosition();

        MoveHeadbutt(_player.ArmDetection.SnapPosHand, headbuttAccelerationCurve, _player.HeadbuttMinDuration, _player.HeadbuttMaxDuration,  _player.DistanceGrab);

        CheckDistanceWithTarget();
    }

    private bool MoveHeadbutt(Vector2 target, AnimationCurve accelCurve, float timeMin, float timeMax, float maxDistanceForTimeMax)
    {
        Vector2 pos = _player.transform.position;
        float remaining = Vector2.Distance(pos, target);

        // snap si déjà sur la cible
        if (remaining <= 0.001f)
        {
            _player.transform.position = target;
            headbuttIsMoving = false;
            return true;
        }

        // calculer la durée cible en fonction de la distance initiale (sur la première frame)
        if (!headbuttIsMoving || headbuttMoveTarget != target)
        {
            headbuttIsMoving = true;
            headbuttMoveStart = pos;
            headbuttMoveTarget = target;
            headbuttMoveElapsed = 0f;

            float fullDist = Vector2.Distance(headbuttMoveStart, headbuttMoveTarget);
            // éviter division par zéro
            float denom = Mathf.Max(0.0001f, maxDistanceForTimeMax);
            float distanceNorm = Mathf.Clamp01(fullDist / denom); // 0 => proche => timeMin, 1 => très loin => timeMax
            headbuttMoveDuration = Mathf.Lerp(timeMin, timeMax, distanceNorm);
        }

        // progression temporelle (0..1)
        headbuttMoveElapsed += Time.deltaTime;
        float s = Mathf.Clamp01(headbuttMoveElapsed / Mathf.Max(0.0001f, headbuttMoveDuration));

        // appliquer la courbe d'accélération (fallback linéaire si null)
        float eval = accelCurve != null ? Mathf.Clamp01(accelCurve.Evaluate(s)) : s;

        // interpolation de la position selon eval
        Vector2 newPos = Vector2.Lerp(headbuttMoveStart, headbuttMoveTarget, eval);
        _player.transform.position = newPos;

        if (s >= 1f)
        {
            headbuttIsMoving = false;
            return true;
        }

        return false;
    }

    private void SetHandPosition()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHandL;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHandR;
    }

    public override void FixedUpdateState() 
    {    
        if (_player.EnemyDetection.IsDectected == true)
        {
            SwitchState(_factory.Spin());
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

    public override void OnTriggerStay2D(Collider2D collider) { }

    private void CheckDistanceWithTarget()
    {
        float distanceWithTarget = Vector2.Distance(_player.transform.position, _player.ArmDetection.SnapPosHand);
        
        if (distanceWithTarget <= DistanceRelative())
        {
            if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.LadderVertical
                || _player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.LadderHorizontal)
            {
                //Debug.Log("LADDER DETECTED AFTER HEADBUTT");
                if (_player.IsLadder != (int)LadderIs.Nothing)
                {
                    SwitchState(_factory.WallIdle());
                }
            }
            else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Floor)
            {
                SwitchState(_factory.Idle());
            }
            else
            {
                SwitchState(_factory.Spin());
            }
        }
    }

    private float DistanceRelative()
    {
        Vector2 closestPoint = _player.PlayerCollider.ClosestPoint(_player.ArmDetection.SnapPosHand);
        
        float distance = Vector2.Distance(_player.transform.position, closestPoint);

        return distance + 0.2f;
    }
}