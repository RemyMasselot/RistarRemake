using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrabState : PlayerBaseState
{
    public PlayerGrabState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private Vector2 _dir;

    public override void EnterState()
    {
        //Debug.Log("ENTER GRAB");
        _dir = _ctx.Aim.ReadValue<Vector2>();
        float angle = Mathf.Atan2(_dir.x, _dir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));
        _ctx.Arms.transform.rotation = _dirQ;
        _ctx.Arms.active = true;
        _ctx.StartCoroutine(GrabDetectionVerif());
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollision(Collision2D collision) {
        //if (collision.gameObject.CompareTag("Ladder"))
        //{
        //    _ctx.ArmDetection.ObjectDetected = 0;
        //    SwitchState(_factory.Idle());
        //}
        //else if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    _ctx.ArmDetection.ObjectDetected = 0;
        //    SwitchState(_factory.Idle());
        //}
    }

    private IEnumerator GrabDetectionVerif()
    {
        yield return new WaitForNextFrameUnit();
        switch (_ctx.ArmDetection.ObjectDetected)
        {
            case 1:
                GrabNothing();
                break;
            case 2:
                GrabEnemy();
                break;
            case 3:
                GrabLadder();
                break;
        }
        Debug.Log(_ctx.ArmDetection.ObjectDetected);
        _ctx.StartCoroutine(SwitchStateEndAnim());
    }

    private void GrabNothing()
    {
        
    }
    private void GrabEnemy()
    {
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Tuer l'ennemi
    }
    private void GrabLadder()
    {
        _ctx.Rb.velocity = _dir.normalized * 100;
        Debug.Log("PULSE");
        //SwitchState(_factory.Idle());
        //Enlever le contrôle du joueur
        //Déplacer le perso jusqu'au point de contact des mains
        //Passage du perso en state IDLECLIMB
    }

    private IEnumerator SwitchStateEndAnim()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("END ANIM");
        //_ctx.ArmDetection.ObjectDetected = 0;
        _ctx.Arms.active = false;
        SwitchState(_factory.Idle());
    }
}