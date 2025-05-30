using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool IsGroundDectected { get; private set; }
    
    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches � reconna�tre


    void FixedUpdate()
    {
        IsGroundDectected = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
