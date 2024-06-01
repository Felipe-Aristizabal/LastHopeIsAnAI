using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float randomMoveInterval = 3f;
    [SerializeField] private float explosionDistance = 1f;
    [SerializeField] private GameObject explosionPrefab;

    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInRange = false;
    private Vector3 randomDirection;
    private float nextMoveChangeTime;
    private bool isExploding = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on the Micro!");
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on the Micro!");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }
    }

    void FixedUpdate()
    {
        if (isPlayerInRange)
        {
            FollowPlayer();
            if (Vector3.Distance(transform.position, player.position) < explosionDistance)
            {
                StartCoroutine(Explode());
            }
        }
        else 
        {
            RandomMove();
        }

        FlipSprite();
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void RandomMove()
    {
        if (Time.time > nextMoveChangeTime)
        {
            ChangeRandomDirection();
            nextMoveChangeTime = Time.time + randomMoveInterval;
        }

        Vector3 newPosition = transform.position + randomDirection * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
    }

    void ChangeRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        randomDirection = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0).normalized;
    }

    void FlipSprite()
    {
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    IEnumerator Explode()
    {
        isExploding = true;

        Color originalColor = spriteRenderer.color;
        Color targetColor = Color.red;

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            animator.SetBool("isRunning", true); 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            animator.SetBool("isRunning", false); 
        }
    }
}
