using Sirenix.OdinInspector;
using UnityEngine;
using static PlayerStateMachine;

public class PlayerVisual : MonoBehaviour
{
    #region VARIABLES

    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private PlayerStateMachine playerStateMachine;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Animator animator;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer spriteRenderer;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handRight;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handLeft;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handOpen;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handClose;


    #endregion

    private void Update()
    {
        // PLAYER DIRECTION
        spriteRenderer.flipX = playerStateMachine.IsPlayerTurnToLeft;

        // NEW ANIMATION
        if (playerStateMachine.IsNewState)
        {
            ChooseAnimEnterNewState();

            if (playerStateMachine.CurrentState is not PlayerMeteorStrikeState)
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                spriteRenderer.flipY = false;
            }
        }
        else if (playerStateMachine.CurrentState is PlayerGrabState) // GRAB STATE ONGOING
        {
            if (playerStateMachine.ArmDetection.ObjectDetected != 0)
            {
                handRight.sprite = handClose;
                handLeft.sprite = handClose;
            }
        }
        else if (playerStateMachine.CurrentState is PlayerMeteorStrikeState) // METEOR STRIKE ONGOING
        {
            MeteorStrikeBodyRotation();
        }

        playerStateMachine.NewStateVerif();
    }

    private void ChooseAnimEnterNewState()
    {
        if (playerStateMachine.CurrentState is PlayerIdleState)
        {
            UpdateAnim("Idle");
        }
        if (playerStateMachine.CurrentState is PlayerWalkState)
        {
            UpdateAnim("Walk");
        }
        if (playerStateMachine.CurrentState is PlayerJumpState)
        {
            UpdateAnim("Jump");
        }
        if (playerStateMachine.CurrentState is PlayerFallState)
        {
            UpdateAnim("Fall");
        }
        if (playerStateMachine.CurrentState is PlayerGrabState)
        {
            EnterGrabStateInitialization();
            ChoiceGrabAnim();
            UpdateAnim("Grab");
        }
        if (playerStateMachine.CurrentState is PlayerHangState)
        {
            UpdateAnim("Hang");
            if (playerStateMachine.ArmDetection.ObjectDetected == 4)
            {
                animator.SetFloat("HangValue", 2);
            }
            else
            {
                animator.SetFloat("HangValue", 1);
            }
        }
        if (playerStateMachine.CurrentState is PlayerMeteorStrikeState)
        {
            UpdateAnim("MeteorStrike");
        }
        if (playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            UpdateAnim("Headbutt");
        }
        if (playerStateMachine.CurrentState is PlayerSpinState)
        {
            UpdateAnim("Spin");
        }
        if (playerStateMachine.CurrentState is PlayerWallIdleState)
        {
            ChooseBetweenVerticalOrHorizontalAnimLadder();
            UpdateAnim("WallIdle");
        }
        if (playerStateMachine.CurrentState is PlayerWallClimbState)
        {
            ChooseBetweenVerticalOrHorizontalAnimLadder();
            UpdateAnim("WallClimb");
        }
        if (playerStateMachine.CurrentState is PlayerWallJumpState)
        {
            UpdateAnim("Jump");
        }
        if (playerStateMachine.CurrentState is PlayerLeapState)
        {
            UpdateAnim("Jump");
        }
        if (playerStateMachine.CurrentState is PlayerDamageState)
        {
            UpdateAnim("Damage");
        }
        if (playerStateMachine.CurrentState is PlayerDeathState)
        {
            if (playerStateMachine.IsPlayerTurnToLeft)
            {
                UpdateAnim("DeathR");
            }
            else
            {
                UpdateAnim("DeathL");
            }
        }
    }
    private void UpdateAnim(string animName)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(param.name, false);
            }
        }
        animator.SetBool(animName, true);
    }


    private void ChooseBetweenVerticalOrHorizontalAnimLadder()
    {
        if (playerStateMachine.IsLadder == (int)LadderIs.VerticalLeft || playerStateMachine.IsLadder == (int)LadderIs.VerticalRight)
        {
            animator.SetFloat("WallVH", 0);
        }
        else if (playerStateMachine.IsLadder == (int)LadderIs.Horizontal)
        {
            animator.SetFloat("WallVH", 1);
        }
    }

    private void EnterGrabStateInitialization()
    {
        handRight.sprite = handOpen;
        handLeft.sprite = handOpen;

        if (playerStateMachine.IsPlayerTurnToLeft == false)
        {
            playerStateMachine.LineArmLeft.sortingOrder = 0;
            playerStateMachine.LineArmRight.sortingOrder = 10;
        }
        else
        {
            playerStateMachine.LineArmLeft.sortingOrder = 10;
            playerStateMachine.LineArmRight.sortingOrder = 0;
        }
    }

    private void ChoiceGrabAnim()
    {
        // Vérification d'un sol ou non
        if (playerStateMachine.GroundDetection.IsGroundDectected)
        {
            if (playerStateMachine.AimDir.y <= 0.6f)
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

    private void MeteorStrikeBodyRotation()
    {
        float angle = Mathf.Atan2(playerStateMachine.MeteorStrikeDirection.y, playerStateMachine.MeteorStrikeDirection.x) * Mathf.Rad2Deg;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (playerStateMachine.MeteorStrikeDirection.x > 0)
        {
            spriteRenderer.flipY = false;
        }
        if (playerStateMachine.MeteorStrikeDirection.x < 0)
        {
            spriteRenderer.flipY = true;
        }
    }
}
