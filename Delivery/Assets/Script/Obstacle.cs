using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Car Setting")]
    public float moveSpeed = 16f;
    public float destroyY = -17f;

    private bool isPaused = false;

    void Update()
    {
        // 🚦 빨간불일 때 완전 정지
        if (isPaused)
            return;

        float bonusSpeed = 0f;
        if (GameManager.Instance != null)
            bonusSpeed = GameManager.Instance.enemySpeedBonus;

        transform.Translate(
            Vector3.down * (moveSpeed + bonusSpeed) * Time.deltaTime
        );

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            HeartSystem hs = FindObjectOfType<HeartSystem>();

            OnPlayerHit(player, hs);
        }
    }

    // ============================================================
    // 🔥 기본 장애물 충돌 처리 (자동차)
    // ============================================================
    protected virtual void OnPlayerHit(PlayerController player, HeartSystem hs)
    {
        if (hs != null)
            hs.TakeDamage(gameObject);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(ScoreManager.Instance.obstacleHitPenalty);
    }

    // ============================================================
    // 🚦 신호등 제어용
    // ============================================================
    public void PauseSpeed()
    {
        isPaused = true;
    }

    public void ResumeSpeed()
    {
        isPaused = false;
    }
}
