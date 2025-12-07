using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool IsDectected;
    public Collider2D DectectedCollider;

    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître


    void FixedUpdate()
    {
        DectectedCollider = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);

        IsDectected = DectectedCollider != null ? true : false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
