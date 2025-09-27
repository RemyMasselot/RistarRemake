using DG.Tweening;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool hasEnded;

    public override void EnterState()
    {
        Debug.Log("ENTER SPIN");
        _ctx.UpdateAnim("Spin");

        hasEnded = false;
        if (_ctx.UseSpine == false)
        {
            _ctx.Arms.gameObject.SetActive(false);
            _ctx.ArmDetection.gameObject.SetActive(false);
            // Move Left Arm
            _ctx.IkArmLeft.transform.position = _ctx.DefaultPosLeft.position;
            // Move Right Arm
            _ctx.IkArmRight.transform.position = _ctx.DefaultPosRight.position;
        }
        _ctx.Rb.gravityScale = 1;

        if (_ctx.ArmDetection.ObjectDetected == 5)
        {
            if (_ctx.GrabDirection.x < 0)
            {
                _ctx.Rb.velocity = new Vector2(1, 1) * 4;
            }
            if (_ctx.GrabDirection.x > 0)
            {
                _ctx.Rb.velocity = new Vector2(-1, 1) * 4;
            }
            if (_ctx.LadderHDetection.IsLadderHDectected == true)
            {
                _ctx.Rb.velocity = new Vector2(0, -1);
            }
        }

        if (_ctx.ArmDetection.ObjectDetected == 2)
        {
            //Debug.Log("greg");
            // CAMERA BEHAVIOR
            _ctx.MainCameraBehavior.CameraImpacted = true;
            _ctx.MainCameraBehavior.CameraInde = true;
            _ctx.MainCameraBehavior.Camera.DOShakePosition(0.2f, _ctx.MainCameraBehavior.CamShakeHeabbutt, 5).OnComplete(()=>
                {
                    _ctx.MainCameraBehavior.CameraImpacted = false;
                });
            _ctx.Rb.velocity = new Vector2(0, 1) * 10;
        }
    }
    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = _ctx.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Spin") && stateInfo.normalizedTime >= 1f && !hasEnded)
        {
            hasEnded = true;
            OnAnimationEnd();
        }
    }

    void OnAnimationEnd()
    {
        //Debug.Log("Animation terminée !");
        SwitchState(_factory.Fall());
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}