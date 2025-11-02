using UnityEngine;
using Sirenix.OdinInspector;

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
    }
}
