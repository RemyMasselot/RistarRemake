using UnityEngine;
using static PlayerStateMachine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool isJumpBufferingTimerCanCount;

    public override void EnterState() 
    {
        _player.ArmDetection.ObjectDetected = 0;
        _player.PlayerRigidbody.gravityScale = 2;

        isJumpBufferingTimerCanCount = false;
        _player.JumpBufferCounter = 10;

        if (_player.ArmDetection.ObjectDetected == 4)
        {
            Debug.Log("FALL from star handle");
            Vector2 dir = (_player.transform.position - _player.StarHandleCentre).normalized;

            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);

            _player.PlayerRigidbody.velocity = dir * _player.StarHandleCurrentImpulse;
        }

        PushAwayFromLadder();
    }
    
    private void PushAwayFromLadder()
    {
        if (_player.LadderVDetectionL.IsLadderVDectectedL == true)
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.JumpForceH / 2, -_player.JumpForceV / 2);
        }
        else if (_player.LadderVDetectionR.IsLadderVDectectedR == true)
        {
            _player.PlayerRigidbody.velocity = new Vector2(-_player.JumpForceH / 2, -_player.JumpForceV / 2);
        }
    }   

    public override void ExitState() { }

    public override void UpdateState() 
    {
        _player.CountTimePassedInState();

        // Jump Buffering Timer
        if (isJumpBufferingTimerCanCount == true)
        {
            _player.JumpBufferCounter += Time.deltaTime;
        }

        CheckSwitchStates();
    }

    public override void FixedUpdateState()
    {
        // Air Control
        float moveValueH = _player.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.MoveDownFallValueMin, _player.MoveDownFallValueMax);
        if (moveValueH != 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(moveValueH * _player.JumpForceH, _player.PlayerRigidbody.velocity.y + moveValueV);
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y + moveValueV);
        }

        _player.PlayerDirectionVerif();
    }

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

        // Exécution du saut
        if (_player.Jump.WasPerformedThisFrame())
        {
            // Coyote Time 
            if (_player.CoyoteCounter > 0)
            {
                SwitchState(_factory.Jump());
            }

            // Jump Buffering
            isJumpBufferingTimerCanCount = true;
            _player.JumpBufferCounter = 0;
        }

        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == true)
        {
            float moveValue = _player.MoveH.ReadValue<float>();
            if (moveValue != 0)
            {
                // Passage en state WALK
                SwitchState(_factory.Walk());
            }
            else
            {
                // Passage en state IDLE
                SwitchState(_factory.Idle());
            }
        }
        else
        {
            _player.CoyoteCounter -= Time.deltaTime;
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollisionStay2D(Collision2D collision)
    {
        if (_player.TimePassedInState > 0.2f)
        {
            Debug.Log("LADDER CHECK FALL");
            _player.LadderVerif(collision);

            if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight)
            {
                SwitchState(_factory.WallClimb());
            }
            else if (_player.IsLadder == (int)LadderIs.Horizontal)
            {
                SwitchState(_factory.WallClimb());
            }
        }
    }
}
