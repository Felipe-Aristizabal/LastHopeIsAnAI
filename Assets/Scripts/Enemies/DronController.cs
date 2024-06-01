using UnityEngine;

public class DronController : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float shootingInterval = 2f;
    [SerializeField] private float maxMagnitudeToRunAway;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;

    private Transform player;
    private float nextShotTime;
    private GameObject fireLaserParent;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextShotTime = Time.time + shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Debug.Log(player.name);
        fireLaserParent = GameObject.Find("LasersParent");
        Debug.Log(fireLaserParent.name);
    }

    void FixedUpdate()
    {
        MoveAwayFromPlayer();
        HandleShooting();
        FlipSprite();
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
}
