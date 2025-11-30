using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject[] itemPrefabs;

    [Header("Spawn Settings")]
    public float minSpawnTime = 5f;
    public float maxSpawnTime = 12f;

    public float spawnXMin = -4.5f;
    public float spawnXMax = 4.5f;
    public float spawnY = 13f;

    private float timer = 0f;
    private float spawnDelay;

    void Start()
    {
        SetRandomDelay();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            SpawnItem();
            SetRandomDelay();
            timer = 0f;
        }
    }

    void SetRandomDelay()
    {
        spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void SpawnItem()
    {
        if (itemPrefabs.Length == 0)
            return;

        GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

        float x = Random.Range(spawnXMin, spawnXMax);

        Instantiate(prefab, new Vector3(x, spawnY, 0f), Quaternion.identity);
    }
}
