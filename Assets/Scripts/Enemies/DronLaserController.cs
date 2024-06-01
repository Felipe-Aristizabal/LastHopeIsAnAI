using UnityEngine;

public class DronLaserController : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 direction;

    void OnEnable()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        if (playerTransform != null)
        {
            Vector2 playerPosition = playerTransform.position;
            direction = (playerPosition - (Vector2)transform.position).normalized;
            RotateTowardsDirection(direction);
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }

        Invoke("DeactivateLaser", 4f);
    }

    void RotateTowardsDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    public void DeactivateLaser()
    {
        gameObject.SetActive(false);
    }
}
