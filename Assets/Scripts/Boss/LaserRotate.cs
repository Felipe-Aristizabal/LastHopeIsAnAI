using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRotate : MonoBehaviour
{
    private enum LaserType
    {
        Laser1,
        Laser2
    }
    [Header("Base Stats")]
    [SerializeField] private LaserType type;
    [SerializeField] private float healt = 10;

    [SerializeField] private BossController bossController;

    [Header("Laser Type 1")]
    [SerializeField] private float speed;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootingInterval = 2f;

    [Header("Laser Type 2")]
    [SerializeField] private float cooldownRotation;
    [SerializeField] private float cooldownScale;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;
    public bool isPhase1;

    private float nextShotTime;

    private GameObject fireLaserParent;
    private List<GameObject> laserPool;
    private Vector2 direction;

    private Transform playerTransform;
    private bool canRotate = true;
    private bool canScale = true;

    [SerializeField] private Transform trailLaserTransform;
    // Start is called before the first frame update
    void Start()
    {

        if (GameObject.FindWithTag("Player"))
        {
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }
        fireLaserParent = GameObject.Find("LaserParent");

        if (type == LaserType.Laser1)
        {
            CreateLaserPool();
        }
        else
        {
            if (trailLaserTransform != null)
            {
                trailLaserTransform.localScale = new Vector2(0.2f, 0);
            }
        }

    }
    void FixedUpdate()
    {
        if (isPhase1)
        {
            if (type == LaserType.Laser1)
            {
                AimPlayerLaser();
                HandleShooting();
            }
            else
            {
                StartCoroutine(RotateWithCooldown(cooldownRotation, minAngle, maxAngle));
                StartCoroutine(ScaleWithCooldown(cooldownScale, trailLaserTransform));
            }

            if (healt <= 0)
            {
                Destroy(this.gameObject);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (type == LaserType.Laser1)
        {
            if (other.CompareTag("MeleeDamage"))
            {
                healt -= 5;
            }
        }
        else
        {
            if (other.CompareTag("MeleeDamage"))
            {
                healt -= 3;
            }
        }
    }


    private IEnumerator RotateWithCooldown(float cooldownRotation, float minAngle, float maxAngle)
    {
        while (true)
        {
            if (canRotate)
            {
                canRotate = false;

                float currentZRotation = transform.eulerAngles.z;
                float targetRotation;

                if (Mathf.Approximately(currentZRotation, maxAngle))
                {
                    targetRotation = minAngle;
                }
                else
                {
                    targetRotation = maxAngle;
                }

                float elapsedTime = 0f;
                while (elapsedTime < cooldownRotation)
                {
                    transform.eulerAngles = new Vector3(
                        transform.eulerAngles.x,
                        transform.eulerAngles.y,
                        Mathf.Lerp(currentZRotation, targetRotation, elapsedTime / cooldownRotation)
                    );
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetRotation);

                yield return new WaitForSeconds(cooldownRotation);

                canRotate = true;
            }
            yield return null;
        }
    }



    private IEnumerator ScaleWithCooldown(float cooldownScale, Transform trailLaserTransform)
    {
        while (true)
        {
            if (canScale)
            {
                canScale = false;

                float scaleY = trailLaserTransform.localScale.y;
                float targetScale;

                if (Mathf.Approximately(scaleY, 1.3f))
                {
                    targetScale = 0f;
                }
                else
                {
                    targetScale = 1.3f;
                }

                float elapsedTime = 0f;
                while (elapsedTime < cooldownScale)
                {
                    trailLaserTransform.localScale = new Vector2(1.3f,
                        Mathf.Lerp(scaleY, targetScale, elapsedTime / cooldownScale)
                    );
                    if (trailLaserTransform.localScale.y < 0.6)
                    {
                        BoxCollider2D box2D = trailLaserTransform.GetComponent<BoxCollider2D>();
                        box2D.enabled = false;

                    }
                    else
                    {
                        BoxCollider2D box2D = trailLaserTransform.GetComponent<BoxCollider2D>();
                        box2D.enabled = true;
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                trailLaserTransform.localScale = new Vector2(1.3f, targetScale);

                yield return new WaitForSeconds(cooldownScale);

                canScale = true;
            }
            yield return null;
        }
    }



    // *------------------LASER TYEPE 1 LOGIG------------------------*
    void AimPlayerLaser()
    {
        if (playerTransform != null)
        {
            Vector2 playerPosition = playerTransform.position;
            direction = (playerPosition - (Vector2)transform.position).normalized;
            RotateTowardsDirection(direction);
        }
    }

    void RotateTowardsDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
    }

    private void CreateLaserPool()
    {
        // GameObject dronLaserParent = new GameObject(gameObject.name);
        // transform.parent = fireLaserParent.transform;

        laserPool = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            GameObject laser = Instantiate(laserPrefab, fireLaserParent.transform);
            laser.SetActive(false);
            laserPool.Add(laser);
        }
    }

    void HandleShooting()
    {
        if (playerTransform != null)
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
}
