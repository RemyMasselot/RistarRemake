using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private Vector2 _dir;

    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _ctx.Animator.SetBool("Grab", true);
        _dir = _ctx.Aim.ReadValue<Vector2>();
        float angle = Mathf.Atan2(_dir.x, _dir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        _ctx.Arms.transform.rotation = _dirQ;
        //_ctx.Arms.active = true;
        //_ctx.StartCoroutine(GrabDetectionVerif());
    }
    public override void UpdateState()
    {
        //CheckSwitchStates();
        if (_ctx.ArmDetection.EndAnim == true && _ctx.ArmDetection.ObjectDetected == 0)
        {
            _ctx.Animator.SetBool("Grab", false);
            SwitchState(_factory.Idle());
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
    public override void OnCollision(Collision2D collision) {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            //_ctx.ArmDetection.ObjectDetected = 0;
            _ctx.Animator.SetBool("Grab", false);
            _ctx.Animator.SetFloat("WallVH", 0);
            SwitchState(_factory.WallIdle());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            //_ctx.ArmDetection.ObjectDetected = 0;
            _ctx.Animator.SetBool("Grab", false);
            _ctx.Animator.SetFloat("WallVH", 1);
            SwitchState(_factory.WallIdle());
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //_ctx.ArmDetection.ObjectDetected = 0;
            _ctx.Animator.SetBool("Grab", false);
            SwitchState(_factory.Idle());
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
        }
        //Debug.Log(_ctx.ArmDetection.ObjectDetected);
    }

    private void GrabOther()
    {
        Debug.Log("Other Detected");
        _ctx.Animator.SetBool("Grab", false);
        _ctx.ArmDetection.ObjectDetected = 0;
        SwitchState(_factory.Idle());
    }
    private void GrabEnemy()
    {
        Debug.Log("Enemy Detected");
        _ctx.Rb.velocity = _dir.normalized * 50;
        _ctx.ArmDetection.ObjectDetected = 0;
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Tuer l'ennemi
    }
    private void GrabLadder()
    {
        Debug.Log("LadderV Detected");
        _ctx.Rb.velocity = _dir.normalized * 50;
        _ctx.ArmDetection.ObjectDetected = 0;
        //SwitchState(_factory.Idle());
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Passage du perso en state IDLECLIMB
    }

    //public void SwitchState()
    //{
    //    //yield return new WaitForSeconds(1);
    //    //Debug.Log("END ANIM");
    //    //_ctx.ArmDetection.ObjectDetected = 0;
    //    //_ctx.Arms.active = false;
    //    SwitchState(_factory.Idle());
    //}
}