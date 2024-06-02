using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float randomMoveInterval = 3f;
    [SerializeField] private float explosionDistance = 1f;
    [SerializeField] private float explosionDamage = 40f; 

    private Animator animator;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInRange = false;
    private Vector3 randomDirection;
    private float nextMoveChangeTime;
    private bool isExploding = false;
    private Color originalColor;
    private Vector3 originalScale;

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

        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (isPlayerInRange)
        {
            if (isExploding) { return; }
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
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    IEnumerator Explode()
    {
        if (isExploding) yield break;

        isExploding = true;

        Color targetColor = Color.red;
        Vector3 targetScale = Vector3.Min(originalScale * 1.2f, new Vector3(3f, 3f, 3f));

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, t);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("_CanExplode", true);
        DealDamageToPlayer();
        AudioSource explosionAudio = GetComponent<AudioSource>();
        explosionAudio.Play();

        StartCoroutine(DeactivateMe());
    }

    IEnumerator DeactivateMe()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }

    private void DealDamageToPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < explosionDistance)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage((int)explosionDamage);
            }
        }
    }

    IEnumerator ResetMicroAnimation()
    {
        float elapsedTime = 0f;
        float duration = 1f;

        Color startColor = spriteRenderer.color;
        Vector3 startScale = transform.localScale;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            spriteRenderer.color = Color.Lerp(startColor, originalColor, t);
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
        transform.localScale = originalScale;
        isExploding = false;
        animator.SetBool("_CanExplode", false); // Reset animator parameter
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            animator.SetBool("isRunning", true); 
        }
        else if (other.CompareTag("BoundCollider"))
        {
            ChangeRandomDirection();
        } 
        else if (other.CompareTag("ExternalBounds"))
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            animator.SetBool("isRunning", false); 
            StopAllCoroutines(); // Stop the explode coroutine if it's running
            StartCoroutine(ResetMicroAnimation());
        }
    }
}
