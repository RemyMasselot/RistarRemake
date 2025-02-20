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
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    // INPUT ACTIONS
    [Space(10)]
    [Header("INPUT ACTIONS")]
    Controller controls;
    public InputAction MoveH;
    public InputAction MoveV;
    public InputAction Jump;
    public InputAction Grab;
    public InputAction Aim;
    public InputAction Back;

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

    // PHYSICS
    public Rigidbody2D Rb { get { return GetComponent<Rigidbody2D>(); } }
    [field: SerializeField] public LayerDetection GroundDetection { get; private set; }
    [field: SerializeField] public LayerDetection LadderVDetection { get; private set; }
    [field: SerializeField] public LayerDetection LadderHDetection { get; private set; }


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
        Back = controls.LAND.BACK;
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