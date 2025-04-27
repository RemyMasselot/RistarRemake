using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _ctx.UpdateAnim("Grab");
        _ctx.AimDir = _ctx.Aim.ReadValue<Vector2>();
        if (_ctx.AimDir == new Vector2(0,0))
        {
            if (_ctx.SpriteRenderer.flipX == false)
            {
                _ctx.AimDir = new Vector2(1,0);
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
                _ctx.SpriteRenderer.flipX = false;
            }
            else if (_ctx.AimDir.x < 0)
            {
                _ctx.SpriteRenderer.flipX = true;
            }
        }
        //float angle = Mathf.Atan2(_ctx.AimDir.x, _ctx.AimDir.y) * Mathf.Rad2Deg;
        //Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        //_ctx.Arms.transform.rotation = _dirQ;
        ExtendArms();
    }
    private void ExtendArms()
    {
        Vector2 grabDirection = (_ctx.AimDir.normalized * _ctx.DistanceGrab);
        // Move Left Arm
        Vector2 PointDestinationArmLeft = new Vector2(_ctx.ShoulderLeft.localPosition.x + grabDirection.x, _ctx.ShoulderLeft.localPosition.y + grabDirection.y);
        _ctx.IkArmLeft.transform.DOLocalMove(PointDestinationArmLeft, _ctx.DurationGrab);
        // Move Right Arm
        Vector2 PointDestinationArmRight = new Vector2(_ctx.ShoulderRight.localPosition.x + grabDirection.x, _ctx.ShoulderRight.localPosition.y + grabDirection.y);
        _ctx.IkArmRight.transform.DOLocalMove(PointDestinationArmRight, _ctx.DurationGrab);
    }
    public override void UpdateState()
    {
        //if (_ctx.ArmDetection.EndAnim == true && _ctx.ArmDetection.ObjectDetected == 0)
        //{
        //    // Vérification d'un sol ou non
        //    if (_ctx.GroundDetection.IsLayerDectected == false)
        //    {
        //        SwitchState(_factory.Fall());
        //    }
        //    else
        //    {
        //        // Passage en WALK ou IDLE
        //        float moveValue = _ctx.MoveH.ReadValue<float>();
        //        if (moveValue != 0)
        //        {
        //            SwitchState(_factory.Walk());
        //        }
        //        else
        //        {
        //            SwitchState(_factory.Idle());
        //        }
        //    }
        //}

        if (_ctx.Grab.WasReleasedThisFrame())
        {
            ShortenArms();
        }

        // Parce que je n'arrive pas à référencer ce script dans le script ArmDetection, ici je vérifie à chaque frame ce que les bras ont touché,
        // plutôt que de lancer la bonne fonction au moment où les bras entre en collision avec un élément dans le script ArmDetection.
        // Un raycast envoyé dans ce script et qui influe sur l'avancé des mains pourrait être une bonne solution
        GrabDetectionVerif();
    }
    private void ShortenArms()
    {
        // Move Left Arm
        _ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationGrab);
        // Move Right Arm
        _ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationGrab).OnComplete(() =>
        {
            if (_ctx.ArmDetection.ObjectDetected == 0)
            {
                // Vérification d'un sol ou non
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
            SwitchState(_factory.WallIdle());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            _ctx.Animator.SetFloat("WallVH", 1);
            SwitchState(_factory.WallIdle());
        }
        //else if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    SwitchState(_factory.Idle());
        //}
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
    }
    private void GrabEnemy()
    {
        Debug.Log("Enemy Detected");
        SwitchState(_factory.Hang());
        //_ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        //_ctx.ArmDetection.ObjectDetected = 0;
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Tuer l'ennemi
    }
    private void GrabLadder()
    {
        Debug.Log("LadderV Detected");
        _ctx.Rb.velocity = _ctx.AimDir.normalized * 10;
        _ctx.ArmDetection.ObjectDetected = 0;
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Passage du perso en state IDLECLIMB
    }
    private void GrabStarHandle()
    {
        Debug.Log("Star Handle Detected");
        SwitchState(_factory.Hang());
    }
}