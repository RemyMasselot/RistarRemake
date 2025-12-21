using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public bool IsDectected;
    private Collider2D DectectedCollider;

    public Vector2 CapsuleSize;
    [SerializeField] private LayerMask LayerToCheck;


    void FixedUpdate()
    {
        IsDectected = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);

        IsDectected = DectectedCollider != null ? true : false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
