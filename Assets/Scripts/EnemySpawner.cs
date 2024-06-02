using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject dronPrefab;
    [SerializeField] private GameObject microPrefab;
    [SerializeField] private int initialSpawnCount = 5;
    [SerializeField] private float respawnDelay = 1f;

    private List<GameObject> dronPool;
    private List<GameObject> microPool;
    private List<Transform> spawnPoints;
    private int dronParentIndex = 0;
    private int microParentIndex = 0;
    private bool allEnemiesDefeated = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Initialize pools and spawn points
        dronPool = new List<GameObject>();
        microPool = new List<GameObject>();
        spawnPoints = new List<Transform>();

        Transform spawnPointParent = GameObject.FindGameObjectWithTag("SpawnpointEnemy").transform;
        foreach (Transform child in spawnPointParent)
        {
            spawnPoints.Add(child);
        }

        InitializePool(dronPrefab, dronPool, "DronParent", 15);
        InitializePool(microPrefab, microPool, "MicroParent", 15);

        StartCoroutine(SpawnEnemies(initialSpawnCount));
    }

    private void InitializePool(GameObject prefab, List<GameObject> pool, string parentTag, int poolSize)
    {
        Transform parent = GameObject.FindGameObjectWithTag(parentTag).transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject instance = Instantiate(prefab, parent);
            instance.SetActive(false);
            pool.Add(instance);
        }
    }

    private IEnumerator SpawnEnemies(int spawnCount)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            ActivateEnemy(dronPool, dronParentIndex);
            dronParentIndex = (dronParentIndex + 1) % dronPool.Count;

            ActivateEnemy(microPool, microParentIndex);
            microParentIndex = (microParentIndex + 1) % microPool.Count;

            if (i >= 10)
            {
                yield return new WaitForSeconds(respawnDelay);
            }
        }

        allEnemiesDefeated = false;
        StartCoroutine(CheckEnemiesDefeated());
    }

    private void ActivateEnemy(List<GameObject> pool, int index)
    {
        if (!pool[index].activeInHierarchy)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            pool[index].transform.position = spawnPoint.position;
            pool[index].SetActive(true);
        }
    }

    private IEnumerator CheckEnemiesDefeated()
    {
        while (!allEnemiesDefeated)
        {
            yield return new WaitForSeconds(1f);

            allEnemiesDefeated = true;
            foreach (GameObject dron in dronPool)
            {
                if (dron.activeInHierarchy)
                {
                    allEnemiesDefeated = false;
                    break;
                }
            }

            if (allEnemiesDefeated)
            {
                foreach (GameObject micro in microPool)
                {
                    if (micro.activeInHierarchy)
                    {
                        allEnemiesDefeated = false;
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(respawnDelay);
        initialSpawnCount++;
        StartCoroutine(SpawnEnemies(initialSpawnCount));
    }

    public void TriggerRespawn()
    {
        StopAllCoroutines();
        foreach (GameObject dron in dronPool)
        {
            dron.SetActive(false);
        }
        foreach (GameObject micro in microPool)
        {
            micro.SetActive(false);
        }
        StartCoroutine(SpawnEnemies(initialSpawnCount));
    }
}
