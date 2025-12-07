using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState() 
    {
        //Debug.Log("ENTER WALK");
        _player.CoyoteCounter = _player.CoyoteTime;
    }
    public override void UpdateState() 
    { 
        _player.CountTimePassedInState();
        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        movement();
        _player.PlayerDirectionVerif();
    }

    private void movement()
    {
        if (_player.IsGrabing == false)
        {
            float moveValue = _player.MoveH.ReadValue<float>();
            _player.PlayerRigidbody.velocity = new Vector2(moveValue * _player.WalkSpeed * Time.deltaTime, 0);
        }
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

        // Passage en state FALL
        if (_player.GroundDetection.IsDectected == false)
        {
            SwitchState(_factory.Fall());
        }
        
        // Passage en state IDLE
        float moveValue = _player.MoveH.ReadValue<float>();
        if (moveValue == 0)
        {
            SwitchState(_factory.Idle());
        }

        // Passage en state JUMP
        if (_player.Jump.WasPerformedThisFrame())
        {
            SwitchState(_factory.Jump());
        }
        else if (_player.JumpBufferCounter <= _player.JumpBufferTime)
        {
            _player.LowJumpActivated = true;
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_player.IsGrabing == false)
        {
            if (_player.Grab.WasPerformedThisFrame())
            {
                SwitchState(_factory.Idle());
                _player.StartGrab();
            }

            // Passage en state HEADBUTT ou HANG
            if (_player.GrabScript.NewStateFromGrab != null)
            {
                SwitchState(_player.GrabScript.NewStateFromGrab);
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
