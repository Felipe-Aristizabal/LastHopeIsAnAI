using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed;
    private int health;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;
    private float meleeAttackCooldown = 0f;
    private bool isMeleeAttacking = false;
    private bool isInvulnerable = false;

    public static event Action OnPlayerDeath;
    private PlayerPowerUps playerPowerUps;
    private bool isTakingDamage = false;

    private enum AttackMode { Melee, Ranged }
    private AttackMode currentAttackMode = AttackMode.Melee;

    [SerializeField] private GameObject rangedAttackPrefab;
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private BoxCollider2D meleeColliderLeft;
    [SerializeField] private BoxCollider2D meleeColliderRight;

    private List<GameObject> projectilePool;
    private GameObject poolParent;
    private float nextFireTime = 0f;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        if (rb2D == null) Debug.LogError("Rigidbody2D is missing!");
        if (myAnimator == null) Debug.LogError("Animator is missing!");
        if (mySpriteRenderer == null) Debug.LogError("SpriteRenderer is missing!");
    }

    private void Start()
    {
        playerPowerUps = GetComponent<PlayerPowerUps>();
        if (playerPowerUps == null) Debug.LogError("PlayerPowerUps component is missing!");

        if (playerPowerUps != null && playerPowerUps.powerUp != null)
        {
            moveSpeed = playerPowerUps.powerUp.MoveSpeed;
            health = playerPowerUps.powerUp.Health;
        }
        else
        {
            Debug.LogError("PlayerPowerUps component or PowerUp instance is missing!");
        }

        InitializeProjectilePool();

        meleeColliderLeft.enabled = false;
        meleeColliderRight.enabled = false;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Move();
        AdjustPlayerFacingDirection();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);

        if (Input.GetMouseButtonDown(1)) // Right click to switch modes
        {
            SwitchAttackMode();
        }

        if (Input.GetMouseButtonDown(0)) // Left click to attack
        {
            Attack();
        }
    }

    private void SwitchAttackMode()
    {
        if (currentAttackMode == AttackMode.Melee)
        {
            currentAttackMode = AttackMode.Ranged;
        }
        else
        {
            currentAttackMode = AttackMode.Melee;
        }
    }

    private void Attack()
    {
        if (Time.time < meleeAttackCooldown)
        {
            return; 
        }

        if (currentAttackMode == AttackMode.Melee)
        {
            myAnimator.SetBool("IsMeleeAttacking", true);
            isMeleeAttacking = true;
            isInvulnerable = true; 
            StartCoroutine(ActivateMeleeCollider());
            meleeAttackCooldown = Time.time + playerPowerUps.powerUp.FireRate;
            Debug.Log("Performing melee attack");
        }
        else if (currentAttackMode == AttackMode.Ranged)
        {
            myAnimator.SetBool("IsRangeAttacking", true);
            PerformRangedAttack();
            Debug.Log("Performing ranged attack");
        }
    }

    private IEnumerator ActivateMeleeCollider()
    {
        Transform selectedFirePoint = mySpriteRenderer.flipX ? firePointRight : firePointLeft;
        BoxCollider2D meleeCollider = selectedFirePoint.GetComponent<BoxCollider2D>();

        if (meleeCollider != null)
        {
            meleeCollider.enabled = true;
            meleeCollider.isTrigger = true; 
            meleeCollider.GetComponent<MeleeCollider>().Initialize(this);

            yield return new WaitForSeconds(0.4f); 
            meleeCollider.enabled = false;
            isInvulnerable = false; 
            myAnimator.SetBool("IsMeleeAttacking", false);
        }
        else
        {
            Debug.LogError("Melee collider not found on fire point");
        }
    }

    public void OnMeleeAttackTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(playerPowerUps.powerUp.FireDamage);
            }
        }
    }

    private void PerformRangedAttack()
    {
        Transform selectedFirePoint = mySpriteRenderer.flipX ? firePointRight : firePointLeft;
        GameObject rangedAttack = GetPooledProjectile();
        if (rangedAttack != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Vector2 direction = (mousePosition - selectedFirePoint.position).normalized;

            rangedAttack.transform.position = selectedFirePoint.position;
            rangedAttack.transform.rotation = Quaternion.identity;  

            ProjectilePlayer projectileScript = rangedAttack.GetComponent<ProjectilePlayer>();
            if (projectileScript != null)
            {
                projectileScript.SetDamage(playerPowerUps.powerUp.FireDamage);
                projectileScript.SetFireRange(playerPowerUps.powerUp.FireRange);
            }

            rangedAttack.SetActive(true);
            StartCoroutine(AnimateProjectile(rangedAttack, direction, mySpriteRenderer.flipX));
        }
    }

    private IEnumerator AnimateProjectile(GameObject projectile, Vector2 direction, bool isFlipped)
    {
        SpriteRenderer projectileRenderer = projectile.GetComponent<SpriteRenderer>();
        Color originalColor = projectileRenderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        projectile.transform.localScale = Vector3.one * 0.01f;
        projectileRenderer.color = transparentColor;

        float duration = 0.2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            projectile.transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, new Vector3(0.5f, 0.5f, 0.5f), t);
            projectileRenderer.color = Color.Lerp(transparentColor, originalColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        projectileRenderer.color = originalColor;
        
        ProjectilePlayer projectileScript = projectile.GetComponent<ProjectilePlayer>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction, isFlipped);
        }
    }

    private void InitializeProjectilePool()
    {
        poolParent = GameObject.FindGameObjectWithTag("PlayerRangedPool");
        if (poolParent == null)
        {
            Debug.LogError("PlayerRangedPool not found in the scene!");
            return;
        }

        projectilePool = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject projectile = Instantiate(rangedAttackPrefab, poolParent.transform);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    private GameObject GetPooledProjectile()
    {
        foreach (GameObject projectile in projectilePool)
        {
            if (!projectile.activeInHierarchy)
            {
                return projectile;
            }
        }
        return null;
    }

    private void Move()
    {
        rb2D.MovePosition(rb2D.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    public void TeleportToMouse()
    {
        if (playerPowerUps != null && playerPowerUps.powerUp.CanTeleport)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = Vector3.MoveTowards(transform.position, mousePos, 1f);
        }
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
        }
        else
        {
            mySpriteRenderer.flipX = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("MeleeCollider"))
        {
            return;
        }

        switch (other.transform.tag)
        {
            case "Laser":
                other.transform.GetComponent<DronLaserController>().DeactivateLaser();
                TakeDamage(20);
                break;
            case "Enemy":
                TakeDamage(5);
                break;
            default:
                TakeDamage(0);
                break;
        }
    }

    private void TakeDamage(int damage)
    {
        if (isInvulnerable)
        {
            return;
        }

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (!isTakingDamage)
            {
                StartCoroutine(TakingDamageAnimation());
            }
        }
    }

    private IEnumerator TakingDamageAnimation()
    {
        isTakingDamage = true;
        Vector3 originalScale = transform.localScale;
        Color originalColor = mySpriteRenderer.color;

        transform.localScale = originalScale * 1.2f;
        mySpriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        transform.localScale = originalScale;
        mySpriteRenderer.color = originalColor;
        isTakingDamage = false;
    }

    public void ResetMeleeAttack()
    {
        myAnimator.SetBool("IsMeleeAttacking", false);
    }

    public void ResetRangeAttack()
    {
        myAnimator.SetBool("IsRangeAttacking", false);
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
