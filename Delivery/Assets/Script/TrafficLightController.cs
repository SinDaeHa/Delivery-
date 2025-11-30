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

    private bool isRed = false;
    private bool playerMoved = false;

    private PlayerController player;
    private HeartSystem heartSystem;
    private ObstacleSpawner spawner;
    private AutoLoopBackground bg;   // 🔥 여기 변경됨 (중요!)

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        heartSystem = FindObjectOfType<HeartSystem>();
        spawner = FindObjectOfType<ObstacleSpawner>();

        // 🔥 새로운 배경 스크립트
        bg = FindObjectOfType<AutoLoopBackground>();

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
            playerMoved = false;

            if (spawner != null) 
                spawner.canSpawn = true;

            FreezeObstacles(false);

            if (bg != null)
                bg.SetRedLight(false);  // 🔥 배경 움직임 ON

            yield return new WaitForSeconds(greenTime);

            // ---------------- YELLOW ----------------
            ShowLight("YELLOW");

            isRed = false;

            if (bg != null)
                bg.SetRedLight(false); // 움직임 계속

            yield return new WaitForSeconds(yellowTime);

            // ---------------- RED ----------------
            float redTime = Random.Range(redMinTime, redMaxTime);
            ShowLight("RED");

            isRed = true;
            playerMoved = false;

            if (spawner != null)
                spawner.canSpawn = false;

            FreezeObstacles(true);

            if (bg != null)
                bg.SetRedLight(true);  // 🔥 배경 멈춤

            float timer = 0f;

            while (timer < redTime)
            {
                timer += Time.deltaTime;

                if (!playerMoved && PlayerTryingToMove())
                {
                    playerMoved = true;

                    if (heartSystem != null)
                        heartSystem.TakeDamage(null);

                    player.TriggerSlowdown();
                    player.TriggerInvincible();
                }

                yield return null;
            }
        }
    }

    void ShowLight(string type)
    {
        lightGreen.SetActive(type == "GREEN");
        lightYellow.SetActive(type == "YELLOW");
        lightRed.SetActive(type == "RED");
    }

    void FreezeObstacles(bool stop)
    {
        Obstacle[] all = FindObjectsOfType<Obstacle>();
        foreach (var obs in all)
        {
            if (stop) obs.PauseSpeed();
            else obs.ResumeSpeed();
        }
    }

    bool PlayerTryingToMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        return Mathf.Abs(x) > 0.1f;
    }
}
