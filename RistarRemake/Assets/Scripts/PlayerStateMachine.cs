using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    ///////////////////////////////////////////////////////       VARIABLES       ///////////////////////////////////////////////////////
    // STATES
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // INPUT ACTIONS
    [Space(10)]
    [Header("INPUT ACTIONS")]
    Controller controls;
    public InputAction Walk;
    public InputAction Jump;

    // MOVE
    public Transform Transform;
    public float WalkSpeed = 10;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    private void Awake()
    {
        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

        Transform = this.transform;
    }

    ///////////////////////////////////////////////////////       START       ///////////////////////////////////////////////////////
    private void Start()
    {
        // Set input actions
        controls = new Controller();
        controls.Enable();
        Walk = controls.LAND.WALK;
        Jump = controls.LAND.JUMP;
    }

    ///////////////////////////////////////////////////////       UPDATE       ///////////////////////////////////////////////////////
    void Update()
    {
        _currentState.UpdateState();
    }

    ///////////////////////////////////////////////////////       UPDATE       ///////////////////////////////////////////////////////
    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }
}