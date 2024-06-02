using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float health = 50f; 
    private SpriteRenderer spriteRenderer;
    private bool isTakingDamage = false;
    private PlayerController playerController;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on the enemy!");
        }

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void TakeDamage(float damage)
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
        Color originalColor = spriteRenderer.color;

        transform.localScale = originalScale * 1.2f;
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
        isTakingDamage = false;
    }

    private void Die()
    {
        playerController.EnemyDefeated();
        gameObject.SetActive(false);
    }
}

