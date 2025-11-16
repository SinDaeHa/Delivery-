using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("프리팹 & 위치")]
    public GameObject obstaclePrefab;  // Car 프리팹
    public float spawnY = 13f;          // 위에서 생성될 Y 위치
    public float[] laneX = new float[4] { -4.5f, -1.5f, 1.5f, 4.5f }; // 4개의 라인 X좌표

    [Header("스폰 설정")]
    public float spawnInterval = 2.0f; // 몇 초마다 한 번씩 웨이브 생성

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnWave();
            timer = 0f;
        }
    }

    void SpawnWave()
    {
        if (obstaclePrefab == null || laneX.Length < 4)
        {
            Debug.LogWarning("Spawner 설정이 잘못되었습니다.");
            return;
        }

        // 1~3개 장애물 생성 (Random.Range 의 int 버전은 max 미포함)
        int obstacleCount = Random.Range(1, 4); // 1, 2, 3 중 하나

        // 0,1,2,3 인덱스를 섞어서 그중 obstacleCount개만 사용
        List<int> indices = new List<int> { 0, 1, 2, 3 };

        for (int i = 0; i < obstacleCount; i++)
        {
            int randomIdx = Random.Range(0, indices.Count);
            int laneIndex = indices[randomIdx];
            indices.RemoveAt(randomIdx); // 같은 라인 중복 방지

            Vector3 spawnPos = new Vector3(laneX[laneIndex], spawnY, 0f);
            Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        }
        // 이렇게 하면 4개 라인 중 obstacleCount 개만 채워지므로,
        // 항상 적어도 1개 라인은 비어 있게 됨.
    }
}
