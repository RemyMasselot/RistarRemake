using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDetection : MonoBehaviour
{
    public int ObjectDetected = 0;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            ObjectDetected = 3;
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            ObjectDetected = 2;
        }
        else
        {
            ObjectDetected = 1;
        }
    }
}
