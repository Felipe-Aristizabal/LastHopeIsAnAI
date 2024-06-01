using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public PowerUp powerUp;

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
        powerUp.SavePowerUps();
        powerUp.LoadPowerUps();
        powerUp.SetInstance(powerUp);
    }
}
