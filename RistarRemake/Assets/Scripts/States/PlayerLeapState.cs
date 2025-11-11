using UnityEngine;

public class PlayerLeapState : PlayerBaseState
{
    public PlayerLeapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float startLeapPositionY;
    private bool newOrientation;

    public override void EnterState()
    {
        //Debug.Log("JUMP ENTER");

        _player.PlayerRigidbody.gravityScale = 1;
        _player.PlayerRigidbody.velocity = new Vector2(0, _player.LeapForceV);
        
        startLeapPositionY = _player.transform.position.y;
        newOrientation = false;
    }

    public override void UpdateState()
    {
        _player.PlayerDirectionVerif();

        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        if (_player.transform.position.y >= startLeapPositionY + 1.5f)
        {
            if (newOrientation == false)
            {
                newOrientation = true;
                if (_player.IsPlayerTurnToLeft == false)
                {
                    _player.PlayerRigidbody.velocity = new Vector2(_player.LeapForceH, _player.LeapForceV / 2);
                }
                else
                {
                    _player.PlayerRigidbody.velocity = new Vector2(-_player.LeapForceH, _player.LeapForceV / 2);
                }
            }

            AirControl();
        }
    }

    private void AirControl()
    {
        float moveValueH = _player.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.MoveDownFallValueMin, _player.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(moveValueH * _player.HorizontalMovementMultiplier, _player.PlayerRigidbody.velocity.y + moveValueV);
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y + moveValueV);
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
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
