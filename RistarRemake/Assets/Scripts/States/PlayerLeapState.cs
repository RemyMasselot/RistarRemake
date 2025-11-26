using System.Threading;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerLeapState : PlayerBaseState
{
    public PlayerLeapState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private float currentTimeLeapCurve = 0f;
    private Vector3 leapStartPosition;

    public override void EnterState()
    {
        //Debug.Log("JUMP ENTER");

        leapStartPosition = _player.transform.position;
    }

    public override void UpdateState()
    {
        _player.PlayerDirectionVerif();

        SetPlayerPosition();

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

            //float moveValueH = _player.MoveH.ReadValue<float>();

            //curveTime += moveValueH;

            if (_player.IsPlayerTurnToLeft)
            {
                _player.transform.position = leapStartPosition + new Vector3(-curveTime * _player.LeapForce, curveValue * _player.LeapForce / 2, _player.transform.position.z);
            }
            else if (_player.IsPlayerTurnToLeft == false)
            {
                _player.transform.position = leapStartPosition + new Vector3(curveTime * _player.LeapForce, curveValue * _player.LeapForce / 2, _player.transform.position.z);
            }
            //Debug.Log("X : " + curveTime + "Y : " + curveValue);
        }
        else
        {
            SwitchState(_factory.Fall());
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
