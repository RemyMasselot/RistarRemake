using UnityEngine;

public class ShowLadder : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.green;


    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = gizmoColor;

        var size = new Vector3(box.size.x, box.size.y, 0.01f);
        var center = new Vector3(box.offset.x, box.offset.y, 0f);
        Gizmos.DrawWireCube(center, size);
    }
}
