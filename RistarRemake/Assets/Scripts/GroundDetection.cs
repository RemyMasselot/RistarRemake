using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool IsDectected;
    private Collider2D playerCollider;
    [SerializeField] private LayerMask LayerToCheck;

    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        CheckIfThereIsAGround();
    }

    private void CheckIfThereIsAGround()
    {
        Vector2 originLeft = new Vector2(playerCollider.bounds.min.x, playerCollider.bounds.min.y);
        Vector2 originRight = new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y);

        RaycastHit2D downLeftVerification = Physics2D.Raycast(originLeft, Vector2.down, 0.1f, LayerToCheck);
        RaycastHit2D downRightVerification = Physics2D.Raycast(originRight, Vector2.down, 0.1f, LayerToCheck);

        if (downLeftVerification.collider != null || downRightVerification.collider != null)
        {
            Vector2 offsetLeft = new Vector2(originLeft.x, originLeft.y + 0.2f);
            Vector2 offsetRight = new Vector2(originRight.x, originRight.y + 0.2f);

            RaycastHit2D sideLeftVerification = Physics2D.Raycast(offsetLeft, Vector2.left, 0.1f, LayerToCheck);
            RaycastHit2D sideRightVerification = Physics2D.Raycast(offsetRight, Vector2.right, 0.1f, LayerToCheck);

            if (sideLeftVerification.collider != null)
            {
                if (downLeftVerification.transform.GetInstanceID() == sideLeftVerification.transform.GetInstanceID())
                {
                    IsDectected = false;
                }
                else
                {
                    IsDectected = true;
                }
            }

            if (sideRightVerification.collider != null)
            {
                if (downRightVerification.transform.GetInstanceID() == sideRightVerification.transform.GetInstanceID())
                {
                    IsDectected = false;
                }
                else
                {
                    IsDectected = true;
                }
            }

            if (sideLeftVerification.collider == null && sideRightVerification.collider == null)
            {
                IsDectected = true;
            }
        }
        else
        {
            IsDectected = false;
        }
    }
}
