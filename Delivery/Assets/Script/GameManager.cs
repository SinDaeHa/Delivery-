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

    [Header("Scene Names")]
    public string currentStageSceneName = "Stage_1";
    public string mainMenuSceneName = "MainMenu";

    private bool isGameOver = false;

    // ============================================================
    // 🔥 BGM 설정
    // ============================================================
    [Header("Stage BGM")]
    public AudioSource bgmSource;
    public AudioClip stageBgm;
    public float bgmFadeInTime = 1.5f;
    public float bgmVolume = 1f;   // Inspector에서 조절

    // ============================================================
    // 🔥 난이도 증가 관련
    // ============================================================
    [Header("Difficulty Scaling")]
    public float difficultyTickInterval = 6f;
    public float speedIncreasePerTick = 0.8f;
    public float spawnTermIncreasePerTick = -0.025f;

    private float difficultyTimer = 0f;

    [HideInInspector] public float enemySpeedBonus = 0f;
    [HideInInspector] public float spawnTermBonus = 0f;

    [HideInInspector] public bool isTrafficRed = false;

    void Start()
    {
        if (fadePanel != null)
            StartCoroutine(StartFadeInRoutine());

        PlayStageBGM();
    }

    void Update()
    {
        // 🔥 추가: 실시간 볼륨 반영
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartStage();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }

        if (isGameOver)
            return;

        stageTimer += Time.deltaTime;

        float t = Mathf.Clamp01(stageTimer / stageDuration);
        if (progressBarFill != null)
            progressBarFill.localScale = new Vector3(t, 1f, 1f);

        if (!stageCleared && stageTimer >= stageDuration)
        {
            stageCleared = true;
            StartCoroutine(StageClearRoutine());
        }

        if (isTrafficRed)
            return;

        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyTickInterval)
        {
            difficultyTimer -= difficultyTickInterval;
            enemySpeedBonus += speedIncreasePerTick;
            spawnTermBonus += spawnTermIncreasePerTick;
        }
    }

    // ============================================================
    // BGM 제어
    // ============================================================
    void PlayStageBGM()
    {
        if (bgmSource == null || stageBgm == null)
            return;

        bgmSource.clip = stageBgm;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        StartCoroutine(FadeInBGM());
    }

    IEnumerator FadeInBGM()
    {
        float timer = 0f;

        while (timer < bgmFadeInTime)
        {
            timer += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, timer / bgmFadeInTime);
            yield return null;
        }

        bgmSource.volume = bgmVolume;
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
            time += Time.unscaledDeltaTime;
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
            time += Time.unscaledDeltaTime;
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

        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        Time.timeScale = 0f;
    }

    void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentStageSceneName);
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
