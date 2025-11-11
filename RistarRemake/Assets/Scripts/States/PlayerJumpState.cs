using UnityEngine;
using static PlayerStateMachine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float jumpOriginY;
    private float TimePassedApex;
    private bool canCountTimeApex;

    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");

        _player.PlayerRigidbody.gravityScale = 0;
        _player.CoyoteCounter = 0;
        jumpOriginY = _player.transform.position.y;

        canCountTimeApex = false;
        TimePassedApex = 0;

        if (_player.ArmDetection.ObjectDetected == 4)
        {
            //Debug.Log("JUMP from star handle");
            Vector2 dir = _player.transform.position - _player.StarHandleCentre;
            
            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);
            
            _player.PlayerRigidbody.velocity = dir.normalized * _player.StarHandleCurrentImpulse;
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.VerticalMovementSpeed);
        }
        _player.ArmDetection.ObjectDetected = 0;
    }

    public override void UpdateState() 
    {
        if (canCountTimeApex)
        {
            TimePassedApex += Time.deltaTime;
        }

        _player.PlayerDirectionVerif();

        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        float velocityX = 0;
        float velocityY = 0;
        
        float moveValue = _player.MoveH.ReadValue<float>();
        velocityX = moveValue != 0 ? moveValue * _player.HorizontalMovementMultiplier : _player.PlayerRigidbody.velocity.x;

        float distanceFromOriginY = Mathf.Abs(_player.transform.position.y - jumpOriginY);

        if (distanceFromOriginY >= _player.MaxVerticalDistance)
        {
            velocityY = 0;
        }
        else
        {
            velocityY = _player.VerticalMovementSpeed;
            canCountTimeApex = true;
        }

        _player.PlayerRigidbody.velocity = new Vector2(velocityX, velocityY);
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

        // Passage en state FALL
        if (TimePassedApex >= _player.MaxTimeApex)
        {
            SwitchState(_factory.Fall());
        }
        else if (TimePassedApex >= 0.05f)
        {
            if (_player.Jump.WasReleasedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallClimb());
        }
    }
}
