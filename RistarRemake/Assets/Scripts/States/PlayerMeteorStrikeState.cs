using UnityEngine;
using DG.Tweening;

public class PlayerMeteorStrikeState : PlayerBaseState
{
    public PlayerMeteorStrikeState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    /// <summary>
    /// FAIRE LONGER LE PLAYER LE LONG DES PAROIS PLUTOT QUE DE REBONDIR
    /// </summary>


    private Vector2 dirPlayer;
    private bool canControl;

    public override void EnterState()
    {
        //Debug.Log("ENTER METEOR STRIKE");

        canControl = false;
        _player.IsPlayerTurnToLeft = false;
        _player.transform.rotation = Quaternion.Euler(0, 0, 0);
        //_player.PlayerRigidbody.gravityScale = 0;

        DOVirtual.DelayedCall(0.3f, () =>
        {
            canControl = true;
        });

        _player.MeteorStrikeDirection = _player.transform.position - _player.StarHandleCentre;
        _player.PlayerRigidbody.velocity = _player.MeteorStrikeDirection.normalized * 10;

        _player.ArmDetection.ObjectDetected = 0;
        _player.GroundDetection.gameObject.SetActive(false);
        _player.LadderHDetection.gameObject.SetActive(false);
        _player.LadderVDetectionL.gameObject.SetActive(false);
        _player.LadderVDetectionR.gameObject.SetActive(false);
        _player.TriggerGoToMeteorStrike.SetActive(false);

        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
    }

    public override void UpdateState()
    {
        _player.CountTimePassedInState();

        CheckSwitchStates();
    }

    public override void FixedUpdateState() 
    {
        // Meteor Strike Control
        if (canControl == true)
        {
            float moveValueH = _player.MoveH.ReadValue<float>();
            float moveValueV = _player.MoveV.ReadValue<float>();
            dirPlayer = new Vector2(moveValueH, moveValueV);

            _player.MeteorStrikeDirection = (_player.MeteorStrikeDirection + dirPlayer).normalized;
        }

        _player.transform.Translate(_player.MeteorStrikeDirection.normalized * _player.MeteorSpeed);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    { 
        if (_player.TimePassedInState >= _player.MaxTimeMeteor)
        {
            _player.GroundDetection.gameObject.SetActive(true);
            _player.LadderHDetection.gameObject.SetActive(true);
            _player.LadderVDetectionL.gameObject.SetActive(true);
            _player.LadderVDetectionR.gameObject.SetActive(true);

            if (_player.MeteorStrikeDirection.y <= 0)
            {
                SwitchState(_factory.Fall());
            }
            else
            {
                SwitchState(_factory.Jump());
            }
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("WALL");
            if (canControl == true)
            {
                canControl = false;
                _player.MeteorSpeed -= 0.1f;
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    canControl = true;
                    _player.MeteorSpeed += 0.1f;
                });
            }

            Vector2 normale = collision.GetContact(0).normal;
            _player.MeteorStrikeDirection = Vector2.Reflect(_player.MeteorStrikeDirection, normale);
        }
    }
}