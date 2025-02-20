using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext) { 
    _context = currentContext;
    }

    public PlayerBaseState Grounded() {
        return new PlayerGroundedState(_context, this);
    }
    public PlayerBaseState Idle() {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Walk() {
        return new PlayerWalkState(_context, this);
    }
    public PlayerBaseState Jump() {
        return new PlayerJumpState(_context, this);
    }
    public PlayerBaseState Fall()
    {
        return new PlayerFallState(_context, this);
    }
    public PlayerBaseState Grab()
    {
        return new PlayerGrabState(_context, this);
    }
    public PlayerBaseState WallIdle()
    {
        return new PlayerWallIdleState(_context, this);
    }
    public PlayerBaseState WallClimb()
    {
        return new PlayerWallClimbState(_context, this);
    }
}
