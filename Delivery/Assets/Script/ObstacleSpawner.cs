using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Car Prefabs")]
    public GameObject[] carPrefabs;

    [Header("Special Obstacles")]
    public GameObject busPrefab;
    public GameObject speedBumpPrefab;
    public GameObject manholePrefab;

    [Header("Spawn Enable (Runtime Auto Unlock)")]
    public bool spawnBus = false;
    public bool spawnSpeedBump = false;
    public bool spawnManhole = false;

    [Header("Unlock Time (Seconds Since Start)")]
    public float busUnlockTime = 20f;
    public float speedBumpUnlockTime = 40f;
    public float manholeUnlockTime = 60f;

    [Header("Spawn Settings")]
    public float spawnY = 17f;
    public float[] laneX = new float[4] { -4.5f, -1.5f, 1.5f, 4.5f };

    [Header("Spawn Term (Seconds)")]
    public float baseSpawnTerm = 1f;

    private float timer = 0f;
    private float elapsedTime = 0f;

    private bool busSpawnedLastWave = false;

    void Update()
    {
        // 🚦 빨간불이면 스폰 정지
        if (GameManager.Instance != null && GameManager.Instance.isTrafficRed)
            return;

        // -------------------------------
        // ⏱ 경과 시간 누적
        // -------------------------------
        elapsedTime += Time.deltaTime;

        // -------------------------------
        // 🔓 특수 장애물 해금
        // -------------------------------
        if (!spawnBus && elapsedTime >= busUnlockTime)
            spawnBus = true;

        if (!spawnSpeedBump && elapsedTime >= speedBumpUnlockTime)
            spawnSpeedBump = true;

        if (!spawnManhole && elapsedTime >= manholeUnlockTime)
            spawnManhole = true;

        // -------------------------------
        // ⏱ 스폰 타이머
        // -------------------------------
        timer += Time.deltaTime;

        float bonus = 0f;
        if (GameManager.Instance != null)
            bonus = GameManager.Instance.spawnTermBonus;

        float currentSpawnTerm = baseSpawnTerm + bonus;

        if (timer >= currentSpawnTerm)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    void SpawnWave()
    {
        List<int> lanes = new List<int> { 0, 1, 2, 3 };
        int obstacleCount = Random.Range(1, 4);

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

            Instantiate(
                prefab,
                new Vector3(laneX[laneIndex], spawnY, 0f),
                Quaternion.identity
            );
        }

        busSpawnedLastWave = busSpawnedThisWave;
    }

    GameObject ChooseObstacle(
        ref bool busSpawnedThisWave,
        ref bool speedBumpSpawnedThisWave,
        ref bool manholeSpawnedThisWave)
    {
        List<GameObject> list = new List<GameObject>();

        // 🚍 버스: 한 웨이브 1개 + 다음 웨이브 금지
        if (spawnBus && !busSpawnedLastWave && !busSpawnedThisWave)
            list.Add(busPrefab);

        // 🟨 과속방지턱: 한 웨이브 1개
        if (spawnSpeedBump && !speedBumpSpawnedThisWave)
            list.Add(speedBumpPrefab);

        // 🕳 맨홀: 한 웨이브 1개
        if (spawnManhole && !manholeSpawnedThisWave)
            list.Add(manholePrefab);

        // 🚗 기본 차량은 항상 등장
        foreach (var car in carPrefabs)
            list.Add(car);

        if (list.Count == 0)
            return null;

        GameObject pick = list[Random.Range(0, list.Count)];

        if (pick == busPrefab)
            busSpawnedThisWave = true;
        else if (pick == speedBumpPrefab)
            speedBumpSpawnedThisWave = true;
        else if (pick == manholePrefab)
            manholeSpawnedThisWave = true;

        return pick;
    }
}
