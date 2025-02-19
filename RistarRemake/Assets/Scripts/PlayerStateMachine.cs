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
    public InputAction MoveH;
    public InputAction MoveV;
    public InputAction Jump;
    public InputAction Grab;
    public InputAction Aim;

    // ANIM
    public Animator Animator;

    // MOVE
    public Transform Transform;
    public float WalkSpeed = 10;

    // JUMP
    [field:SerializeField] public float JumpForceV { get; private set; } = 7f;
    [field:SerializeField] public float JumpForceH { get; private set; } = 3f;

    // GRAB
    public GameObject Arms;
    [field: SerializeField] public ArmDetection ArmDetection { get; set; }

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Rigidbody2D Rb { get { return GetComponent<Rigidbody2D>(); } }


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
        MoveH = controls.LAND.MOVEH;
        MoveV = controls.LAND.MOVEV;
        Jump = controls.LAND.JUMP;
        Grab = controls.LAND.GRAB;
        Aim = controls.LAND.AIM;
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

    ///////////////////////////////////////////////////////       COLLISION       ///////////////////////////////////////////////////////
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentState.OnCollision(collision);
    }
}