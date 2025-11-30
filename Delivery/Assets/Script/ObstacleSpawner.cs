using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Car Prefabs (Default Cars)")]
    public GameObject[] carPrefabs;

    [Header("New Obstacle Prefabs")]
    public GameObject busPrefab;
    public GameObject speedBumpPrefab;
    public GameObject manholePrefab;

    [Header("Toggle Spawn Types")]
    public bool spawnBus = true;
    public bool spawnSpeedBump = true;
    public bool spawnManhole = true;

    [Header("Spawn Settings")]
    public float spawnY = 17f;
    public float[] laneX = new float[4] { -4.5f, -1.5f, 1.5f, 4.5f };
    public float SpawnTerm = 3f;

    private float timer = 0f;
    private float spawnDistance;
    private float obstacleSpeed;

    public bool canSpawn = true;

    // 🔥 버스만 다음 웨이브 등장 금지
    private bool busSpawnedLastWave = false;

    void Start()
    {
        Obstacle obs = carPrefabs[0].GetComponent<Obstacle>();
        if (obs != null)
            obstacleSpeed = obs.moveSpeed;

        SpriteRenderer sr = carPrefabs[0].GetComponent<SpriteRenderer>();
        float carHeight = sr.bounds.size.y;

        spawnDistance = carHeight * SpawnTerm;
    }

    void Update()
    {
        if (!canSpawn) return;

        timer += Time.deltaTime;
        float moved = timer * obstacleSpeed;

        if (moved >= spawnDistance)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    void SpawnWave()
    {
        List<int> lanes = new List<int> { 0, 1, 2, 3 };
        int obstacleCount = Random.Range(1, 4);

        // 🔥 이번 웨이브 출현 여부
        bool busSpawnedThisWave = false;
        bool speedBumpSpawnedThisWave = false;
        bool manholeSpawnedThisWave = false;

        for (int i = 0; i < obstacleCount; i++)
        {
            if (lanes.Count == 0)
                break;

            int randomIdx = Random.Range(0, lanes.Count);
            int laneIndex = lanes[randomIdx];
            lanes.RemoveAt(randomIdx);

            GameObject prefab = ChooseObstacle(
                ref busSpawnedThisWave,
                ref speedBumpSpawnedThisWave,
                ref manholeSpawnedThisWave
            );

            if (prefab == null)
                continue;

            Vector3 pos = new Vector3(laneX[laneIndex], spawnY, 0f);
            Instantiate(prefab, pos, Quaternion.identity);
        }

        // 🔥 버스만 다음 웨이브 금지
        busSpawnedLastWave = busSpawnedThisWave;
    }

    GameObject ChooseObstacle(
        ref bool busWave,
        ref bool speedBumpWave,
        ref bool manholeWave
    )
    {
        List<GameObject> list = new List<GameObject>();

        // 🔥 버스: 한 웨이브 1회 제한 + 다음웨이브도 금지
        if (spawnBus && !busSpawnedLastWave && !busWave)
            list.Add(busPrefab);

        // 🔥 스피드범프: 한 웨이브 1회만
        if (spawnSpeedBump && !speedBumpWave)
            list.Add(speedBumpPrefab);

        // 🔥 맨홀: 한 웨이브 1회만
        if (spawnManhole && !manholeWave)
            list.Add(manholePrefab);

        // 🔥 기본 자동차는 제한 없음
        foreach (var car in carPrefabs)
            list.Add(car);

        if (list.Count == 0)
            return null;

        GameObject pick = list[Random.Range(0, list.Count)];

        // 🔥 이번 웨이브 출현 기록
        if (pick == busPrefab) busWave = true;
        if (pick == speedBumpPrefab) speedBumpWave = true;
        if (pick == manholePrefab) manholeWave = true;

        return pick;
    }
}
