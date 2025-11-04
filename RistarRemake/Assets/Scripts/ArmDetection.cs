using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDetection : MonoBehaviour
{
    public int ObjectDetected = 0;
    public PlayerGrabState PlayerGrabState;
    public bool EndAnim = false;
    public Vector2 SnapPosHand;
    public Vector2 SnapPosHandL;
    public Vector2 SnapPosHandR;
    [SerializeField] private Transform HandR;
    [SerializeField] private Transform HandL;

    public int rayCount = 8;              // Nombre de rayons
    public float rayDistance = 5f;         // Longueur des rayons
    public float angleRange = 90f;         // Ouverture en degrés (ex: 90°, 180°, etc.)
    public float rotationOffset = 0f;      // Rotation globale du cône (modifiable en temps réel)
    public LayerMask layerMask;
    public Color gizmoColor = Color.red;

    private void Update()
    {
        Vector3 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = 6;
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
                else if (hit.collider.CompareTag("Wall"))
                {
                    //SnapPosHand = hit.collider.gameObject.transform.position;
                    SnapPosHandL = HandL.position;
                    SnapPosHandR = HandR.position;
                    ObjectDetected = 5;
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
                else if (hit.collider.CompareTag("StarHandle"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = 4;
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
                else if (hit.collider.CompareTag("LadderV") || hit.collider.CompareTag("LadderH"))
                {
                    SnapPosHandL = HandL.position;
                    SnapPosHandR = HandR.position;
                    ObjectDetected = 3;
                    //Debug.Log("reg");
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = 2;
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
                else
                {
                    ObjectDetected = 1;
                    //PlayerGrabState.GrabDetectionVerif(ObjectDetected);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            // Calcul de l'angle pour chaque rayon
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            Gizmos.DrawRay(origin, direction * rayDistance);
        }
    }
}
