using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Button Image")]
    public Image buttonImage;
    public Sprite btnNormal;
    public Sprite btnPressed;

    [Header("Fade Panel")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("BGM")]
    public AudioSource bgmSource;
    public float bgmFadeDuration = 1f;
    public float bgmMaxVolume = 1f;

    [Header("Button Click SFX")]
    public AudioSource sfxSource;
    public AudioClip clickSfx;

    [Header("Scene")]
    public string storySceneName = "Story";

    private bool isClicked = false;

    void Start()
    {
        // 🎵 BGM 페이드 인
        if (bgmSource != null)
        {
            bgmSource.volume = 0f;
            bgmSource.Play();
            StartCoroutine(FadeBGM(0f, bgmMaxVolume, bgmFadeDuration));
        }

        // 🌑 화면 페이드 인
        if (fadePanel != null)
            StartCoroutine(FadeInRoutine());
    }

    public void OnClickStart()
    {
        if (isClicked) return;
        isClicked = true;

        // 🔊 버튼 클릭 효과음
        if (sfxSource != null && clickSfx != null)
            sfxSource.PlayOneShot(clickSfx);

        StartCoroutine(ButtonSequence());
    }

    IEnumerator ButtonSequence()
    {
        // 버튼 눌림 이미지
        if (buttonImage != null && btnPressed != null)
            buttonImage.sprite = btnPressed;

        yield return new WaitForSeconds(0.1f);

        if (buttonImage != null && btnNormal != null)
            buttonImage.sprite = btnNormal;

        // 🎵 BGM 페이드 아웃
        if (bgmSource != null)
            StartCoroutine(FadeBGM(bgmSource.volume, 0f, bgmFadeDuration));

        // 🌑 화면 페이드 아웃
        yield return StartCoroutine(FadeOutRoutine());

        SceneManager.LoadScene(storySceneName);
    }

    // ------------------------------------------------
    // 🎵 BGM Fade
    // ------------------------------------------------
    IEnumerator FadeBGM(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            bgmSource.volume = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        bgmSource.volume = to;
    }

    // ------------------------------------------------
    // 🌑 Fade In / Out
    // ------------------------------------------------
    IEnumerator FadeInRoutine()
    {
        Color c = fadePanel.color;
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
}
