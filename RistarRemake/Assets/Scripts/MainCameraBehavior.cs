using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    // SETTINGS
    public GameObject target;
    [SerializeField] private GameObject player;
    public Vector3 CameraPositionFallOff;


    // VARIABLE DATA
    public float SizeDefault;
    public float SizeJump;

    private void Start()
    {
        target = player;
    }

    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z) + CameraPositionFallOff;
    }
}
