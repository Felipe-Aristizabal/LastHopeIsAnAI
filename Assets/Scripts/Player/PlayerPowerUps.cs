using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public PowerUp powerUp;

    // public const float maxMoveSpeed = 10f;
    // public const float maxBaseDamage = 15f;
    // public const float minFireRate = 0.5f;
    // //Can teleport
    // public const int maxHealth = 200;
    // public const float maxFireDamage = 10f;
    // public const float maxFireRange = 4f;

    void Awake()
    {
        powerUp = new PowerUp(
            5f, 
            10f, 
            1.5f, 
            false, 
            100, 
            6f, 
            2f
        );
        powerUp.LoadPowerUps();
        powerUp.SetInstance(powerUp);
    }
}
