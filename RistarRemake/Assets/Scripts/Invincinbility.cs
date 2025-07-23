using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invincinbility : MonoBehaviour
{
    public bool IsInvincible = false;
    public float InvincibilityTime = 2;
    public float InvincibilityCounter;

    // Start is called before the first frame update
    void Start()
    {
        InvincibilityCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsInvincible == true)
        {
            InvincibilityCounter -= Time.deltaTime;
        }
        if (InvincibilityCounter <= 0 )
        {
            IsInvincible = false;
        }
    }
}
