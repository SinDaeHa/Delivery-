using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Prefaps List")]
    public GameObject[] carPrefabs;   // 여러 개의 Car 프리팹을 배열로 받음

    [Header("Locate Setting")]
    public float spawnY = 13f;
    public float[] laneX = new float[4] { -4.5f, -1.5f, 1.5f, 4.5f };

    private float timer = 0f;
    private float spawnDistance;
    private float obstacleSpeed;
    public bool canSpawn = true;


    void Start()
    {
        // 첫 Car 프리팹의 moveSpeed 가져오기
        Obstacle obs = carPrefabs[0].GetComponent<Obstacle>();
        if (obs != null)
            obstacleSpeed = obs.moveSpeed;

        // 기존 spawnDistance 계산 로직 유지
        SpriteRenderer sr = carPrefabs[0].GetComponent<SpriteRenderer>();
        float carHeight = sr.bounds.size.y;
        spawnDistance = carHeight * 3f;
    }

    void Update()
    {
        if (!canSpawn) return;    // 🔥 추가: 스폰 금지 상태라면 동작하지 않음

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
        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogWarning("Car 프리팹이 비어 있습니다.");
            return;
        }

        int obstacleCount = Random.Range(1, 4);
        List<int> indices = new List<int> { 0, 1, 2, 3 };

        for (int i = 0; i < obstacleCount; i++)
        {
            int randomIdx = Random.Range(0, indices.Count);
            int laneIndex = indices[randomIdx];
            indices.RemoveAt(randomIdx);

            // ★ Car 프리팹 랜덤 선택
            GameObject randomCar = carPrefabs[Random.Range(0, carPrefabs.Length)];

            Vector3 spawnPos = new Vector3(laneX[laneIndex], spawnY, 0f);

            // ★ Instantiate
            GameObject carObj = Instantiate(randomCar, spawnPos, Quaternion.identity);

            // ★ x축 반전
            SpriteRenderer sr = carObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipY = true; // 이미지 반전
            }
        }
    }
}
