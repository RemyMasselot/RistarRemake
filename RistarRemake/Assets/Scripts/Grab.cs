using DG.Tweening;
using UnityEngine;
using static ArmDetection;

public class Grab : MonoBehaviour
{
    private PlayerStateMachine _player;

    public PlayerBaseState NewStateFromGrab = null;

    private bool canStartGrabSituation = true;
    private bool canCancelGrab = true;
    private bool isHoldGrabTimerRunning = false;
    private float currentHoldGrabTimerValue;
    private bool canStartHoldGrabTimer = true;

    public bool CanCountGrabBufferTime = false;

    private void Start()
    {
        _player = GetComponent<PlayerStateMachine>();
    }

    #region INITIALISATION

    public void GrabInitialisation()
    {
        _player.ArmDetection.ObjectGrabed = (int)ObjectGrabedIs.Nothing;
        NewStateFromGrab = null;
        canStartGrabSituation = true;
        canCancelGrab = true;
        isHoldGrabTimerRunning = false;
        canStartHoldGrabTimer = true;
        _player.ElementGrabed = null;
        Debug.Log("Grab Initialisation");

        DirectionCorrection();

        MoveLimbsToDefaultPosition();

        _player.NewStatePlayed.Invoke();

        ExtendArms();
    }

    private void DirectionCorrection()
    {
        if (_player.Aim.ReadValue<Vector2>() == new Vector2(0, 0))
        {
            _player.AimDir = new Vector2 (_player.MoveH.ReadValue<float>(), _player.MoveV.ReadValue<float>());

            if (_player.AimDir == new Vector2(0, 0))
            {
                if (_player.IsPlayerTurnToLeft == false)
                {
                    _player.AimDir = new Vector2(1, 0);
                }
                else
                {
                    _player.AimDir = new Vector2(-1, 0);
                }
            }
        }
        else
        {
            _player.AimDir = _player.Aim.ReadValue<Vector2>();
        }

        //Correction facing direction
        if (_player.AimDir != new Vector2(0, 0))
        {
            if (_player.AimDir.x > 0)
            {
                _player.IsPlayerTurnToLeft = false;
            }
            else if (_player.AimDir.x < 0)
            {
                _player.IsPlayerTurnToLeft = true;
            }
        }
        
        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsDectected == true)
        {
            if (_player.AimDir.y <= 0)
            {
                _player.AimDir.y = 0;
            }
        }
    }

    private void MoveLimbsToDefaultPosition()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.DefaultPosRight.position;

        // Déplacement de Arm Detection pour suivre les mains
        _player.ArmDetection.GetComponent<Transform>().position = (_player.IkArmLeft.position + _player.IkArmRight.position) / 2;
        float angle = Mathf.Atan2(_player.AimDir.y, _player.AimDir.x) * Mathf.Rad2Deg;
        _player.ArmDetection.rotationOffset = angle;

        _player.ArmDetection.gameObject.SetActive(true);
    }

    private void ExtendArms()
    {
        if (_player.ArmDetection.ObjectGrabed != (int)ObjectGrabedIs.Nothing)
        {
            Debug.Log("Object Grabed : " + _player.ArmDetection.ObjectGrabed);
            return;
        }

        Vector2 _grabDirection = _player.AimDir.normalized * _player.DistanceGrab;

        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_player.ShoulderLeft.position.x + _grabDirection.x, _player.ShoulderLeft.position.y + _grabDirection.y);

        Vector2 pos = _player.IkArmLeft.position;
        float step = _player.ExtendArmSpeed * Time.deltaTime;
        float dist = Vector2.Distance(pos, PointDestinationArmLeft);

        if (dist <= 0.001f || step >= dist)
        {
            _player.IkArmLeft.position = PointDestinationArmLeft;
        }

        _player.IkArmLeft.position = Vector2.MoveTowards(pos, PointDestinationArmLeft, step);


        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_player.ShoulderRight.position.x + _grabDirection.x, _player.ShoulderRight.position.y + _grabDirection.y);

        Vector2 posR = _player.IkArmRight.position;
        float stepR = _player.ExtendArmSpeed * Time.deltaTime;
        float distR = Vector2.Distance(posR, PointDestinationArmRight);

        if (distR <= 0.001f || stepR >= distR)
        {
            _player.IkArmRight.position = PointDestinationArmRight;
        }

        _player.IkArmRight.position = Vector2.MoveTowards(posR, PointDestinationArmRight, stepR);


        float distanceToTarget = Vector2.Distance(_player.IkArmRight.position, PointDestinationArmRight);
        if (distanceToTarget <= 0.01f && canStartHoldGrabTimer)
        {
            canStartHoldGrabTimer = false;
            StartHoldGrabTimer();
        }
    }

    void StartHoldGrabTimer()
    {
        currentHoldGrabTimerValue = _player.MaxHoldGrabTime;
        isHoldGrabTimerRunning = true;
    }

    #endregion

    public void Update()
    {
        if (_player.IsGrabing)
        {
            DirectionCorrection();

            MoveArmDectection();

            ExtendArms();

            GrabDetectionVerif();

            CancelGrab();

            if (_player.Grab.WasPerformedThisFrame())
            {
                CanCountGrabBufferTime = true;
                _player.GrabBufferCounter = 0;
            }
            if (CanCountGrabBufferTime)
            {
                _player.GrabBufferCounter += Time.deltaTime;
            }
        }
    }

    private void MoveArmDectection()
    {
        _player.ArmDetection.GetComponent<Transform>().position = (_player.IkArmLeft.position + _player.IkArmRight.position) / 2;
        float angle = Mathf.Atan2(_player.AimDir.y, _player.AimDir.x) * Mathf.Rad2Deg;
        _player.ArmDetection.rotationOffset = angle;
    }

    #region SITUATIONS GRAB

    public void GrabDetectionVerif()
    {
        // Enter DAMAGE STATE
        //if (_player.Invincinbility.IsInvincible == false)
        //{
        //    if (_player.EnemyDetection.IsGroundDectected == true)
        //    {
        //        UpdatePlayerState(_player.StatesFactory.Damage());
        //    }
        //}
        if (canStartGrabSituation)
        {
            //Debug.Log(_player.ArmDetection.ObjectDetected);
            switch (_player.ArmDetection.ObjectGrabed)
            {
                case (int)ObjectGrabedIs.Other:
                    GrabOther();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.Enemy:
                    GrabEnemy();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.LadderVertical:
                    GrabLadder();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.LadderHorizontal:
                    GrabLadder();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.StarHandle:
                    GrabStarHandle();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.Wall:
                    GrabWall();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.Floor:
                    GrabFloor();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectGrabedIs.Ceiling:
                    GrabWall();
                    canStartGrabSituation = false;
                    break;
            }
        }
    }
    private void GrabOther()
    {
        //Debug.Log("Other Detected");
        ShortenArms();
    }
    private void GrabWall()
    {
        //Debug.Log("Wall Detected");
        ExitGrab();
        NewStateFromGrab = _player.StatesFactory.Headbutt();
    }
    private void GrabFloor()
    {
        //Debug.Log("Floor Detected");
        ShortenArms();
    }
    private void GrabEnemy()
    {
        //Debug.Log("Enemy Detected ");
        ExitGrab();
        NewStateFromGrab = _player.StatesFactory.Hang();
    }
    private void GrabLadder()
    {
        //Debug.Log("Ladder Detected");
        ExitGrab();
        NewStateFromGrab = _player.StatesFactory.Headbutt();
    }
    private void GrabStarHandle()
    {
        //Debug.Log("Star Handle Detected");
        ExitGrab();
        NewStateFromGrab = _player.StatesFactory.Hang();
    }

    #endregion

    private void CancelGrab()
    {
        if (canCancelGrab)
        {
            if (isHoldGrabTimerRunning == true)
            {
                currentHoldGrabTimerValue -= Time.deltaTime;
                if (currentHoldGrabTimerValue <= 0f)
                {
                    ShortenArms();
                }
            }

            float distanceToPlayer = Vector2.Distance(_player.ArmDetection.GetComponent<Transform>().position, _player.transform.position);

            if (distanceToPlayer >= 1.5f)
            {
                if (_player.Grab.WasReleasedThisFrame()
                    || _player.Grab.ReadValue<float>() <= 0f)
                {
                    ShortenArms();
                }
            }
        }
    }

    public void ShortenArms()
    {
        canCancelGrab = false;
        isHoldGrabTimerRunning = false;

        // Move Left Arm
        _player.IkArmLeft.transform.DOLocalMove(_player.DefaultPosLeft.localPosition, _player.TimeToExtendArms);
        // Move Right Arm
        _player.IkArmRight.transform.DOLocalMove(_player.DefaultPosRight.localPosition, _player.TimeToExtendArms).OnComplete(() =>
        {
            ExitGrab();
        });
    }

    public void ExitGrab()
    {
        _player.ArmDetection.gameObject.SetActive(false);
        _player.IsGrabing = false;
        _player.NewStatePlayed.Invoke();
    }
}