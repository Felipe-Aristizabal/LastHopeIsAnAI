using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed;
    private int health;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    public static event Action OnPlayerDeath;
    private PlayerPowerUps playerPowerUps;
    private bool isTakingDamage = false;

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

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
