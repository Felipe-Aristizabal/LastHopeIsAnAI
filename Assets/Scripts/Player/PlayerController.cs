using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb2D;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;


    private int health;
    public int Health
    {
        get { return health; }
        private set { health = value; }
    }

    public static event Action OnPlayerDeath;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

        health = 100;
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
        AdjustPlayerFacingDirection();
        Move();
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

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x) { mySpriteRenderer.flipX = true; }
        else { mySpriteRenderer.flipX = false; }
    }


    void OnCollisionEnter(Collision other)
    {
        switch (other.transform.tag)
        {
            case "Laser": 
                TakeDamage(20); 
                break;
            default: 
                TakeDamage(0);
                break;
        }
    }
    private void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }
}
