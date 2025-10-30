using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("JUMP ENTER");
        _player.UpdateAnim("Jump");
        _player.Leap = false;
        _player.Rb.gravityScale = 1;
        _player.Rb.velocity = new Vector2(_player.JumpForceH, _player.JumpForceV);
    }
    public override void UpdateState() 
    {
        if (_player.Rb.velocity.x != 0)
        {
            _player.SpriteRenderer.flipX = _player.Rb.velocity.x < 0;
        }

        //if (_player.Rb.velocity.x > 0)
        //{
        //    _player.SpriteRenderer.flipX = false;
        //}
        //else if (_player.Rb.velocity.x < 0)
        //{
        //    _player.SpriteRenderer.flipX = true;
        //}

        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        float moveValue = _player.MoveH.ReadValue<float>();
        _player.Rb.velocity = new Vector2 (_player.JumpForceH * moveValue, _player.Rb.velocity.y);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
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
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            _player.Animator.SetFloat("WallVH", 0);
            SwitchState(_factory.WallClimb());
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            _player.Animator.SetFloat("WallVH", 1);
            SwitchState(_factory.WallClimb());
        }
    }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
