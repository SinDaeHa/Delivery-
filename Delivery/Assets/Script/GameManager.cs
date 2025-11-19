using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public RectTransform progressBarFill;   //진척도 바
    public float stageDuration = 120f;      //스테이지 길이: 2분
    float stageTimer = 0f;
    private bool isGameOver = false;

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // UI 켜기
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // 게임 멈추기
        Time.timeScale = 0f;

        Debug.Log("Game Over UI 띄움");
    }

    void Update()
{
    // 게임오버 전까지는 시간 흐름
    if (!isGameOver)
    {
        stageTimer += Time.deltaTime;

        float t = Mathf.Clamp01(stageTimer / stageDuration); // 0~1

        if (progressBarFill != null)
        {
            progressBarFill.localScale = new Vector3(t, 1f, 1f);
        }

        // 2분 다 찼을 때 스테이지 클리어 처리
        if (stageTimer >= stageDuration)
        {
            Debug.Log("Stage Clear!");
        }
    }

    //재시작 코드
    if (isGameOver && Input.GetKeyDown(KeyCode.R))
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

}
