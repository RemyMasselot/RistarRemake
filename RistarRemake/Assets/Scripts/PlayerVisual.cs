using Sirenix.OdinInspector;
using UnityEngine;
using static ArmDetection;

public class PlayerVisual : MonoBehaviour
{
    #region VARIABLES

    private PlayerStateMachine playerStateMachine;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform pivot;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handRight;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handLeft;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handOpen;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handClose;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private GameObject arms;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private LineRenderer lineArmRight;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private LineRenderer lineArmLeft;

    #endregion

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pivot = transform.parent;
        playerStateMachine.NewStatePlayed.AddListener(UpdateVisual);
    }

    private void Update()
    {
        ArmVisibility();

        if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing) // GRAB STATE ONGOING
        {
            if (playerStateMachine.AimDir.x < 0)
            {
                playerStateMachine.IsPlayerTurnToLeft = true;
            }
            else if(playerStateMachine.AimDir.x > 0)
            {
                playerStateMachine.IsPlayerTurnToLeft = false;
            }

            if (playerStateMachine.ArmDetection.ObjectGrabed != (int)ObjectGrabedIs.Nothing)
            {
                // HANDS CLOSE
                handRight.sprite = handClose;
                handLeft.sprite = handClose;
            }
        }
        
        ChangePlayerDirection();
        
        if (playerStateMachine.CurrentState is PlayerMeteorStrikeState) // METEOR STRIKE ONGOING
        {
            MeteorStrikeBodyRotation();
        }
        else if (playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            HeadbuttBodyRotation();
        }
    }

    private void ChangePlayerDirection()
    {
        float newScaleX = playerStateMachine.IsPlayerTurnToLeft ? -1 : 1;

        pivot.localScale = new Vector3(newScaleX, 1, 1);
    }

    private void UpdateVisual()
    {
        // NEW ANIMATION
        ChooseAnimationOnEnterNewState();

        // PUT BACK THE BODY ROTATION TO 0 IF NOT IN METEOR STRIKE
        if (playerStateMachine.CurrentState is not PlayerMeteorStrikeState)
        {
            pivot.rotation = Quaternion.Euler(0, 0, 0);
            spriteRenderer.flipY = false;
        }
    }

    private void ChooseAnimationOnEnterNewState()
    {
        if (playerStateMachine.CurrentState is PlayerIdleState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Idle");
        }
        else if (playerStateMachine.CurrentState is PlayerWalkState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Walk");
        }
        else if (playerStateMachine.CurrentState is PlayerJumpState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerFallState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Fall");
        }
        else if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing)
        {
            EnterGrabStateInitialization();
            ChoiceGrabAnimation();
            animator.SetTrigger("Grab");
        }
        else if (playerStateMachine.CurrentState is PlayerHangState && playerStateMachine.IsGrabing == false)
        {
            animator.SetFloat("HangValue", playerStateMachine.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.StarHandle ? 2 : 1);
            animator.SetTrigger("Hang");
        }
        else if (playerStateMachine.CurrentState is PlayerMeteorStrikeState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("MeteorStrike");
        }
        else if (playerStateMachine.CurrentState is PlayerHeadbuttState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Headbutt");
        }
        else if (playerStateMachine.CurrentState is PlayerSpinState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Spin");
        }
        else if (playerStateMachine.CurrentState is PlayerWallIdleState && playerStateMachine.IsGrabing == false)
        {
            ChooseBetweenVerticalOrHorizontalAnimationLadder();
            animator.SetTrigger("WallIdle");
        }
        else if (playerStateMachine.CurrentState is PlayerWallClimbState && playerStateMachine.IsGrabing == false)
        {
            ChooseBetweenVerticalOrHorizontalAnimationLadder();
            animator.SetTrigger("WallClimb");
        }
        else if (playerStateMachine.CurrentState is PlayerWallJumpState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerLeapState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerDamageState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Damage");
        }
        else if (playerStateMachine.CurrentState is PlayerDeathState && playerStateMachine.IsGrabing == false)
        {
            if (playerStateMachine.IsPlayerTurnToLeft)
            {
                animator.SetTrigger("DeathR");
            }
            else
            {
                animator.SetTrigger("DeathL");
            }
        }
    }

    private void ChooseBetweenVerticalOrHorizontalAnimationLadder()
    {
        if (playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.VerticalLeft || playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.VerticalRight)
        {
            animator.SetFloat("WallVH", 0);
        }
        else if (playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.Horizontal)
        {
            animator.SetFloat("WallVH", 1);
        }
    }

    private void EnterGrabStateInitialization()
    {
        handLeft.sprite = handOpen;
        handRight.sprite = handOpen;
    }

    private void ChoiceGrabAnimation()
    {
        // Vérification d'un sol ou non
        if (playerStateMachine.CurrentState is PlayerIdleState
            || playerStateMachine.CurrentState is PlayerWalkState)
        {
            if (playerStateMachine.AimDir.y <= 0.4f)
            {
                animator.SetFloat("GrabAnimId", 1);
            }
            else
            {
                animator.SetFloat("GrabAnimId", 2);
            }
        }
        else
        {
            if (playerStateMachine.AimDir.y <= -0.2f)
            {
                animator.SetFloat("GrabAnimId", 5);
            }
            else if (playerStateMachine.AimDir.y >= 0.2f)
            {
                animator.SetFloat("GrabAnimId", 4);
            }
            else
            {
                animator.SetFloat("GrabAnimId", 3);
            }
        }
    }

    private void ArmVisibility()
    {
        // Draw Line Arm
        lineArmLeft.SetPosition(0, playerStateMachine.ShoulderLeft.position);
        lineArmLeft.SetPosition(1, playerStateMachine.IkArmLeft.position);
        lineArmRight.SetPosition(0, playerStateMachine.ShoulderRight.position);
        lineArmRight.SetPosition(1, playerStateMachine.IkArmRight.position);

        if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing
            || playerStateMachine.CurrentState is PlayerHangState
            || playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            HandsRotation();
            arms.SetActive(true);
        }
        else
        {
            arms.SetActive(false);
        }
    }

    private void HandsRotation()
    {
        Vector2 lineReference = new Vector2(0, 1);

        Vector2 directionHandLeft = lineArmLeft.GetPosition(1) - lineArmLeft.GetPosition(0);
        float angleHandLeft = Vector2.SignedAngle(lineReference, directionHandLeft);

        Vector2 directionHandRight = lineArmLeft.GetPosition(1) - lineArmLeft.GetPosition(0);
        float angleHandRight = Vector2.SignedAngle(lineReference, directionHandRight);

        playerStateMachine.IkArmRight.transform.rotation = Quaternion.Euler(0, 0, angleHandRight);
        playerStateMachine.IkArmLeft.transform.rotation = Quaternion.Euler(0, 0, angleHandLeft);
    }

    private void MeteorStrikeBodyRotation()
    {
        float angle = Mathf.Atan2(playerStateMachine.MeteorStrikeDirection.y, playerStateMachine.MeteorStrikeDirection.x) * Mathf.Rad2Deg;
        pivot.rotation = Quaternion.Euler(0, 0, angle);

        if (playerStateMachine.MeteorStrikeDirection.x != 0)
        {
            float newScaleY = playerStateMachine.MeteorStrikeDirection.x > 0 ? 1 : -1;
            pivot.localScale = new Vector3(pivot.localScale.x, newScaleY, 1);
        }
    }

    private void HeadbuttBodyRotation()
    {
        Vector2 lineReference = new Vector2(0, 1);
        Vector2 direction = playerStateMachine.HeadbuttDirection;

        float spriteAngleCorrection = playerStateMachine.IsPlayerTurnToLeft ? -23f : 23f;

        float angle = Vector2.SignedAngle(lineReference, direction);
        pivot.rotation = Quaternion.Euler(0, 0, angle + spriteAngleCorrection);
        //Debug.Log(angle);
    }
}
