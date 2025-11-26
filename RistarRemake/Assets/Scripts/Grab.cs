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

    public PlayerBaseState NewState = null;

    private bool isHoldGrabTimerRunning = false;
    private float currentHoldGrabTimerValue;

    private void Start()
    {
        _player = GetComponent<PlayerStateMachine>();

        NewState = null;
        _player.ArmDetection.ObjectDetected = (int)ObjectDetectedIs.Nothing;
        _player.AimDir = _player.Aim.ReadValue<Vector2>();
    }

    public void GrabInitialisation()
    {
        _player.NewStatePlayed.Invoke();

        NewState = null;
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
        if (_player.GroundDetection.IsGroundDectected == true)
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
            Debug.Log("Arms Extended");
            StartHoldGrabTimer();
        });
    }

    void StartHoldGrabTimer()
    {
        currentHoldGrabTimerValue = _player.MaxHoldGrabTime;
        isHoldGrabTimerRunning = true;
    }

    public void Update()
    {
        if (_player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.Ladder || _player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.Wall)
        {
            DOTween.Kill(_player.IkArmLeft);
            DOTween.Kill(_player.IkArmRight);
            _player.IkArmLeft.position = _player.ArmDetection.SnapPosHandL;
            _player.IkArmRight.position = _player.ArmDetection.SnapPosHandR;
        }
        else
        {
            if (isHoldGrabTimerRunning == true)
            {
                currentHoldGrabTimerValue -= Time.deltaTime;
                if (currentHoldGrabTimerValue <= 0f)
                {
                    isHoldGrabTimerRunning = false;
                    ShortenArms();
                }
            }

            if (_player.Grab.WasReleasedThisFrame())
            {
                isHoldGrabTimerRunning = false;
                ShortenArms();
            }
        }

        // Déplacement de Arm Detection pour suivre les mains
        _player.ArmDetection.GetComponent<Transform>().position = (_player.IkArmLeft.position + _player.IkArmRight.position) / 2;
        float angle = Mathf.Atan2(_player.AimDir.y, _player.AimDir.x) * Mathf.Rad2Deg;
        _player.ArmDetection.rotationOffset = angle;


        // Parce que je n'arrive pas à référencer ce script dans le script ArmDetection, ici je vérifie à chaque frame ce que les bras ont touché,
        // plutôt que de lancer la bonne fonction au moment où les bras entre en collision avec un élément dans le script ArmDetection.
        // Un raycast envoyé dans ce script et qui influe sur l'avancé des mains pourrait être une bonne solution
        GrabDetectionVerif();
    }

    public void ShortenArms()
    {
        _player.ArmDetection.gameObject.SetActive(false);

        // Move Left Arm
        _player.IkArmLeft.transform.DOLocalMove(_player.DefaultPosLeft.localPosition, _player.TimeToExtendArms);
        // Move Right Arm
        _player.IkArmRight.transform.DOLocalMove(_player.DefaultPosRight.localPosition, _player.TimeToExtendArms).OnComplete(() =>
        {
            if (_player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.Nothing)
            {
                // Vérification d'un sol ou non
                if (_player.GroundDetection.IsGroundDectected == false)
                {
                    UpdatePlayerState(_player.StatesFactory.Fall());
                }
                else
                {
                    // Passage en WALK ou IDLE
                    float moveValue = _player.MoveH.ReadValue<float>();
                    if (moveValue != 0)
                    {
                        UpdatePlayerState(_player.StatesFactory.Walk());
                    }
                    else
                    {
                        UpdatePlayerState(_player.StatesFactory.Idle());
                    }
                }
            }
        });
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            if (_player.CurrentState is not PlayerWallIdleState && _player.CurrentState is not PlayerWallClimbState)
            {
                UpdatePlayerState(_player.StatesFactory.WallIdle());
                Debug.Log("LADDER GRAB COLLISION");
            }
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (_player.CurrentState is PlayerHeadbuttState)
            {
                UpdatePlayerState(_player.StatesFactory.Headbutt());
            }
        }
    }

    public void GrabDetectionVerif()
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                UpdatePlayerState(_player.StatesFactory.Damage());
            }
        }

        switch (_player.ArmDetection.ObjectDetected)
        {
            case (int)ObjectDetectedIs.Other:
                GrabOther();
                break;
            case (int)ObjectDetectedIs.Enemy:
                GrabEnemy();
                break;
            case (int)ObjectDetectedIs.Ladder:
                GrabLadder();
                break;
            case (int)ObjectDetectedIs.StarHandle:
                GrabStarHandle();
                break;
            case (int)ObjectDetectedIs.Wall:
                GrabWall();
                break;
            case (int)ObjectDetectedIs.Floor:
                GrabFloor();
                break;
        }
        //Debug.Log(_ctx.ArmDetection.ObjectDetected);
    }

    private void GrabOther()
    {
        Debug.Log("Other Detected");
        isHoldGrabTimerRunning = false;
        ShortenArms();
    }
    private void GrabWall()
    {
        Debug.Log("Wall Detected");
        _player.ArmDetection.gameObject.SetActive(false);

        _player.PlayerRigidbody.velocity = _player.AimDir.normalized * 10;
    }
    private void GrabFloor()
    {
        Debug.Log("Floor Detected");
        _player.ArmDetection.gameObject.SetActive(false);

        // Move Left Arm
        _player.IkArmLeft.transform.DOLocalMove(_player.DefaultPosLeft.localPosition, _player.TimeToExtendArms);
        // Move Right Arm
        _player.IkArmRight.transform.DOLocalMove(_player.DefaultPosRight.localPosition, _player.TimeToExtendArms).OnComplete(() =>
        {
            // Vérification d'un sol ou non
            if (_player.GroundDetection.IsGroundDectected == false)
            {
                UpdatePlayerState(_player.StatesFactory.Fall());
            }
            else
            {
                // Passage en WALK ou IDLE
                float moveValue = _player.MoveH.ReadValue<float>();
                if (moveValue != 0)
                {
                    UpdatePlayerState(_player.StatesFactory.Walk());
                }
                else
                {
                    UpdatePlayerState(_player.StatesFactory.Idle());
                }
            }
        });
        _player.ArmDetection.ObjectDetected = (int)ObjectDetectedIs.Nothing;
    }
    private void GrabEnemy()
    {
        //Debug.Log("Enemy Detected");
        UpdatePlayerState(_player.StatesFactory.Hang());
    }
    private void GrabLadder()
    {
        //Debug.Log("Ladder Detected");
        _player.ArmDetection.gameObject.SetActive(false);

        _player.PlayerRigidbody.velocity = _player.AimDir.normalized * 10;
    }
    private void GrabStarHandle()
    {
        //Debug.Log("Star Handle Detected");
        UpdatePlayerState(_player.StatesFactory.Hang());
    }

    private void UpdatePlayerState(PlayerBaseState newState)
    {
        _player.IsGrabing = false;
        if (NewState != null)
        {
            NewState = newState;
        }
        _player.NewStatePlayed.Invoke();
    }
}