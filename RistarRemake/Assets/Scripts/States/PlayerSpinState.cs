using DG.Tweening;
using UnityEngine;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    private bool hasEnded;

    public override void EnterState()
    {
        Debug.Log("ENTER SPIN");

        hasEnded = false;
        if (_player.UseSpine == false)
        {
            //_player.PlayerVisual.UpdateAnim("Spin");
            _player.Arms.gameObject.SetActive(false);
            _player.ArmDetection.gameObject.SetActive(false);
            // Move Left Arm
            _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
            // Move Right Arm
            _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
        }

        _player.PlayerRigidbody.gravityScale = 1;

        if (_player.ArmDetection.ObjectDetected == 5)
        {
            if (_player.IsPlayerTurnToLeft)
            {
                _player.PlayerRigidbody.velocity = new Vector2(1, 1) * 4;
            }
            else if (_player.IsPlayerTurnToLeft == false)
            {
                _player.PlayerRigidbody.velocity = new Vector2(-1, 1) * 4;
            }
            if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                _player.PlayerRigidbody.velocity = new Vector2(0, -1);
            }
        }

        if (_player.ArmDetection.ObjectDetected == 2)
        {
            //Debug.Log("greg");
            // CAMERA BEHAVIOR
            //_player.CameraImpacted = true;
            //_player.CameraInde = true;
            //_player.Camera.DOShakePosition(0.2f, _player.MainCameraBehavior.CamShakeHeabbutt, 5).OnComplete(() =>
            //    {
            //        _player.CameraImpacted = false;
            //    });
            _player.PlayerRigidbody.velocity = new Vector2(0, 1) * 10;
        }
    }
    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Spin") && stateInfo.normalizedTime >= 1f && !hasEnded)
        {
            hasEnded = true;
            OnAnimationEnd();
        }
    }

    void OnAnimationEnd()
    {
        //Debug.Log("Animation terminée !");
        SwitchState(_factory.Fall());
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}