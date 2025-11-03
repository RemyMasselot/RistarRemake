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
        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("Jump");
        //}
        _player.Leap = false;
        _player.PlayerRigidbody.gravityScale = 1;
        PosY = _player.transform.position.y;
        newOrientation = false;
        _player.PlayerRigidbody.velocity = new Vector2(0, _player.LeapForceV);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        if (newOrientation == false)
        {
            if (_player.transform.position.y >= PosY + 1.5f)
            {
                //Debug.Log("efz");
                newOrientation = true;
                if (_player.IsPlayerTurnToLeft == false)
                {
                    _player.PlayerRigidbody.velocity = new Vector2(_player.LeapForceH, _player.LeapForceV/2);
                }
                else
                { 
                    _player.PlayerRigidbody.velocity = new Vector2(-_player.LeapForceH, _player.LeapForceV/2);
                }
                // Air Control
                float moveValueH = _player.MoveH.ReadValue<float>();
                float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.MoveDownFallValue, _player.MoveDownFallValueMax);
                if (moveValueH != 0)
                {
                    _player.PlayerRigidbody.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.PlayerRigidbody.velocity.y + moveValueV);
                }
                else
                {
                    _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y + moveValueV);
                }
            }
        }
        _player.PlayerDirectionVerif();
        // Rotation visuelle -- SANS SPINE
        //if (_player.Rb.velocity.x > 0)
        //{
        //    _player.SpriteRenderer.flipX = false;
        //}
        //if (_player.Rb.velocity.x < 0)
        //{
        //    _player.SpriteRenderer.flipX = true;
        //}
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
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
