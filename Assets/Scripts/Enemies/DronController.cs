using System.Collections.Generic;
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
    private Rigidbody2D rb2D;
    private Vector3 randomDirection;
    private float nextMoveChangeTime;
    private bool isChasingPlayer = false;
    private bool isPlayerAlive = true;
    private List<GameObject> laserPool;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();

        if (rb2D == null)
        {
            Debug.LogError("Rigidbody2D is missing on the drone!");
            return;
        }

        rb2D.isKinematic = true; 

        nextShotTime = Time.time + shootingInterval;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        fireLaserParent = GameObject.Find("LasersParent");

        ChangeRandomDirection();
        nextMoveChangeTime = Time.time + randomMoveInterval;

        AssignDronName();
        PlayerController.OnPlayerDeath += StopAttacking;

        CreateLaserPool();
    }

    void FixedUpdate()
    {
        if (isPlayerAlive)
        { 
            HandleShooting();
        }

        if (Vector3.Distance(transform.position, player.position) < maxMagnitudeToRunAway)
        {
            MoveAwayFromPlayer();
        }
        else if (isChasingPlayer)
        {
            MoveTowardsPlayer();
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

    private void CreateLaserPool()
    {
        GameObject dronLaserParent = new GameObject(gameObject.name);
        dronLaserParent.transform.parent = fireLaserParent.transform;

        laserPool = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject laser = Instantiate(laserPrefab, dronLaserParent.transform);
            laser.SetActive(false);
            laserPool.Add(laser);
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
        if (isChasingPlayer && player != null)
        {
            if (Time.time > nextShotTime)
            {
                ShootLaser();
                nextShotTime = Time.time + shootingInterval;
            }
        }
    }

    void ShootLaser()
    {
        GameObject laser = GetPooledLaser();
        if (laser != null)
        {
            laser.transform.position = firePoint.position;
            laser.transform.rotation = firePoint.rotation;
            laser.SetActive(true);
        }
    }

    GameObject GetPooledLaser()
    {
        foreach (GameObject laser in laserPool)
        {
            if (!laser.activeInHierarchy)
            {
                return laser;
            }
        }
        return null;
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

    private void StopAttacking()
    {
        isPlayerAlive = false;
    }
}
