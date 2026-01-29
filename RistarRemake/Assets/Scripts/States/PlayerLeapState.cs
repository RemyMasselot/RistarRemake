using UnityEngine;
using static PlayerStateMachine;

public class PlayerLeapState : PlayerBaseState
{
    public PlayerLeapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float currentTimeLeapCurve = 0f;
    private Vector3 leapStartPosition;

    private bool isLeapingWithoutControl;

    public override void EnterState()
    {
        //Debug.Log("JUMP ENTER");
        isLeapingWithoutControl = true;

        leapStartPosition = _player.transform.position;
    }

    public override void UpdateState()
    {
        _player.PlayerDirectionVerif();

        SetPlayerPosition();

        AirControl();

        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    private void SetPlayerPosition()
    {

        float lastKeyTime = _player.LeapCurve.keys[_player.LeapCurve.length - 1].time;

        if (currentTimeLeapCurve < lastKeyTime)
        {
            currentTimeLeapCurve += Time.deltaTime;
            float curveTime = Mathf.Clamp01(currentTimeLeapCurve / _player.TimeToLeap);
            float curveValue = _player.LeapCurve.Evaluate(curveTime); // renvoie une valeur entre 0 et 1

            if (isLeapingWithoutControl)
            {
                float valueX = 0;

                if (_player.IsPlayerTurnToLeft)
                {
                    //_player.transform.position = leapStartPosition + new Vector3(-curveTime * _player.LeapForce, curveValue * _player.LeapForce / 2, _player.transform.position.z);
                    valueX = leapStartPosition.x - curveTime * _player.LeapForce;
                }
                else if (_player.IsPlayerTurnToLeft == false)
                {
                    //_player.transform.position = leapStartPosition + new Vector3(curveTime * _player.LeapForce, curveValue * _player.LeapForce / 2, _player.transform.position.z);
                    valueX = leapStartPosition.x + curveTime * _player.LeapForce;
                }

                _player.transform.position = new Vector2(valueX, _player.transform.position.y);
            }
            
            _player.transform.position = new Vector2(_player.transform.position.x, leapStartPosition.y + curveValue * _player.LeapForce / 2);
        }
        else
        {
            SwitchState(_factory.Fall());
        }
    }

    private void AirControl()
    {
        float moveValueH = _player.MoveH.ReadValue<float>();
        if (moveValueH != 0)
        {
            isLeapingWithoutControl = false;
        }

        float velocityX = _player.PlayerRigidbody.velocity.x;
        velocityX = moveValueH != 0 ? moveValueH * _player.HorizontalJumpMovementMultiplier : _player.PlayerRigidbody.velocity.x;

        if (moveValueH > 0)
        {
            if (velocityX < _player.WalkMinSpeed)
            {
                velocityX = _player.WalkMinSpeed;
            }
        }
        else if (moveValueH < 0 && velocityX > -_player.WalkMinSpeed)
        {
            velocityX = -_player.WalkMinSpeed;
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

        // Passage en state GRAB
        //if (_player.IsGrabing == false)
        //{
        //    if (_player.Grab.WasPerformedThisFrame())
        //    {
        //        //SwitchState(_factory.Grab());
        //        _player.StartGrab();
        //    }
        //}
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
