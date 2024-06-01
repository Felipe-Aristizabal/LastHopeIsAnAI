using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    public PowerUp powerUp;

    void Awake()
    {
        powerUp = new PowerUp(3f, 10f, 1f, false, 100);
        powerUp.LoadPowerUps();
    }
}
