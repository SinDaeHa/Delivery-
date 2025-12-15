using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryController : MonoBehaviour
{
    [Header("Story Images")]
    public Image storyImage;
    public Sprite[] storySprites;   // 5장 이미지

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    [Header("Next Scene")]
    public string nextSceneName = "Stage_1";

    [Header("Interaction Sound")]
    public AudioClip nextSound;
    public float nextSoundVolume = 1f;

    private int currentIndex = 0;
    private bool isTransitioning = false;

    void Start()
    {
        // 첫 이미지 표시
        if (storySprites.Length > 0)
            storyImage.sprite = storySprites[0];

        // 페이드 인
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (isTransitioning)
            return;

        // 화면 클릭 → 다음 이미지
        if (Input.GetMouseButtonDown(0))
        {
            NextImage();
        }
    }

    // -------------------------------
    // 이미지 진행
    // -------------------------------
    void NextImage()
    {
        currentIndex++;

        if (currentIndex >= storySprites.Length)
        {
            // 마지막 이미지 이후 → Stage_1
            StartCoroutine(LoadNextScene());
        }
        else
        {
            storyImage.sprite = storySprites[currentIndex];
            if (nextSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    nextSound,
                    transform.position,
                    nextSoundVolume
                );
            }
        }
    }

    // -------------------------------
    // Skip 버튼용
    // -------------------------------
    public void OnClickSkip()
    {  
        Debug.Log("SKIP CLICKED");
        if (isTransitioning) return;
        StartCoroutine(LoadNextScene());
    }

    // -------------------------------
    // 씬 전환
    // -------------------------------
    IEnumerator LoadNextScene()
    {
        isTransitioning = true;

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(nextSceneName);
    }

    // -------------------------------
    // Fade
    // -------------------------------
    IEnumerator FadeIn()
    {
        float time = 0f;
        Color c = fadePanel.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            fadePanel.color = new Color(c.r, c.g, c.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadePanel.color = new Color(c.r, c.g, c.b, 0f);
    }

    IEnumerator FadeOut()
    {
        float time = 0f;
        Color c = fadePanel.color;

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
