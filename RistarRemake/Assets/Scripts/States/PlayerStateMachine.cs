using Spine.Unity;
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
    public PlayerStateFactory _states;
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
    public bool UseSpine = false;
    public Animator Animator;
    public SpriteRenderer SpriteRenderer;
    public SkeletonAnimation SkeletonAnimation;

    // MOVE
    public Transform Transform;
    public float WalkSpeed = 10;

    // JUMP
    [field:SerializeField] public float JumpForceV { get; private set; } = 7f;
    [field:SerializeField] public float JumpForceH { get; private set; } = 3f;

    // JUMP
    [field: SerializeField] public float LeapForceV { get; private set; } = 7f;
    [field: SerializeField] public float LeapForceH { get; private set; } = 3f;

    // CLIMB
    [field: SerializeField] public bool Leap { get; set; } = false;
    [field: SerializeField] public bool Fall { get; set; } = false;


    // GRAB
    public bool GamepadUsed;
    public GameObject Arms;
    [field: SerializeField] public ArmDetection ArmDetection { get; set; }
    [HideInInspector] public Vector2 AimDir;
    public float DistanceGrab;
    public float DurationGrab;
    public Transform IkArmRight;
    public Transform ShoulderRight;
    public Transform DefaultPosRight;
    public Transform IkArmLeft;
    public Transform ShoulderLeft;
    public Transform DefaultPosLeft;

    // STAR HANDLE
    public int StarHandleTargetValue = 200;

    // PHYSICS
    public Rigidbody2D Rb { get { return GetComponent<Rigidbody2D>(); } }
    [field: SerializeField] public GroundDetection GroundDetection { get; private set; }


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

    ///////////////////////////////////////////////////////       FIXED UPDATE       ///////////////////////////////////////////////////////
    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    ///////////////////////////////////////////////////////       COLLISION       ///////////////////////////////////////////////////////
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentState.OnCollision(collision);
    }

    public void UpdateAnim(string animName)
    {
        foreach (var param in Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                Animator.SetBool(param.name, false);
            }
        }
        Animator.SetBool(animName, true);
    }
}