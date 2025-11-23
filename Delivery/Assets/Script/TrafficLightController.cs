using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    [Header("Signal Objects")]
    public GameObject lightGreen;
    public GameObject lightYellow;
    public GameObject lightRed;

    [Header("Green Time (Random Range)")]
    public float greenMinTime = 30f;
    public float greenMaxTime = 40f;

    [Header("Yellow Time")]
    public float yellowTime = 3f;

    [Header("Red Time (Random Range)")]
    public float redMinTime = 4f;
    public float redMaxTime = 6f;

    private bool isRed = false;         // 현재 빨간불 상태인가?
    private bool playerMoved = false;   // 빨간불 상태에서 플레이어가 움직였나?

    private PlayerController player;
    private HeartSystem heartSystem;
    private ObstacleSpawner spawner;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        heartSystem = FindObjectOfType<HeartSystem>();
        spawner = FindObjectOfType<ObstacleSpawner>();

        StartCoroutine(LightCycleRoutine());
    }

    IEnumerator LightCycleRoutine()
    {
        while (true)
        {
            // ---------------- GREEN ----------------
            float greenTime = Random.Range(greenMinTime, greenMaxTime);
            ShowLight("GREEN");
            isRed = false;

            // 스폰 재개
            if (spawner != null)
                spawner.canSpawn = true;

            // 멈춰있던 적 다시 움직이기
            FreezeObstacles(false);

            yield return new WaitForSeconds(greenTime);

            // ---------------- YELLOW ----------------
            ShowLight("YELLOW");
            isRed = false;

            yield return new WaitForSeconds(yellowTime);

            // ---------------- RED ----------------
            float redTime = Random.Range(redMinTime, redMaxTime);
            ShowLight("RED");
            isRed = true;
            playerMoved = false;

            // 장애물 스폰 금지
            if (spawner != null)
                spawner.canSpawn = false;

            // 현재 존재하는 적들 모두 멈추기
            FreezeObstacles(true);

            // 빨간불 유지 시간
            float timer = 0f;
            while (timer < redTime)
            {
                timer += Time.deltaTime;

                // 빨간불 동안 플레이어 움직임 감지
                if (!playerMoved && PlayerTryingToMove())
                {
                    playerMoved = true;

                    // 벌칙: 충돌 처리와 동일
                    if (heartSystem != null)
                        heartSystem.TakeDamage(null);  // null = 충돌한 적 없음

                    player.TriggerSlowdown();
                    player.TriggerInvincible();
                }

                yield return null;
            }

            // 빨간불 종료 → 루프 반복
        }
    }

    // ---------------- LIGHT CONTROL ----------------

    void ShowLight(string type)
    {
        lightGreen.SetActive(type == "GREEN");
        lightYellow.SetActive(type == "YELLOW");
        lightRed.SetActive(type == "RED");
    }

    // ---------------- OBSTACLE FREEZE ----------------

    void FreezeObstacles(bool stop)
    {
        Obstacle[] all = FindObjectsOfType<Obstacle>();

        foreach (var obs in all)
        {
            if (stop)
                obs.PauseSpeed();
            else
                obs.ResumeSpeed();
        }
    }

    // ---------------- PLAYER MOVEMENT CHECK ----------------

    bool PlayerTryingToMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        return Mathf.Abs(x) > 0.1f;
    }
}
