using UnityEngine;
using static PlayerStateMachine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() {
        //Debug.Log("JUMP ENTER");
        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("Jump");
        //}
        _player.Leap = false;
        _player.PlayerRigidbody.gravityScale = 1;
        _player.PlayerRigidbody.velocity = new Vector2(_player.JumpForceH, _player.JumpForceV);
    }
    public override void UpdateState() 
    {
        _player.PlayerDirectionVerif();
        //if (_player.Rb.velocity.x != 0)
        //{
        //    _player.SpriteRenderer.flipX = _player.Rb.velocity.x < 0;
        //}

        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        float moveValue = _player.MoveH.ReadValue<float>();
        _player.PlayerRigidbody.velocity = new Vector2 (_player.JumpForceH * moveValue, _player.PlayerRigidbody.velocity.y);
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
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallClimb());
        }

        //if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight)
        //{
        //    //_player.Animator.SetFloat("WallVH", 0);
        //    SwitchState(_factory.WallClimb());
        //}
        //else if (_player.IsLadder == (int)LadderIs.Horizontal)
        //{
        //    //_player.Animator.SetFloat("WallVH", 1);
        //    SwitchState(_factory.WallClimb());
        //}
    }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
