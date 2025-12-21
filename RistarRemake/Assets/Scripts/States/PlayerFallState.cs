using UnityEngine;
using static ArmDetection;
using static PlayerStateMachine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool isJumpBufferingTimerCanCount;

    private float speedIncreaseCurrent = 0f;
    private float currentTimerValue = 0f;

    private bool canMoveFreeFromLadder = false;

    public override void EnterState() 
    {
        //Debug.Log("ENTER FALL STATE");
        isJumpBufferingTimerCanCount = false;
        _player.JumpBufferCounter = 10;
        _player.LowJumpActivated = false;
        speedIncreaseCurrent = 0;
        currentTimerValue = 0;
        canMoveFreeFromLadder = true;

        if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.StarHandle)
        {
            Debug.Log("FALL from star handle");
            Vector2 dir = (_player.transform.position - _player.StarHandleCentre).normalized;

            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);

            _player.PlayerRigidbody.velocity = dir * _player.StarHandleCurrentImpulse;
        }

        _player.ArmDetection.ObjectGrabed = (int)ObjectGrabedIs.Nothing;

        if (_player.PreviousState is PlayerWallClimbState
            || _player.PreviousState is PlayerWallIdleState)
        {
            PushAwayFromLadder();
        }
    }
    
    private void PushAwayFromLadder()
    {
        if (_player.IsPlayerTurnToLeft == true)
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.HorizontalJumpMovementMultiplier / 2, -_player.MaxSpeedToGoToApex / 2);
            canMoveFreeFromLadder = false;
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(-_player.HorizontalJumpMovementMultiplier / 2, -_player.MaxSpeedToGoToApex / 2);
            canMoveFreeFromLadder = false;
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

        UpdateSpeedIncreaseCurrent();

        CheckSwitchStates();
    }

    private void UpdateSpeedIncreaseCurrent()
    {
        if (currentTimerValue < _player.TimeToGoToFallSpeedMax)
        {
            currentTimerValue += Time.deltaTime;
            float t = Mathf.Clamp01(currentTimerValue / _player.TimeToGoToFallSpeedMax);
            float curveValue = _player.FallSpeedCurve.Evaluate(t); // renvoie une valeur entre 0 et 1
            speedIncreaseCurrent = _player.FallSpeedMax * curveValue;
        }
        else
        {
            speedIncreaseCurrent = _player.FallSpeedMax;
        }
    }

    public override void FixedUpdateState()
    {
        // Air Control
        float moveValueH = _player.MoveH.ReadValue<float>();
        float moveValueV = _player.MoveV.ReadValue<float>();

        float velocityX = 0;
        float speedInputVertical = 0;

        if (moveValueV < 0)
        {
            speedInputVertical = _player.InputFallSpeedIncrease * Mathf.Abs(moveValueV);
        }
        else if (moveValueV > 0)
        {
            speedInputVertical = _player.InputFallSpeedDecrease * Mathf.Abs(moveValueV);
        }

        float velocityY = speedInputVertical - speedIncreaseCurrent;


        if (canMoveFreeFromLadder == false)
        {
            if (_player.IsLadder == (int)LadderIs.VerticalLeft)
            {
                canMoveFreeFromLadder = moveValueH > 0;
                velocityX = _player.PlayerRigidbody.velocity.x;
            }
            else if (_player.IsLadder == (int)LadderIs.VerticalRight)
            {
                canMoveFreeFromLadder = moveValueH < 0;
                velocityX = _player.PlayerRigidbody.velocity.x;
            }
        }
        else
        {
            velocityX = moveValueH != 0 ? moveValueH * _player.HorizontalJumpMovementMultiplier : _player.PlayerRigidbody.velocity.x;
        }

        _player.PlayerRigidbody.velocity = new Vector2(velocityX, velocityY);

        _player.PlayerDirectionVerif();
    }

    public override void CheckSwitchStates() 
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsDectected == true)
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
                //Debug.Log("COYOTE JUMP");
                SwitchState(_factory.Jump());
            }

            // Jump Buffering
            isJumpBufferingTimerCanCount = true;
            _player.JumpBufferCounter = 0;
        }

        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsDectected == true)
        {
            float moveValue = _player.MoveH.ReadValue<float>();
            if (moveValue != 0 && _player.IsGrabing == false)
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
        if (_player.IsGrabing == false)
        {
            if (_player.Grab.WasPerformedThisFrame())
            {
                //SwitchState(_factory.Grab());
                _player.StartGrab();
            }

            // Passage en state HEADBUTT ou HANG
            if (_player.GrabScript.NewStateFromGrab != null)
            {
                SwitchState(_player.GrabScript.NewStateFromGrab);
                //Debug.Log("FALL to GRAB STATE SWITCH"); 
            }
        }
    }

    public override void OnTriggerStay2D(Collider2D collider)
    {
        if (_player.TimePassedInState > 0.05f)
        {
            //Debug.Log("LADDER CHECK FALL");
            _player.LadderVerif();

            if (_player.IsLadder != (int)LadderIs.Nothing)
            {
                SwitchState(_factory.WallIdle());
            }
        }
    }
}
