using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        Debug.Log("ENTER SPIN");
        _ctx.UpdateAnim("Spin");
        if (_ctx.UseSpine == false)
        {
            _ctx.Arms.gameObject.SetActive(false);
            // Move Left Arm
            _ctx.IkArmLeft.transform.position = _ctx.DefaultPosLeft.position;
            // Move Right Arm
            _ctx.IkArmRight.transform.position = _ctx.DefaultPosRight.position;
        }
        _ctx.Rb.gravityScale = 1;

        if (_ctx.ArmDetection.ObjectDetected == 5)
        {
            if (_ctx.SpriteRenderer.flipX == true)
            {
                _ctx.Rb.velocity = new Vector2(1, 1) * 4;
            }
            else
            {
                _ctx.Rb.velocity = new Vector2(-1, 1) * 4;
            }
        }

        if (_ctx.ArmDetection.ObjectDetected == 2)
        {
            _ctx.Rb.velocity = new Vector2(0, 1) * 10;
        }
    }
    public override void UpdateState()
    {
        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}