using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    private void Awake()
    {
        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
    }
}
