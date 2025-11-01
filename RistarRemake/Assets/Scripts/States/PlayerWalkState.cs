using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void EnterState() {
        //Debug.Log("ENTER WALK");
        _player.UpdateAnim("Walk");
        // Mise à jour du coyote time
        _player.CoyoteCounter = _player.CoyoteTime;
        //_ctx.MainCameraBehavior.PlayerTouchGround();
    }
    public override void UpdateState() { 
        CheckSwitchStates();
    }
    public override void FixedUpdateState() {
        // Déplacements du personnage
        float moveValue = _player.MoveH.ReadValue<float>();
        _player.Rb.velocity = new Vector2(moveValue * _player.WalkSpeed * Time.deltaTime, 0);

        if (_player.UseSpine == false)
        {
            // Rotation visuelle -- SANS SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SpriteRenderer.flipX = false;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SpriteRenderer.flipX = true;
            }
        }
        else
        {
            //Rotation visuelle -- AVEC SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = 1;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = -1;
            }
        }

        // Vérification d'un sol ou non
        if (_player.GroundDetection.IsGroundDectected == false)
        {
            SwitchState(_factory.Fall());
        }
        //Debug.Log(_ctx.LayerDetection.IsLayerDectected);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state IDLE
        float moveValue = _player.MoveH.ReadValue<float>();
        if (moveValue == 0)
        {
            SwitchState(_factory.Idle());
        }

        // Passage en state JUMP
        if (_player.Jump.WasPerformedThisFrame() || _player.JumpReady == true)
        {
            SwitchState(_factory.Jump());
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
