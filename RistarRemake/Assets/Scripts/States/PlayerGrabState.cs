using UnityEngine;
using DG.Tweening;
using static PlayerStateMachine;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _player.AimDir = _player.Aim.ReadValue<Vector2>();
        CorrectionAimGround();
        _player.UpdateAnim("Grab");
        _player.ArmDetection.ObjectDetected = 0;

        if (_player.UseSpine == false)
        {
            //SANS SPINE
            ChoiceGrabAnim();
            _player.HandRight.sprite = _player.HandOpen;
            _player.HandLeft.sprite = _player.HandOpen;
            // Move Left Arm
            _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
            // Move Right Arm
            _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
            // Déplacement de Arm Detection pour suivre les mains
            _player.ArmDetection.GetComponent<Transform>().position = (_player.IkArmLeft.position + _player.IkArmRight.position) / 2;
            float angle = Mathf.Atan2(_player.AimDir.y, _player.AimDir.x) * Mathf.Rad2Deg;
            _player.ArmDetection.rotationOffset = angle;
            
            if (_player.AimDir == new Vector2(0, 0))
            {
                if (_player.SpriteRenderer.flipX == false)
                {
                    _player.AimDir = new Vector2(1, 0);
                    _player.LineArmLeft.sortingOrder = -1;
                    _player.LineArmRight.sortingOrder = 0;
                }
                else
                {
                    _player.AimDir = new Vector2(-1, 0);
                    _player.LineArmLeft.sortingOrder = 0;
                    _player.LineArmRight.sortingOrder = -1;
                }
            }
            else
            {
                if (_player.AimDir.x > 0)
                {
                    _player.SpriteRenderer.flipX = false;
                    _player.LineArmLeft.sortingOrder = -1;
                    _player.LineArmRight.sortingOrder = 0;
                }
                else if (_player.AimDir.x < 0)
                {
                    _player.SpriteRenderer.flipX = true;
                    _player.LineArmLeft.sortingOrder = 0;
                    _player.LineArmRight.sortingOrder = -1;
                }
            }
            ExtendArmsWithoutSpine();
            StartTimer();
        }
        else
        {
            // AVEC SPINE
            if (_player.AimDir == new Vector2(0, 0))
            {
                if (_player.SkeletonAnimation.skeleton.ScaleX == 1)
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
                    _player.SkeletonAnimation.skeleton.ScaleX = 1;
                }
                else if (_player.AimDir.x < 0)
                {
                    _player.SkeletonAnimation.skeleton.ScaleX = -1;
                }
            }
            ExtendArmsWithSpine();
        }

    }
    private void CorrectionAimGround()
    {
        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == true)
        {
            if (_player.AimDir.y <= 0)
            {
                _player.AimDir.y = 0;
            }
        }
    }

    private void ChoiceGrabAnim()
    {
        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == true)
        {
            if (_player.AimDir.y <= 0.6f)
            {
                _player.Animator.SetFloat("GrabAnimId", 1);
            }
            else
            {
                _player.Animator.SetFloat("GrabAnimId", 2);
            }
        }
        else
        {
            if (_player.AimDir.y <= -0.2f)
            {
                _player.Animator.SetFloat("GrabAnimId", 5);
            }
            else if (_player.AimDir.y >= 0.2f)
            {
                _player.Animator.SetFloat("GrabAnimId", 4);
            }
            else
            {
                _player.Animator.SetFloat("GrabAnimId", 3);
            }
        }
    }

    private void ExtendArmsWithSpine()
    {
        _player.ArmDetection.gameObject.SetActive(true);

        Vector2 grabDirection = (_player.AimDir.normalized * _player.DistanceGrab);
        if (_player.SkeletonAnimation.skeleton.ScaleX == 1)
        {
            grabDirection.x *= 1;
        }
        else
        {
            grabDirection.x *= -1;
        }
        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_player.ShoulderLeft.localPosition.x + grabDirection.x, _player.ShoulderLeft.localPosition.y + grabDirection.y);
        _player.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _player.DurationExtendGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_player.ShoulderRight.localPosition.x + grabDirection.x, _player.ShoulderRight.localPosition.y + grabDirection.y);
        _player.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _player.DurationExtendGrab);
    }
    private void ExtendArmsWithoutSpine()
    {
        _player.Arms.gameObject.SetActive(true);
        _player.ArmDetection.gameObject.SetActive(true);

        // Hands rotation
        float angle = Mathf.Atan2(_player.AimDir.x, _player.AimDir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        _player.IkArmRight.transform.rotation = _dirQ;
        _player.IkArmLeft.transform.rotation = _dirQ;

        Vector2 grabDirection = (_player.AimDir.normalized * _player.DistanceGrab);
        _player.GrabDirection = grabDirection;
        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_player.ShoulderLeft.localPosition.x + grabDirection.x, _player.ShoulderLeft.localPosition.y + grabDirection.y);
        _player.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _player.DurationExtendGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_player.ShoulderRight.localPosition.x + grabDirection.x, _player.ShoulderRight.localPosition.y + grabDirection.y);
        _player.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _player.DurationExtendGrab);
        //Debug.Log(PointDestinationArmLeft);
    }

    void StartTimer()
    {
        _player.CurrentTimerValue = _player.MaxTimeGrab;
        _player.IsTimerRunning = true;
    }
    public override void UpdateState()
    {
        if (_player.UseSpine == false)
        {
            ChoiceGrabAnim();
        }

        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == false)
        {
            // Air Control
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (moveValueH != 0)
            {
                _player.Rb.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.Rb.velocity.y);
            }
            else
            {
                _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, _player.Rb.velocity.y);
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
        
        if (_player.UseSpine == false)
        {
            // Draw Line Arm
            _player.LineArmLeft.SetPosition(0, _player.ShoulderLeft.position);
            _player.LineArmLeft.SetPosition(1, _player.IkArmLeft.position);
            _player.LineArmRight.SetPosition(0, _player.ShoulderRight.position);
            _player.LineArmRight.SetPosition(1, _player.IkArmRight.position);
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
            if (_player.UseSpine == false)
            {
                _player.Arms.gameObject.SetActive(false);
            }
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

        if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight)
        {
            if (_player.UseSpine == false)
            {
                _player.Animator.SetFloat("WallVH", 0);
                _player.Arms.gameObject.SetActive(false);
            }
            SwitchState(_factory.WallIdle());
        }
        else if (_player.IsLadder == (int)LadderIs.Horizontal)
        {
            //if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                if (_player.UseSpine == false)
                {
                    _player.Animator.SetFloat("WallVH", 1);
                    _player.Arms.gameObject.SetActive(false);
                }
                SwitchState(_factory.WallIdle());
            }
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
        
        if (_player.ArmDetection.ObjectDetected != 0)
        {
            if (_player.UseSpine == false)
            {
                _player.HandRight.sprite = _player.HandClose;
                _player.HandLeft.sprite = _player.HandClose;
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

        _player.Rb.velocity = _player.AimDir.normalized * 10;
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
            if (_player.UseSpine == false)
            {
                _player.Arms.gameObject.SetActive(false);
            }
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

        _player.Rb.velocity = _player.AimDir.normalized * 10;
    }
    private void GrabStarHandle()
    {
        //Debug.Log("Star Handle Detected");
        SwitchState(_factory.Hang());
    }
}