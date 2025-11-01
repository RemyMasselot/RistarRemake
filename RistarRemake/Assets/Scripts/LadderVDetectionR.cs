using UnityEngine;

public class LadderVDetectionR : MonoBehaviour
{
    [field: SerializeField] public int IsLadderVDectectedR { get; private set; }

    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches � reconna�tre


    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
        if (hit != null)
        {
            IsLadderVDectectedR = hit.gameObject.layer;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
