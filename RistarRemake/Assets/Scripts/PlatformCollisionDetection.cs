using UnityEngine;

public class PlatformCollisionDetection : MonoBehaviour
{
    public bool CeilingDetected = false;
    public bool WallDetected = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Savoir quel côté du player est touché
        Vector2 collisionNormal = collision.GetContact(0).normal;


        CeilingDetected = Vector2.Dot(collisionNormal, Vector2.down) > 0.5f ? true : false;

        if (Vector2.Dot(collisionNormal, Vector2.left) > 0.5f
            || Vector2.Dot(collisionNormal, Vector2.right) > 0.5f)
        {
            WallDetected = true;
        }
        else
        {
            WallDetected = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CeilingDetected = false;
        WallDetected = false;
    }
}
