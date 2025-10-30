using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMeteorStrikeState : PlayerBaseState
{
    public PlayerMeteorStrikeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private Vector2 _rot;
    private Vector2 _dirPlayer;
    private bool CanControl;

    public override void EnterState()
    {
        Debug.Log("ENTER METEOR STRIKE");
        _ctx.UpdateAnim("MeteorStrike");
        // CAMERA BEHAVIOR
        _ctx.CameraInde = true;
        CanControl = false;
        DOVirtual.DelayedCall(0.3f, () =>
        {
            CanControl = true;
        });

        Vector2 dir = _ctx.transform.position - _ctx.ShCentre;
        _ctx.Rb.velocity = dir.normalized * 10;

        _ctx.ArmDetection.ObjectDetected = 0;
        _ctx.Rb.gravityScale = 0;
        _rot = _ctx.transform.position - _ctx.ShCentre;
        _ctx.GroundDetection.gameObject.SetActive(false);
        _ctx.LadderHDetection.gameObject.SetActive(false);
        _ctx.LadderVDetectionL.gameObject.SetActive(false);
        _ctx.LadderVDetectionR.gameObject.SetActive(false);
        _ctx.Arms.gameObject.SetActive(false);
        _ctx.TriggerGoToMeteorStrike.SetActive(false);
        // Move Left Arm
        _ctx.IkArmLeft.transform.position = _ctx.DefaultPosLeft.position;
        // Move Right Arm
        _ctx.IkArmRight.transform.position = _ctx.DefaultPosRight.position;
        _ctx.transform.rotation = Quaternion.Euler(0, 0, 0);
        _ctx.SpriteRenderer.flipX = false;
        StartTimer();
    }

    void StartTimer()
    {
        _ctx.CurrentTimerValueMeteor = _ctx.MaxTimeMeteor;
        _ctx.IsTimerRunningMeteor = true;
    }

    public override void UpdateState()
    {
        if (_ctx.IsTimerRunningMeteor == true)
        {
            _ctx.CurrentTimerValueMeteor -= Time.deltaTime;
            if (_ctx.CurrentTimerValueMeteor <= 0f)
            {
                _ctx.IsTimerRunningMeteor = false;
                _ctx.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                _ctx.SpriteRenderer.flipY = false;
                _ctx.GroundDetection.gameObject.SetActive(true);
                _ctx.LadderHDetection.gameObject.SetActive(true);
                _ctx.LadderVDetectionL.gameObject.SetActive(true);
                _ctx.LadderVDetectionR.gameObject.SetActive(true);
                if (_rot.y <= 0)
                {
                    SwitchState(_factory.Fall());
                }
                else
                {
                    SwitchState(_factory.Jump());
                }
            }
        }
    }
    public override void FixedUpdateState() 
    {
        // Air Control
        if (CanControl == true)
        {
            float moveValueH = _ctx.MoveH.ReadValue<float>();
            float moveValueV = _ctx.MoveV.ReadValue<float>();
            _dirPlayer = new Vector2(moveValueH, moveValueV);
            _rot = (_rot + _dirPlayer).normalized;
        }
        _ctx.transform.Translate(_rot.normalized * _ctx.MeteorSpeed);

        // Body Rotation
        float angle = Mathf.Atan2(_rot.y, _rot.x) * Mathf.Rad2Deg;
        _ctx.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (_rot.x > 0)
        {
            _ctx.SpriteRenderer.flipY = false;
        }
        if (_rot.x < 0)
        {
            _ctx.SpriteRenderer.flipY = true;
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
    }
    public override void OnCollisionStay2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("WALL");
            if (CanControl == true)
            {
                CanControl = false;
                _ctx.MeteorSpeed -= 0.1f;
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    CanControl = true;
                    _ctx.MeteorSpeed += 0.1f;
                });
            }

            Vector2 normale = collision.GetContact(0).normal;
            _rot = Vector2.Reflect(_rot, normale);
        }
    }

}