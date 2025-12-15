using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("Progress Bar")]
    public RectTransform progressBarFill;
    public float stageDuration = 9999999999f;
    private float stageTimer = 0f;

    [Header("Fade Panel")]
    public Image fadePanel;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public float fadeInWait = 0.5f;
    public float fadeOutWait = 1.5f;

    [Header("Stage Settings")]
    public string nextStageName = "";
    private bool stageCleared = false;

    private bool isGameOver = false;

    // ============================================================
    // 🔥 난이도 증가 관련
    // ============================================================
    [Header("Difficulty Scaling")]
    public float difficultyTickInterval = 6f;       // 6초
    public float speedIncreasePerTick = 0.8f;       // 적 속도 증가
    public float spawnTermIncreasePerTick = -0.03f;   // 스폰텀 증가

    private float difficultyTimer = 0f;

    [HideInInspector] public float enemySpeedBonus = 0f;
    [HideInInspector] public float spawnTermBonus = 0f;

    // 🚦 신호등 상태
    [HideInInspector] public bool isTrafficRed = false;

    void Start()
    {
        if (fadePanel != null)
            StartCoroutine(StartFadeInRoutine());
    }

    void Update()
    {
        if (isGameOver || stageCleared)
            return;

        // -------------------------------
        // 스테이지 타이머
        // -------------------------------
        stageTimer += Time.deltaTime;

        float t = Mathf.Clamp01(stageTimer / stageDuration);
        if (progressBarFill != null)
            progressBarFill.localScale = new Vector3(t, 1f, 1f);

        if (stageTimer >= stageDuration)
        {
            stageCleared = true;
            StartCoroutine(StageClearRoutine());
        }

        // -------------------------------
        // 🚦 빨간불이면 난이도 증가 멈춤
        // -------------------------------
        if (isTrafficRed)
            return;

        // -------------------------------
        // 난이도 증가 (틱 방식)
        // -------------------------------
        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyTickInterval)
        {
            difficultyTimer -= difficultyTickInterval;

            enemySpeedBonus += speedIncreasePerTick;
            spawnTermBonus += spawnTermIncreasePerTick;
        }
    }

    // ============================================================
    // 페이드 인
    // ============================================================
    IEnumerator StartFadeInRoutine()
    {
        Color c = fadePanel.color;
        fadePanel.color = new Color(c.r, c.g, c.b, 1f);

        yield return new WaitForSecondsRealtime(fadeInWait);

        float time = 0f;
        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, 0f);
    }

    // ============================================================
    // 스테이지 종료
    // ============================================================
    IEnumerator StageClearRoutine()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.DisableCollisionForStageEnd();
            player.StartBlinking(fadeDuration + fadeOutWait);
        }

        yield return StartCoroutine(FadeOutRoutine());
        yield return new WaitForSecondsRealtime(fadeOutWait);

        if (!string.IsNullOrEmpty(nextStageName))
            SceneManager.LoadScene(nextStageName);
    }

    IEnumerator FadeOutRoutine()
    {
        Color c = fadePanel.color;
        float time = 0f;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, 1f);
    }

    // ============================================================
    // 게임 오버
    // ============================================================
    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }
}
