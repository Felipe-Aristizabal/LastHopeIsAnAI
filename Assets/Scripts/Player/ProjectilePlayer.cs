using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private Vector2 direction;
    private float damage;
    private float fireRange;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on the projectile!");
        }
    }

    void OnEnable()
    {
        Invoke("DeactivateProjectile", fireRange);
    }

    public void SetDirection(Vector2 newDirection, bool isFlipped)
    {
        direction = newDirection;
        RotateTowardsDirection(newDirection, isFlipped);

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetFireRange(float newfireRange)
    {
        fireRange = newfireRange;
    }

    private void RotateTowardsDirection(Vector2 direction, bool isFlipped)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + (isFlipped ? 90 : 0)));
    }

    public void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        DeactivateProjectile();
    }
}
