using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeteorStrikeState : PlayerBaseState
{
    public PlayerMeteorStrikeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private Vector2 _rot;
    private Vector2 _dirPlayer;

    public override void EnterState()
    {
        Debug.Log("ENTER METEOR STRIKE");
        _ctx.UpdateAnim("MeteorStrike");
        _ctx.Rb.gravityScale = 0;
        _rot = _ctx.transform.rotation.eulerAngles;
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
        float moveValueH = _ctx.MoveH.ReadValue<float>();
        float moveValueV = _ctx.MoveV.ReadValue<float>();
        _dirPlayer = new Vector2(moveValueH, moveValueV);
        _rot = (_rot + _dirPlayer).normalized;
        _ctx.transform.Translate(_rot * _ctx.MeteorSpeed);

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
    public override void OnCollision(Collision2D collision) { }
}