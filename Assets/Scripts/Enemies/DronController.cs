using UnityEngine;

public class DronController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float randomMoveInterval = 3f;
    [SerializeField] private float maxMagnitudeToRunAway;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private float nextShotTime;
    private GameObject fireLaserParent;
    private SpriteRenderer spriteRenderer;
    private Vector3 randomDirection;
    private float nextMoveChangeTime;
    private bool isChasingPlayer = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextShotTime = Time.time + shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        fireLaserParent = GameObject.Find("LasersParent");

        ChangeRandomDirection();
        nextMoveChangeTime = Time.time + randomMoveInterval;

        AssignDronName();
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) < maxMagnitudeToRunAway)
        {
            MoveAwayFromPlayer();
            HandleShooting();
        }
        else if (isChasingPlayer)
        {
            MoveTowardsPlayer();
            HandleShooting();
        }
        else
        {
            RandomMove();
        }

        FlipSprite();
    }

    private void AssignDronName()
    {
        GameObject dronParent = GameObject.FindGameObjectWithTag("DronParent");
        if (dronParent != null)
        {
            int dronCount = dronParent.transform.childCount;
            gameObject.name = "Dron_" + (dronCount + 1);
        }
        else
        {
            Debug.LogWarning("DronParent not found. Make sure there is a GameObject with the tag 'DronParent' in the scene.");
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
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

    void MoveAwayFromPlayer()
    {
        Vector3 direction = transform.position - player.position;
        if (direction.magnitude < maxMagnitudeToRunAway)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void HandleShooting()
    {
        if (Time.time > nextShotTime)
        {
            ShootLaser();
            nextShotTime = Time.time + shootingInterval;
        }
    }

    void ShootLaser()
    {
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation, fireLaserParent.transform);
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasingPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasingPlayer = false;
        }
    }
}
