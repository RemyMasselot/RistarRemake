using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext) { 
    _context = currentContext;
    }

    public PlayerBaseState Idle() {
        return new PlayerIdleState();
    }
    public PlayerBaseState Walk()
    {
        return new PlayerIdleState();
    }
    public PlayerBaseState Jump()
    {
        return new PlayerIdleState();
    }
    public PlayerBaseState Grounded()
    {
        return new PlayerIdleState();
    }
}
