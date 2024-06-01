using UnityEngine;

[System.Serializable]
public class PowerUp
{
    private float moveSpeed;
    private float baseDamage;
    private float fireRate;
    private int health;
    private bool canTeleport;

    public float MoveSpeed => moveSpeed;
    public float BaseDamage => baseDamage;
    public float FireRate => fireRate;
    public bool CanTeleport => canTeleport;
    public int Health => health;

    public PowerUp(float moveSpeed, float baseDamage, float fireRate, bool canTeleport, int health)
    {
        this.moveSpeed = moveSpeed;
        this.baseDamage = baseDamage;
        this.fireRate = fireRate;
        this.canTeleport = canTeleport;
        this.health = health;
    }

    public void LoadPowerUps()
    {
        if (PlayerPrefs.HasKey("Health"))
        {
            health = PlayerPrefs.GetInt("Health");
        }

        if (PlayerPrefs.HasKey("MoveSpeed"))
        {
            moveSpeed = PlayerPrefs.GetFloat("MoveSpeed");
        }

        if (PlayerPrefs.HasKey("CanTeleport"))
        {
            canTeleport = PlayerPrefs.GetInt("CanTeleport") == 1;
        }

        if (PlayerPrefs.HasKey("BaseDamage"))
        {
            baseDamage = PlayerPrefs.GetFloat("BaseDamage");
        }

        if (PlayerPrefs.HasKey("FireRate"))
        {
            fireRate = PlayerPrefs.GetFloat("FireRate");
        }
    }

    public void SavePowerUps()
    {
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetFloat("MoveSpeed", moveSpeed);
        PlayerPrefs.SetInt("CanTeleport", canTeleport ? 1 : 0);
        PlayerPrefs.SetFloat("BaseDamage", baseDamage);
        PlayerPrefs.SetFloat("FireRate", fireRate);
        PlayerPrefs.Save();
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
        SavePowerUps();
    }

    public void IncreaseHealth(float amount)
    {
        health += (int)amount;
        SavePowerUps();
    }

    public void UnlockTeleport()
    {
        canTeleport = true;
        SavePowerUps();
    }

    public void IncreaseDamage(float amount)
    {
        baseDamage += amount;
        SavePowerUps();
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate += amount;
        SavePowerUps();
    }
}
