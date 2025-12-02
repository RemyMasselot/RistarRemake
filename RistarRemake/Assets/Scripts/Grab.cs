using DG.Tweening;
using UnityEngine;
using static ArmDetection;
using static PlayerStateMachine;

public class Grab : MonoBehaviour
{
    /// <summary>
    /// POUVOIR SPAM LE GRAB EN UTILISANT LA TECHNIQUE DU JUMP BUFERING
    /// </summary>
    /// 

    private PlayerStateMachine _player;

    public PlayerBaseState NewStateFromGrab = null;

    private bool canStartGrabSituation = true;
    private bool canCancelGrab = true;
    private bool isHoldGrabTimerRunning = false;
    private float currentHoldGrabTimerValue;

    private void Start()
    {
        _player = GetComponent<PlayerStateMachine>();
    }

    #region INITIALISATION

    public void GrabInitialisation()
    {
        _player.NewStatePlayed.Invoke();

        NewStateFromGrab = null;
        canStartGrabSituation = true;
        canCancelGrab = true;
        isHoldGrabTimerRunning = false;
        _player.ArmDetection.ObjectDetected = (int)ObjectDetectedIs.Nothing;
        _player.AimDir = _player.Aim.ReadValue<Vector2>();

        DirectionCorrection();

        MoveLimbsToDefaultPosition();

        ExtendArms();
    }

    private void DirectionCorrection()
    {
        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsDectected == true)
        {
            if (_player.AimDir.y <= 0)
            {
                _player.AimDir.y = 0;
            }
        }

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
        else
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
        Vector2 _grabDirection = _player.AimDir.normalized * _player.DistanceGrab;

        if (_player.GetComponentInChildren<PlayerVisualSpine>() != null)
        {
            if (_player.AimDir.x < 0)
            {
                _grabDirection.x *= -1;
            }
        }

        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_player.ShoulderLeft.localPosition.x + _grabDirection.x, _player.ShoulderLeft.localPosition.y + _grabDirection.y);
        _player.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _player.TimeToExtendArms);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_player.ShoulderRight.localPosition.x + _grabDirection.x, _player.ShoulderRight.localPosition.y + _grabDirection.y);
        _player.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _player.TimeToExtendArms).OnComplete(() =>
        {
            //Debug.Log("Arms Extended");
            StartHoldGrabTimer();
        });
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
            MoveArmDectection();

            GrabDetectionVerif();

            CancelGrab();
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
            switch (_player.ArmDetection.ObjectDetected)
            {
                case (int)ObjectDetectedIs.Other:
                    GrabOther();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectDetectedIs.Enemy:
                    GrabEnemy();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectDetectedIs.Ladder:
                    GrabLadder();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectDetectedIs.StarHandle:
                    GrabStarHandle();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectDetectedIs.Wall:
                    GrabWall();
                    canStartGrabSituation = false;
                    break;
                case (int)ObjectDetectedIs.Floor:
                    GrabFloor();
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

            if (_player.Grab.WasReleasedThisFrame())
            {
                ShortenArms();
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