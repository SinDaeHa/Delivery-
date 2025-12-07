using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Score Settings")]
    public int score = 0;

    public float scorePerSecond = 100f;  // 1초마다 100
    private float accumulatedTime = 0f;

    [Header("Bonus Scores")]
    public int fullHeartBonus = 100;     // 하트가 FULL일 때 HeartItem
    public int scoreItemAmount = 300;    // 점수 아이템
    public int obstacleHitPenalty = -300; // 장애물 충돌

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Tick 방식 점수 증가
        accumulatedTime += Time.deltaTime;
        float tickScore = scorePerSecond * Time.deltaTime;
        score += Mathf.RoundToInt(tickScore);

        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}
