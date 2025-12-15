using UnityEngine;

public class HeartItem : MonoBehaviour
{
    [Header("Item Move Setting")]
    public float speedMultiplier = 0.25f;  
    public float minX = -5.5f;
    public float maxX = 5.5f;

    private float baseSpeed;
    private float horizontalDir;

    void Start()
    {
        baseSpeed = 24f;
        horizontalDir = Random.value < 0.5f ? -1f : 1f;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        pos.y -= baseSpeed * speedMultiplier * Time.deltaTime;
        pos.x += horizontalDir * baseSpeed * speedMultiplier * Time.deltaTime;

        if (pos.x < minX)
        {
            pos.x = minX;
            horizontalDir = 1;
        }
        else if (pos.x > maxX)
        {
            pos.x = maxX;
            horizontalDir = -1;
        }

        transform.position = pos;

        if (pos.y < -13f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HeartSystem hs = FindObjectOfType<HeartSystem>();
            if (hs != null)
            {
                // ⭐ 먹기 "직전에" 풀하트인지 검사
                bool wasFull = hs.IsFullHeart();

                // ❤️ 기존 하트 증가 수행
                hs.AddHeart();

                // ⭐ 풀하트였다면 점수 증가
                if (wasFull)
                {
                    if (ScoreManager.Instance != null)
                        ScoreManager.Instance.AddScore(ScoreManager.Instance.fullHeartBonus);
                }
            }

            Destroy(gameObject);
        }
    }
}
