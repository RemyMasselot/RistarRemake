using UnityEngine;
using DG.Tweening;
using static PlayerStateMachine;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    /// <summary>
    /// POUVOIR SPAM LE GRAB EN UTILISANT LA TECHNIQUE DU JUMP BUFERING
    /// </summary>

    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _player.ArmDetection.ObjectDetected = 0;
        _player.AimDir = _player.Aim.ReadValue<Vector2>();

        DirectionCorrection();

        MoveLimbsToDefaultPosition();

        ExtendArms();

        StartTimer();
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
        Vector2 _grabDirection = _player.AimDir.normalized *_player.DistanceGrab;

        if (_player.GetComponentInChildren<PlayerVisualSpine>() != null)
        {
            if (_player.AimDir.x < 0)
            {
                _grabDirection.x *= -1;
            }
        }

        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_player.ShoulderLeft.localPosition.x + _grabDirection.x, _player.ShoulderLeft.localPosition.y + _grabDirection.y);
        _player.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _player.DurationExtendGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_player.ShoulderRight.localPosition.x + _grabDirection.x, _player.ShoulderRight.localPosition.y + _grabDirection.y);
        _player.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _player.DurationExtendGrab);
    }

    void StartTimer()
    {
        _player.CurrentTimerValue = _player.MaxTimeGrab;
        _player.IsTimerRunning = true;
    }

    public override void UpdateState()
    {
        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == false)
        {
            // Air Control
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (moveValueH != 0)
            {
                _player.PlayerRigidbody.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.PlayerRigidbody.velocity.y);
            }
            else
            {
                _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y);
            }
        }

        if (_player.ArmDetection.ObjectDetected == 3 || _player.ArmDetection.ObjectDetected == 5)
        {
            DOTween.Kill(_player.IkArmLeft);
            DOTween.Kill(_player.IkArmRight);
            _player.IkArmLeft.position = _player.ArmDetection.SnapPosHandL;
            _player.IkArmRight.position = _player.ArmDetection.SnapPosHandR;
            //Debug.Log("dfef");
        }
        else
        {
            if (_player.IsTimerRunning == true)
            {
                _player.CurrentTimerValue -= Time.deltaTime;
                if (_player.CurrentTimerValue <= 0f)
                {
                    _player.IsTimerRunning = false;
                    ShortenArms();
                }
            }

            if (_player.Grab.WasReleasedThisFrame())
            {
                _player.IsTimerRunning = false;
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
        _player.IkArmLeft.transform.DOLocalMove(_player.DefaultPosLeft.localPosition, _player.DurationExtendGrab);
        // Move Right Arm
        _player.IkArmRight.transform.DOLocalMove(_player.DefaultPosRight.localPosition, _player.DurationExtendGrab).OnComplete(() =>
        {
            if (_player.ArmDetection.ObjectDetected == 0)
            {
                // Vérification d'un sol ou non
                if (_player.GroundDetection.IsGroundDectected == false)
                {
                    SwitchState(_factory.Fall());
                }
                else
                {
                    // Passage en WALK ou IDLE
                    float moveValue = _player.MoveH.ReadValue<float>();
                    if (moveValue != 0)
                    {
                        SwitchState(_factory.Walk());
                    }
                    else
                    {
                        SwitchState(_factory.Idle());
                    }
                }
            }
        });
    }

    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallIdle());
        }
        
        if (collision.gameObject.CompareTag("Wall"))
        {
            SwitchState(_factory.Spin());
        }
    }

    public void GrabDetectionVerif()
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        switch (_player.ArmDetection.ObjectDetected)
        {
            case 1:
                GrabOther();
                break;
            case 2:
                GrabEnemy();
                break;
            case 3:
                GrabLadder();
                break;
            case 4:
                GrabStarHandle();
                break;
            case 5:
                GrabWall();
                break;
            case 6:
                GrabFloor();
                break;
        }
        //Debug.Log(_ctx.ArmDetection.ObjectDetected);
    }

    private void GrabOther()
    {
        Debug.Log("Other Detected");
        _player.IsTimerRunning = false;
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
        _player.IkArmLeft.transform.DOLocalMove(_player.DefaultPosLeft.localPosition, _player.DurationExtendGrab);
        // Move Right Arm
        _player.IkArmRight.transform.DOLocalMove(_player.DefaultPosRight.localPosition, _player.DurationExtendGrab).OnComplete(() =>
        {
            // Vérification d'un sol ou non
            if (_player.GroundDetection.IsGroundDectected == false)
            {
                SwitchState(_factory.Fall());
            }
            else
            {
                // Passage en WALK ou IDLE
                float moveValue = _player.MoveH.ReadValue<float>();
                if (moveValue != 0)
                {
                    SwitchState(_factory.Walk());
                }
                else
                {
                    SwitchState(_factory.Idle());
                }
            }
        });
        _player.ArmDetection.ObjectDetected = 0;
    }
    private void GrabEnemy()
    {
        //Debug.Log("Enemy Detected");
        SwitchState(_factory.Hang());
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
        SwitchState(_factory.Hang());
    }
}