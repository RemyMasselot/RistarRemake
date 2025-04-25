using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    //private Vector2 _dir;

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
        float angle = Mathf.Atan2(_ctx.AimDir.x, _ctx.AimDir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        _ctx.Arms.transform.rotation = _dirQ;
    }
    public override void UpdateState()
    {
        if (_ctx.ArmDetection.EndAnim == true && _ctx.ArmDetection.ObjectDetected == 0)
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

        // Parce que je n'arrive pas à référencer ce script dans le script ArmDetection, ici je vérifie à chaque frame ce que les bras ont touché,
        // plutôt que de lancer la bonne fonction au moment où les bras entre en collision avec un élément dans le script ArmDetection.
        // Un raycast envoyé dans ce script et qui influe sur l'avancé des mains pourrait être une bonne solution
        GrabDetectionVerif();
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