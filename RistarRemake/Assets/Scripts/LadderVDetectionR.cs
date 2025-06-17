using UnityEngine;

public class LadderVDetectionR : MonoBehaviour
{
    [field: SerializeField] public int IsLadderVDectectedR { get; private set; }

    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître


    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
        if (hit != null)
        {
            IsLadderVDectectedR = hit.gameObject.layer;
            //if (hit.gameObject.layer == LayerMask.NameToLayer("LadderV"))
            //{
            //    IsLadderVDectectedR = 1;
            //}
            //if (hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            //{
            //    IsLadderVDectectedR = 2;
            //}
            //else
            //{
            //    IsLadderVDectectedR = 0;
            //}
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
