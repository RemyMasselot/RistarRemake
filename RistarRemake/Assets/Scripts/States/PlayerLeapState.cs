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
        _player.UpdateAnim("Jump");
        _player.Leap = false;
        _player.Rb.gravityScale = 1;
        PosY = _player.transform.position.y;
        newOrientation = false;
        _player.Rb.velocity = new Vector2(0, _player.LeapForceV);
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
            if (_player.transform.position.y >= PosY + 1.5f)
            {
                //Debug.Log("efz");
                newOrientation = true;
                if (_player.SpriteRenderer.flipX == false)
                {
                    _player.Rb.velocity = new Vector2(_player.LeapForceH, _player.LeapForceV/2);
                }
                else
                { 
                    _player.Rb.velocity = new Vector2(-_player.LeapForceH, _player.LeapForceV/2);
                }
                // Air Control
                float moveValueH = _player.MoveH.ReadValue<float>();
                float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.MoveDownFallValue, _player.MoveDownFallValueMax);
                if (moveValueH != 0)
                {
                    _player.Rb.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.Rb.velocity.y + moveValueV);
                }
                else
                {
                    _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, _player.Rb.velocity.y + moveValueV);
                }
            }
        }
        // Rotation visuelle -- SANS SPINE
        if (_player.Rb.velocity.x > 0)
        {
            _player.SpriteRenderer.flipX = false;
        }
        if (_player.Rb.velocity.x < 0)
        {
            _player.SpriteRenderer.flipX = true;
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates()
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }

        // Passage en state FALL
        if (_player.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
