using UnityEngine;

public class PlayerDamageState : PlayerBaseState
{
    public PlayerDamageState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    /// <summary>
    /// Propulser le personnage dans la direction opposé eà l'ennemi avec lequel il est entré en collision
    /// </summary>

    public override void EnterState() 
    {
        Debug.Log("ENTER DAMAGE");

        _player.LifesNumber--;

        if (_player.LifesNumber > 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(0, 10);

            _player.Invincinbility.InvincibilityCounter = _player.Invincinbility.InvincibilityTime;
            _player.Invincinbility.IsInvincible = true;
        }
        else
        {
            _player.PlayerRigidbody.velocity = Vector2.zero;
        }
    }
    public override void UpdateState() 
    {
        _player.CountTimePassedInState();

        CheckSwitchStates();
    }

    public override void FixedUpdateState() 
    {
        MoveX();
    }

    private void MoveX()
    {
        float moveValue = _player.MoveH.ReadValue<float>();
        if (moveValue != 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(moveValue * _player.HorizontalJumpMovementMultiplier, _player.PlayerRigidbody.velocity.y);
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y);
        }
    }

    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Passage en state DEATH
        if (_player.LifesNumber <= 0)
        {
            SwitchState(_factory.Death());
        }
        
        // Passage en state FALL
        if (_player.TimePassedInState >= 0.2f)
        {
            SwitchState(_factory.Fall());
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
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
