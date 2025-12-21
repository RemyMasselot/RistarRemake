using UnityEngine;

public class Invincinbility : MonoBehaviour
{
    private PlayerStateMachine player;

    [HideInInspector] public bool IsInvincible = false;
    [HideInInspector] public float InvincibilityTime = 2;
    [HideInInspector] public float InvincibilityCounter;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStateMachine>();

        InvincibilityCounter = 0;
        InvincibilityTime = player.InvicibilityTime;
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
