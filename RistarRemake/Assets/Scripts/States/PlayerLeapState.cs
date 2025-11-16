using UnityEngine;

public class PlayerLeapState : PlayerBaseState
{
    public PlayerLeapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float startLeapPositionY;
    private bool newOrientation;
    private float currentTimeLeapCurve = 0f;
    private Vector3 leapStartPosition;

    public override void EnterState()
    {
        //Debug.Log("JUMP ENTER");

        //_player.PlayerRigidbody.gravityScale = 1;

        // animation curve
        // position XY selon le temps
        // Temps du leap
        // Set player.veliocity to animation curve value

        leapStartPosition = _player.transform.position;

        //_player.PlayerRigidbody.velocity = new Vector2(0, _player.LeapForceV);

        //startLeapPositionY = _player.transform.position.y;
        //newOrientation = false;
    }

    public override void UpdateState()
    {
        _player.PlayerDirectionVerif();

        SetPlayerPosition();

        CheckSwitchStates();
    }

    private void SetPlayerPosition()
    {

        float lastKeyTime = _player.LeapCurve.keys[_player.LeapCurve.length - 1].time;

        if (currentTimeLeapCurve < lastKeyTime)
        {
            currentTimeLeapCurve += Time.deltaTime;
            float curveTime = Mathf.Clamp01(currentTimeLeapCurve / _player.TimeToLeap);
            float curveValue = _player.LeapCurve.Evaluate(curveTime); // renvoie une valeur entre 0 et 1
            _player.transform.position = leapStartPosition + new Vector3(-curveTime * _player.LeapForce, curveValue * _player.LeapForce / 2, _player.transform.position.z);
            //Debug.Log("X : " + curveTime + "Y : " + curveValue);
        }
        else
        {
            SwitchState(_factory.Fall());
        }
    }

    public override void FixedUpdateState()
    {
        //if (_player.transform.position.y >= startLeapPositionY + 1.5f)
        //{
        //    if (newOrientation == false)
        //    {
        //        newOrientation = true;
        //        if (_player.IsPlayerTurnToLeft == false)
        //        {
        //            _player.PlayerRigidbody.velocity = new Vector2(_player.LeapForceH, _player.LeapForceV / 2);
        //        }
        //        else
        //        {
        //            _player.PlayerRigidbody.velocity = new Vector2(-_player.LeapForceH, _player.LeapForceV / 2);
        //        }
        //    }

        //    AirControl();
        //}
    }

    private void AirControl()
    {
        float moveValueH = _player.MoveH.ReadValue<float>();
        float moveValueV = Mathf.Clamp(_player.MoveV.ReadValue<float>(), _player.InputFallSpeedIncrease, _player.InputFallSpeedDecrease);
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

        //// Passage en state FALL
        //if (_player.PlayerRigidbody.velocity.y < 0)
        //{
        //    SwitchState(_factory.Fall());
        //}
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
