using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeapState : PlayerBaseState
{
    public PlayerLeapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float PosY;
    private bool newOrientation;

    public override void EnterState()
    {
        //Debug.Log("JUMP ENTER");
        _ctx.UpdateAnim("Jump");
        _ctx.Leap = false;
        _ctx.Rb.gravityScale = 1;
        PosY = _ctx.transform.position.y;
        newOrientation = false;
        _ctx.Rb.velocity = new Vector2(0, _ctx.LeapForceV);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        if (newOrientation == true)
        {
            //float moveValue = _ctx.MoveH.ReadValue<float>();
            //_ctx.Rb.velocity = new Vector2(_ctx.LeapForceH * moveValue, _ctx.Rb.velocity.y);
        }
        else
        {
            if (_ctx.transform.position.y >= PosY + 1.5f)
            {
                Debug.Log("efz");
                newOrientation = true;
                if (_ctx.SpriteRenderer.flipX == false)
                {
                    _ctx.Rb.velocity = new Vector2(_ctx.LeapForceH, _ctx.LeapForceV/2);
                }
                else
                { 
                    _ctx.Rb.velocity = new Vector2(-_ctx.LeapForceH, _ctx.LeapForceV/2);
                }
            }
        }
        // Air Control
        float moveValueH = _ctx.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_ctx.MoveV.ReadValue<float>(), _ctx.MoveDownFallValue, _ctx.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _ctx.Rb.velocity = new Vector2(moveValueH * _ctx.JumpForceH, _ctx.Rb.velocity.y + moveValueV);
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.Rb.velocity.y + moveValueV);
        }
        // Rotation visuelle -- SANS SPINE
        if (_ctx.Rb.velocity.x > 0)
        {
            _ctx.SpriteRenderer.flipX = false;
        }
        if (_ctx.Rb.velocity.x < 0)
        {
            _ctx.SpriteRenderer.flipX = true;
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates()
    {
        // Enter DAMAGE STATE
        if (_ctx.Invincinbility.IsInvincible == false)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }

        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
