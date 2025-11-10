using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    public float cornerDistance = 0.08f; // Distance de correction horizontale
    public LayerMask groundLayer;

    [Header("Box settings")]
    public float boxWidth = 0.02f;
    public float boxHeight = 0.1f;
    public float heightOffset = 0.5f;
    public float sideOffset = 0.3f;

    [Header("Ray settings")]
    public float rayLength = 0.06f; // Longueur des rayons latéraux pour vérifier l’espace

    private PlayerStateMachine playerStateMachine;

    void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    void Update()
    {
        if (playerStateMachine.CurrentState is PlayerJumpState)
        {
            Vector2 pos = transform.position;
            Vector2 boxSize = new Vector2(boxWidth, boxHeight);

            // Positions des OverlapBox
            Vector2 leftPos = new Vector2(pos.x - sideOffset, pos.y + heightOffset);
            Vector2 rightPos = new Vector2(pos.x + sideOffset, pos.y + heightOffset);

            // Détection de coin touché
            bool hitLeft = Physics2D.OverlapBox(leftPos, boxSize, 0f, groundLayer);
            bool hitRight = Physics2D.OverlapBox(rightPos, boxSize, 0f, groundLayer);

            // Raycasts horizontaux pour vérifier l’espace libre
            bool spaceRight = !Physics2D.Linecast(pos, pos + Vector2.right * (sideOffset + cornerDistance), groundLayer);
            bool spaceLeft = !Physics2D.Linecast(pos, pos + Vector2.left * (sideOffset + cornerDistance), groundLayer);

            // Correction
            if (hitLeft && !hitRight && spaceRight)
            {
                transform.position += new Vector3(cornerDistance, 0f, 0f);
            }
            else if (hitRight && !hitLeft && spaceLeft)
            {
                transform.position -= new Vector3(cornerDistance, 0f, 0f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        Vector2 boxSize = new Vector2(boxWidth, boxHeight);

        // OverlapBox (coins haut gauche/droit)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(pos.x - sideOffset, pos.y + heightOffset), boxSize);
        Gizmos.DrawWireCube(new Vector2(pos.x + sideOffset, pos.y + heightOffset), boxSize);

        // Linecasts latéraux
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, pos + Vector2.left * (sideOffset + cornerDistance));
        Gizmos.DrawLine(pos, pos + Vector2.right * (sideOffset + cornerDistance));
    }
}