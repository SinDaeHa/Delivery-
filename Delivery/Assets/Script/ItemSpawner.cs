using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Item Prefab")]
    public GameObject heartItemPrefab;

    [Header("Spawn Position")]
    public float spawnY = 13f;  // 화면 위에서 등장
    public float[] laneX = new float[4] { -4.5f, -1.5f, 1.5f, 4.5f };

    [Header("Spawn Interval Setting (Random)")]
    public float minSpawnTime = 8f;   // 최소 쿨타임
    public float maxSpawnTime = 16f;  // 최대 쿨타임

    private float nextSpawnTime = 0f;

    void Start()
    {
        // 첫 스폰 시간 랜덤 설정
        nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Update()
    {
        nextSpawnTime -= Time.deltaTime;

        if (nextSpawnTime <= 0f)
        {
            SpawnItem();

            // 다음 스폰 시간 다시 랜덤 설정
            nextSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    void SpawnItem()
    {
        if (heartItemPrefab == null) return;

        int lane = Random.Range(0, laneX.Length);

        Vector3 pos = new Vector3(laneX[lane], spawnY, 0f);

        Instantiate(heartItemPrefab, pos, Quaternion.identity);
    }
}
