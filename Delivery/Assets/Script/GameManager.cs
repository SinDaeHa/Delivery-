using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverUI;

    [Header("Progress Bar")]
    public RectTransform progressBarFill;
    public float stageDuration = 120f;
    private float stageTimer = 0f;

    [Header("Fade Panel")]
    public Image fadePanel;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;     // 페이드 인/아웃 시간
    public float fadeInWait = 0.5f;     // 씬 시작 시 검은 화면 유지 시간
    public float fadeOutWait = 1.5f;    // 씬 종료 후 검은 화면 유지 시간

    [Header("Stage Settings")]
    public string nextStageName = "";   // 다음 씬 이름(없으면 마지막 스테이지)
    private bool stageCleared = false;

    private bool isGameOver = false;

    void Start()
    {
        // 씬 시작 시 페이드 인 실행
        if (fadePanel != null)
            StartCoroutine(StartFadeInRoutine());
    }

    void Update()
    {
        if (isGameOver) return;
        if (stageCleared) return;

        // 스테이지 타이머 증가
        stageTimer += Time.deltaTime;

        // 진행 바 채우기
        float t = Mathf.Clamp01(stageTimer / stageDuration);
        if (progressBarFill != null)
            progressBarFill.localScale = new Vector3(t, 1f, 1f);

        // 스테이지 종료
        if (stageTimer >= stageDuration)
        {
            stageCleared = true;
            StartCoroutine(StageClearRoutine());
        }
    }

    // ============================================================
    // 🔥 씬 시작 페이드 인
    // ============================================================
    IEnumerator StartFadeInRoutine()
    {
        Color c = fadePanel.color;

        // 1) 시작 시 검은 화면 유지
        fadePanel.color = new Color(c.r, c.g, c.b, 1f);
        yield return new WaitForSecondsRealtime(fadeInWait);

        // 2) 페이드 인
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
    // 🔥 스테이지 종료 → 페이드 아웃 → 다음 씬으로 전환
    // ============================================================
    IEnumerator StageClearRoutine()
    {
        Time.timeScale = 1f; // 혹시 모를 GameOver 잔여 상태 방지

        // 🔥 플레이어 충돌 OFF + 깜빡임
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.DisableCollisionForStageEnd();
            player.StartBlinking(fadeDuration + fadeOutWait);
        }

        // 1) 페이드 아웃
        yield return StartCoroutine(FadeOutRoutine());

        // 2) 검은 화면 유지
        yield return new WaitForSecondsRealtime(fadeOutWait);

        // 3) 다음 스테이지 로드
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
    // 🔥 게임 오버 처리
    // ============================================================
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;
    }
}
