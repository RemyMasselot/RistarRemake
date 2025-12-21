using UnityEngine;
using static PlayerStateMachine;

public class PlayerWallJumpState : PlayerBaseState
{
    public PlayerWallJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    private float wallJumpOriginY;

    public override void EnterState() 
    {
        Debug.Log("WALL JUMP ENTER");

        //_player.PlayerRigidbody.gravityScale = 1;
        //_player.PlayerRigidbody.velocity = new Vector2(_player.HorizontalMovementMultiplier, _player.JumpSpeedMax);

        wallJumpOriginY = _player.transform.position.y;
    }

    public override void UpdateState() 
    {
        _player.PlayerDirectionVerif();

        CheckSwitchStates();
    }

    public override void FixedUpdateState() 
    {
        float moveValueX = _player.MoveH.ReadValue<float>();
        //_player.PlayerRigidbody.velocity = new Vector2 (_player.HorizontalMovementMultiplier * moveValue, _player.PlayerRigidbody.velocity.y);

        float velocityY = 2;

        _player.PlayerRigidbody.velocity = new Vector2(moveValueX * 12, velocityY);
    }

    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state GRAB
        if (_player.IsGrabing == false)
        {
            if (_player.Grab.WasPerformedThisFrame())
            {
                //SwitchState(_factory.Grab());
                _player.StartGrab();
            }
        }

        // Passage en state FALL
        if (_player.Jump.WasReleasedThisFrame())
        {
            SwitchState(_factory.Fall());
        }
        else if (_player.transform.position.y > wallJumpOriginY + 3)
        {
            SwitchState(_factory.Fall());
        }
    }

    public override void OnTriggerStay2D(Collider2D collider) 
    {
        _player.LadderVerif();

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallIdle());
        }
    }
}
