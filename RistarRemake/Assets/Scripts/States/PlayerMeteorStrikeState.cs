using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMeteorStrikeState : PlayerBaseState
{
    public PlayerMeteorStrikeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private Vector2 _dirPlayer;
    private bool CanControl;

    public override void EnterState()
    {
        Debug.Log("ENTER METEOR STRIKE");
        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("MeteorStrike");
        //}
        // CAMERA BEHAVIOR
        _player.CameraInde = true;
        CanControl = false;
        DOVirtual.DelayedCall(0.3f, () =>
        {
            CanControl = true;
        });

        Vector2 dir = _player.transform.position - _player.StarHandleCentre;
        _player.PlayerRigidbody.velocity = dir.normalized * 10;

        _player.ArmDetection.ObjectDetected = 0;
        _player.PlayerRigidbody.gravityScale = 0;
        _player.MeteorStrikeDirection = _player.transform.position - _player.StarHandleCentre;
        _player.GroundDetection.gameObject.SetActive(false);
        _player.LadderHDetection.gameObject.SetActive(false);
        _player.LadderVDetectionL.gameObject.SetActive(false);
        _player.LadderVDetectionR.gameObject.SetActive(false);
        _player.Arms.gameObject.SetActive(false);
        _player.TriggerGoToMeteorStrike.SetActive(false);
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
        _player.transform.rotation = Quaternion.Euler(0, 0, 0);
        _player.IsPlayerTurnToLeft = false;
        StartTimer();
    }

    void StartTimer()
    {
        _player.CurrentTimerValueMeteor = _player.MaxTimeMeteor;
        _player.IsTimerRunningMeteor = true;
    }

    public override void UpdateState()
    {
        if (_player.IsTimerRunningMeteor == true)
        {
            _player.CurrentTimerValueMeteor -= Time.deltaTime;
            if (_player.CurrentTimerValueMeteor <= 0f)
            {
                _player.IsTimerRunningMeteor = false;
                //_player.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                //_player.SpriteRenderer.flipY = false;
                _player.GroundDetection.gameObject.SetActive(true);
                _player.LadderHDetection.gameObject.SetActive(true);
                _player.LadderVDetectionL.gameObject.SetActive(true);
                _player.LadderVDetectionR.gameObject.SetActive(true);
                if (_player.MeteorStrikeDirection.y <= 0)
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
            float moveValueH = _player.MoveH.ReadValue<float>();
            float moveValueV = _player.MoveV.ReadValue<float>();
            _dirPlayer = new Vector2(moveValueH, moveValueV);

            _player.MeteorStrikeDirection = (_player.MeteorStrikeDirection + _dirPlayer).normalized;
        }

        _player.transform.Translate(_player.MeteorStrikeDirection.normalized * _player.MeteorSpeed);


        //// Body Rotation
        //float angle = Mathf.Atan2(_player.MeteorStrikeThrowDirection.y, _player.MeteorStrikeThrowDirection.x) * Mathf.Rad2Deg;
        //_player.SpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

        //if (_player.MeteorStrikeThrowDirection.x > 0)
        //{
        //    _player.SpriteRenderer.flipY = false;
        //}
        //if (_player.MeteorStrikeThrowDirection.x < 0)
        //{
        //    _player.SpriteRenderer.flipY = true;
        //}
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
                _player.MeteorSpeed -= 0.1f;
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    CanControl = true;
                    _player.MeteorSpeed += 0.1f;
                });
            }

            Vector2 normale = collision.GetContact(0).normal;
            _player.MeteorStrikeDirection = Vector2.Reflect(_player.MeteorStrikeDirection, normale);
        }
    }

}