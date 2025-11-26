using UnityEngine;
using static ArmDetection;
using static PlayerStateMachine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool canCountTimeApex;
    private float TimePassedAtApex;

    private float jumpOriginY;
    private float distanceFromOriginY;

    private float timerUpdatePositionY = 0f;
    private float currentPositionY;

    private bool canModifyVelocityXLeft;
    private bool canModifyVelocityXRight;

    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");

        _player.CoyoteCounter = 0;
        jumpOriginY = _player.transform.position.y;
        currentPositionY = jumpOriginY;

        canCountTimeApex = false;
        TimePassedAtApex = 0;

        canModifyVelocityXLeft = true;
        canModifyVelocityXRight = true;

        if (_player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.StarHandle)
        {
            //Debug.Log("JUMP from star handle");
            Vector2 dir = _player.transform.position - _player.StarHandleCentre;
            
            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);
            
            _player.PlayerRigidbody.velocity = dir.normalized * _player.StarHandleCurrentImpulse;
        }
        _player.ArmDetection.ObjectDetected = (int)ObjectDetectedIs.Nothing;
    }

    public override void UpdateState() 
    {
        _player.CountTimePassedInState();

        if (canCountTimeApex)
        {
            TimePassedAtApex += Time.deltaTime;
        }

        _player.PlayerDirectionVerif();

        UpdatePositionY();

        CheckSwitchStates();
    }

    private void UpdatePositionY()
    {
        if (_player.CornerCorrection.HitLeft == false && _player.CornerCorrection.HitRight == false)
        {
            if (timerUpdatePositionY < _player.TimeToGoToApex)
            {
                timerUpdatePositionY += Time.deltaTime;
                float t = Mathf.Clamp01(timerUpdatePositionY / _player.TimeToGoToApex);
                float curveValue = _player.JumpSpeedCurve.Evaluate(t); // renvoie une valeur entre 0 et 1
                currentPositionY = jumpOriginY + _player.VerticalJumpDistanceHigh * curveValue;
            }
            else
            {
                currentPositionY = jumpOriginY + _player.VerticalJumpDistanceHigh;
            }
        }
        
        distanceFromOriginY = Mathf.Abs(_player.transform.position.y - jumpOriginY);

        if (distanceFromOriginY >= _player.VerticalJumpDistanceHigh)
        {
            canCountTimeApex = true;
        }

        _player.transform.position = new Vector2(_player.transform.position.x, currentPositionY);
    }

    public override void FixedUpdateState() 
    {
        ModifyVelocityX();
    }

    private void ModifyVelocityX()
    {
        float velocityX = 0;
        float moveValueH = _player.MoveH.ReadValue<float>();

        if (_player.CornerCorrection.HitLeft && !_player.CornerCorrection.HitRight && moveValueH <= 0.5f)
        {
            _player.transform.position += new Vector3(_player.CornerCorrection.CornerDistance, 0f, 0f);
            canModifyVelocityXLeft = false;
        }
        else if (_player.CornerCorrection.HitRight && !_player.CornerCorrection.HitLeft && moveValueH >= -0.5f)
        {
            _player.transform.position -= new Vector3(_player.CornerCorrection.CornerDistance, 0f, 0f);
            canModifyVelocityXRight = false;
        }

        if (canModifyVelocityXLeft == false)
        {
            if (moveValueH > 0)
            {
                canModifyVelocityXLeft = true;
            }
        }
        else if (canModifyVelocityXRight == false)
        {
            if (moveValueH < 0)
            {
                canModifyVelocityXRight = true;
            }
        }

        if (moveValueH < 0 && canModifyVelocityXLeft == true)
        {
            velocityX = moveValueH != 0 ? moveValueH * _player.HorizontalJumpMovementMultiplier : _player.PlayerRigidbody.velocity.x;
        }
        else if (moveValueH > 0 && canModifyVelocityXRight == true)
        {
            velocityX = moveValueH != 0 ? moveValueH * _player.HorizontalJumpMovementMultiplier : _player.PlayerRigidbody.velocity.x;
        }
        else
        {
            velocityX = 0;
        }

        _player.PlayerRigidbody.velocity = new Vector2(velocityX, _player.PlayerRigidbody.velocity.y);
    }

    public override void ExitState() { }
    public override void InitializeSubState() { }
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

        // Passage en state FALL
        if (_player.Jump.WasReleasedThisFrame())
        {
            if (distanceFromOriginY <= _player.VerticalJumpDistanceLow)
            {
                _player.LowJumpActivated = true;
            }
            else 
            {
                SwitchState(_factory.Fall());
            }
        }

        if (TimePassedAtApex >= _player.MaxTimeAtApex)
        {
            SwitchState(_factory.Fall());
        }
        else if (_player.LowJumpActivated && distanceFromOriginY >= _player.VerticalJumpDistanceLow)
        {
            SwitchState(_factory.Fall());
        }

        if (_player.CornerCorrection.HitLeft && _player.CornerCorrection.HitRight)
        {
            if (_player.LowJumpActivated)
            {
                SwitchState(_factory.Fall());
            }
            else if (_player.TimePassedInState >= _player.TimeToGoToApex)
            {
                SwitchState(_factory.Fall());
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

        // Passage en state HEADBUTT ou HANG
        if (_player.GrabScript.NewStateFromGrab != null)
        {
            SwitchState(_player.GrabScript.NewStateFromGrab);
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        if (_player.TimePassedInState > 0.05f)
        {
            _player.LadderVerif(collision);
            
            if (_player.IsLadder != (int)LadderIs.Nothing)
            {
                SwitchState(_factory.WallIdle());
            }
        }
    }
}
