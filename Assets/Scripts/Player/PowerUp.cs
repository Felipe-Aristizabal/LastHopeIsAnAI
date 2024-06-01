using UnityEngine;

[System.Serializable]
public class PowerUp
{
    public static PowerUp Instance { get; private set;}

    private float moveSpeed;
    private float baseDamage;
    private float fireDamage;
    private float fireRange;
    private float fireRate;
    private int health;
    private bool canTeleport;

    public const float maxMoveSpeed = 10f;
    public const float maxBaseDamage = 15f;
    public const float minFireRate = 0.5f;
    //Can teleport
    public const int maxHealth = 200;
    public const float maxFireDamage = 10f;
    public const float maxFireRange = 4f;
    

    public float MoveSpeed => moveSpeed;
    public float BaseDamage => baseDamage;
    public float FireDamage => fireDamage;
    public float FireRange => fireRange;
    public float FireRate => fireRate;
    public bool CanTeleport => canTeleport;
    public int Health => health;

    public PowerUp(float moveSpeed, float baseDamage, float fireRate, bool canTeleport, int health, float fireDamage, float fireRange)
    {
        this.moveSpeed = moveSpeed;
        this.baseDamage = baseDamage;
        this.fireRate = fireRate;
        this.canTeleport = canTeleport;
        this.health = health;
        this.fireDamage = fireDamage;
        this.fireRange = fireRange;
    }

    public void SetInstance(PowerUp powerUpToSet)
    {
        Instance = powerUpToSet;
    }

    public void LoadPowerUps()
    {
        if (PlayerPrefs.HasKey("Health"))
        {
            health = PlayerPrefs.GetInt("Health");
            Debug.Log("Health: " + health);
        }

        if (PlayerPrefs.HasKey("MoveSpeed"))
        {
            moveSpeed = PlayerPrefs.GetFloat("MoveSpeed");
            Debug.Log("MoveSpeed: " + moveSpeed);
        }

        if (PlayerPrefs.HasKey("CanTeleport"))
        {
            canTeleport = PlayerPrefs.GetInt("CanTeleport") == 1;
            Debug.Log("CanTeleport: " + canTeleport);
        }

        if (PlayerPrefs.HasKey("BaseDamage"))
        {
            baseDamage = PlayerPrefs.GetFloat("BaseDamage");
            Debug.Log("BaseDamage: " + baseDamage);
        }

        if (PlayerPrefs.HasKey("FireRate"))
        {
            fireRate = PlayerPrefs.GetFloat("FireRate");
            Debug.Log("FireRate: " + fireRate);
        }

        if (PlayerPrefs.HasKey("FireDamage"))
        {
            fireDamage = PlayerPrefs.GetFloat("FireDamage");
            Debug.Log("FireDamage: " + fireDamage);
        }

        if (PlayerPrefs.HasKey("FireRange"))
        {
            fireRange = PlayerPrefs.GetFloat("FireRange");
            Debug.Log("FireRange: " + fireRange);
        }
    }

    public void SavePowerUps()
    {
        PlayerPrefs.SetInt("Health", health);
        PlayerPrefs.SetFloat("MoveSpeed", moveSpeed);
        PlayerPrefs.SetInt("CanTeleport", canTeleport ? 1 : 0);
        PlayerPrefs.SetFloat("BaseDamage", baseDamage);
        PlayerPrefs.SetFloat("FireRate", fireRate);
        PlayerPrefs.SetFloat("FireDamage", fireDamage);
        PlayerPrefs.SetFloat("FireRange", fireRange);
        PlayerPrefs.Save();
    }

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed = Mathf.Min(moveSpeed + amount, maxMoveSpeed);
        SavePowerUps();
    }

    public void IncreaseHealth(float amount)
    {
        health = Mathf.Min(health + (int)amount, maxHealth);
        SavePowerUps();
    }

    public void UnlockTeleport()
    {
        canTeleport = true;
        SavePowerUps();
    }

    public void IncreaseDamage(float amount)
    {
        baseDamage = Mathf.Min(baseDamage + amount, maxBaseDamage);
        SavePowerUps();
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate = Mathf.Max(fireRate - amount, minFireRate);
        SavePowerUps();
    }

    public void IncreaseFireDamage(float amount)
    {
        fireDamage = Mathf.Min(fireDamage + amount, maxFireDamage);
        SavePowerUps();
    }

    public void IncreaseFireRange(float amount)
    {
        fireRange = Mathf.Min(fireRange + amount, maxFireRange);
        SavePowerUps();
    }

    public float ReturnMaxMoveSpeed() { return maxMoveSpeed; }
    public float ReturnMaxBaseDamage() { return maxBaseDamage; }
    public float ReturnMaxFireDamage() { return maxFireDamage; }
    public float ReturnMaxFireRange() { return maxFireRange; }
    public float ReturnMaxFireRate() { return minFireRate; }
    public int ReturnMaxHealth() { return maxHealth; }
    public bool ReturnCanTeleport() { return canTeleport; }

}
