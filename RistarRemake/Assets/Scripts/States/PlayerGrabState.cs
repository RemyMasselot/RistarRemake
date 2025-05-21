using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEngine.UI.Image;
using UnityEngine.U2D;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _ctx.AimDir = _ctx.Aim.ReadValue<Vector2>();
        //Debug.Log(_ctx.AimDir);
        //if (_ctx.GamepadUsed == false)
        //{
        //    //Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //    _ctx.AimDir = new Vector2(_ctx.AimDir.x - _ctx.transform.position.x, _ctx.AimDir.y - _ctx.transform.position.y);
        //    Debug.Log(_ctx.gameObject.name);
        //}

        ChoiceGrabAnim();

        _ctx.UpdateAnim("Grab");
        
        if (_ctx.UseSpine == false)
        {
            //SANS SPINE
            if (_ctx.AimDir == new Vector2(0, 0))
            {
                if (_ctx.SpriteRenderer.flipX == false)
                {
                    _ctx.AimDir = new Vector2(1, 0);
                    _ctx.LineArmLeft.sortingOrder = -1;
                    _ctx.LineArmRight.sortingOrder = 0;
                }
                else
                {
                    _ctx.AimDir = new Vector2(-1, 0);
                    _ctx.LineArmLeft.sortingOrder = 0;
                    _ctx.LineArmRight.sortingOrder = -1;
                }
            }
            else
            {
                if (_ctx.AimDir.x > 0)
                {
                    _ctx.SpriteRenderer.flipX = false;
                    _ctx.LineArmLeft.sortingOrder = -1;
                    _ctx.LineArmRight.sortingOrder = 0;
                }
                else if (_ctx.AimDir.x < 0)
                {
                    _ctx.SpriteRenderer.flipX = true;
                    _ctx.LineArmLeft.sortingOrder = 0;
                    _ctx.LineArmRight.sortingOrder = -1;
                }
            }
            ExtendArmsWithoutSpine();
            StartTimer();
        }
        else
        {
            // AVEC SPINE
            if (_ctx.AimDir == new Vector2(0, 0))
            {
                if (_ctx.SkeletonAnimation.skeleton.FlipX == false)
                {
                    _ctx.AimDir = new Vector2(1, 0);
                }
                else
                {
                    _ctx.AimDir = new Vector2(-1, 0);
                }
            }
            else
            {
                if (_ctx.AimDir.x > 0)
                {
                    _ctx.SkeletonAnimation.skeleton.FlipX = false;
                }
                else if (_ctx.AimDir.x < 0)
                {
                    _ctx.SkeletonAnimation.skeleton.FlipX = true;
                }
            }
            ExtendArmsWithSpine();
        }

    }

    private void ChoiceGrabAnim()
    {
        // V�rification d'un sol ou non
        RaycastHit2D hit = Physics2D.Raycast(_ctx.transform.position, _ctx.AimDir.normalized * _ctx.DistanceGrab); // 10f = distance max
        
        if (hit.collider.name == "Ground")
        {
            if (_ctx.AimDir.y <= 0.6f)
            {
                _ctx.Animator.SetFloat("GrabAnimId", 1);
                if (_ctx.AimDir.y <= 0)
                    _ctx.AimDir.y = 0;
            }
            else
            {
                _ctx.Animator.SetFloat("GrabAnimId", 2);
            }
        }
        else
        {
            if (_ctx.AimDir.y <= -0.2f)
            {
                _ctx.Animator.SetFloat("GrabAnimId", 5);
            }
            else if (_ctx.AimDir.y >= 0.2f)
            {
                _ctx.Animator.SetFloat("GrabAnimId", 4);
            }
            else
            {
                _ctx.Animator.SetFloat("GrabAnimId", 3);
            }
        }
    }

    private void ExtendArmsWithSpine()
    {
        _ctx.ArmDetection.gameObject.SetActive(true);

        Vector2 grabDirection = (_ctx.AimDir.normalized * _ctx.DistanceGrab);
        if (_ctx.SkeletonAnimation.skeleton.FlipX == false)
        {
            grabDirection.x *= 1;
        }
        else
        {
            grabDirection.x *= -1;
        }
        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_ctx.ShoulderLeft.localPosition.x + grabDirection.x, _ctx.ShoulderLeft.localPosition.y + grabDirection.y);
        _ctx.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _ctx.DurationExtendGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_ctx.ShoulderRight.localPosition.x + grabDirection.x, _ctx.ShoulderRight.localPosition.y + grabDirection.y);
        _ctx.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _ctx.DurationExtendGrab);
    }
    private void ExtendArmsWithoutSpine()
    {
        _ctx.Arms.gameObject.SetActive(true);
        _ctx.ArmDetection.gameObject.SetActive(true);

        // Hands rotation
        float angle = Mathf.Atan2(_ctx.AimDir.x, _ctx.AimDir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        _ctx.IkArmRight.transform.rotation = _dirQ;
        _ctx.IkArmLeft.transform.rotation = _dirQ;

        Vector2 grabDirection = (_ctx.AimDir.normalized * _ctx.DistanceGrab);

        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_ctx.ShoulderLeft.localPosition.x + grabDirection.x, _ctx.ShoulderLeft.localPosition.y + grabDirection.y);
        _ctx.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _ctx.DurationExtendGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_ctx.ShoulderRight.localPosition.x + grabDirection.x, _ctx.ShoulderRight.localPosition.y + grabDirection.y);
        _ctx.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _ctx.DurationExtendGrab);
        //Debug.Log(PointDestinationArmLeft);
    }

    void StartTimer()
    {
        _ctx.CurrentTimerValue = _ctx.MaxTimeGrab;
        _ctx.IsTimerRunning = true;
    }
    public override void UpdateState()
    {
        ChoiceGrabAnim();

        if (_ctx.UseSpine == false)
        {
            // Draw Line Arm
            _ctx.LineArmLeft.SetPosition(0, _ctx.ShoulderLeft.position);
            _ctx.LineArmLeft.SetPosition(1, _ctx.IkArmLeft.position);
            _ctx.LineArmRight.SetPosition(0, _ctx.ShoulderRight.position);
            _ctx.LineArmRight.SetPosition(1, _ctx.IkArmRight.position);
        }

        if (_ctx.IsTimerRunning == true)
        {
            _ctx.CurrentTimerValue -= Time.deltaTime;
            if (_ctx.CurrentTimerValue <= 0f)
            {
                _ctx.IsTimerRunning = false;
                ShortenArms();
            }
        }

        // D�placement de Arm Detection pour suivre les mains
        _ctx.ArmDetection.GetComponent<Transform>().position = (_ctx.IkArmLeft.position + _ctx.IkArmRight.position) / 2;
        float angle = Mathf.Atan2(_ctx.AimDir.y, _ctx.AimDir.x) * Mathf.Rad2Deg;
        _ctx.ArmDetection.rotationOffset = angle;

        if (_ctx.Grab.WasReleasedThisFrame())
        {
            _ctx.IsTimerRunning = false;
            ShortenArms();
        }

        // Parce que je n'arrive pas � r�f�rencer ce script dans le script ArmDetection, ici je v�rifie � chaque frame ce que les bras ont touch�,
        // plut�t que de lancer la bonne fonction au moment o� les bras entre en collision avec un �l�ment dans le script ArmDetection.
        // Un raycast envoy� dans ce script et qui influe sur l'avanc� des mains pourrait �tre une bonne solution
        GrabDetectionVerif();

        if (_ctx.ArmDetection.ObjectDetected == 3)
        {
            _ctx.IkArmLeft.position = _ctx.ArmDetection.SnapPosHandL;
            _ctx.IkArmRight.position = _ctx.ArmDetection.SnapPosHandR;
        }
    }
    public void ShortenArms()
    {
        _ctx.ArmDetection.gameObject.SetActive(false);

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationExtendGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationExtendGrab).OnComplete(() =>
        {
            if (_ctx.UseSpine == false)
            {
                _ctx.Arms.gameObject.SetActive(false);
            }
            if (_ctx.ArmDetection.ObjectDetected == 0)
            {
                // V�rification d'un sol ou non
                if (_ctx.GroundDetection.IsLayerDectected == false)
                {
                    SwitchState(_factory.Fall());
                }
                else
                {
                    // Passage en WALK ou IDLE
                    float moveValue = _ctx.MoveH.ReadValue<float>();
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
    public override void OnCollision(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            _ctx.Animator.SetFloat("WallVH", 0);
            if (_ctx.UseSpine == false)
            {
                _ctx.Arms.gameObject.SetActive(false);
            }
            SwitchState(_factory.WallIdle());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            _ctx.Animator.SetFloat("WallVH", 1);
            if (_ctx.UseSpine == false)
            {
                _ctx.Arms.gameObject.SetActive(false);
            }
            SwitchState(_factory.WallIdle());
        }
    }

    public void GrabDetectionVerif()
    {
        switch (_ctx.ArmDetection.ObjectDetected)
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
        }
        //Debug.Log(_ctx.ArmDetection.ObjectDetected);
    }

    private void GrabOther()
    {
        Debug.Log("Other Detected");
        _ctx.ArmDetection.ObjectDetected = 0;
        if (_ctx.UseSpine == false)
        {
            _ctx.Arms.gameObject.SetActive(false);
        }
        _ctx.ArmDetection.gameObject.SetActive(false);

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationExtendGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationExtendGrab);
        _ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        _ctx.ArmDetection.ObjectDetected = 0;
    }
    private void GrabEnemy()
    {
        Debug.Log("Enemy Detected");
        SwitchState(_factory.Hang());
        //_ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        //_ctx.ArmDetection.ObjectDetected = 0;
        //Enlever le contr�le du joueur
        //D�placer le perso jusqu'au point de contact des mains
        //Tuer l'ennemi
    }
    private void GrabLadder()
    {
        Debug.Log("Ladder Detected");
        _ctx.ArmDetection.gameObject.SetActive(false);

        // Move Left Arm
        _ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationExtendGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationExtendGrab);
        _ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        _ctx.ArmDetection.ObjectDetected = 0;
        //Enlever le contr�le du joueur
        //D�placer le perso jusqu'au point de contact des mains
        //Passage du perso en state IDLECLIMB
    }
    private void GrabStarHandle()
    {
        Debug.Log("Star Handle Detected");
        SwitchState(_factory.Hang());
    }
}