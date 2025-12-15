using UnityEngine;

public class ScoreItem : MonoBehaviour
{
    [Header("Item Move Setting")]
    public float speedMultiplier = 0.25f;  
    public float minX = -5.5f;
    public float maxX = 5.5f;

    private float baseSpeed;
    private float horizontalDir;   // -1 또는 +1

    void Start()
    {
        
        baseSpeed = 24f; // fallback
        
        // 처음 좌우 방향 랜덤
        horizontalDir = Random.value < 0.5f ? -1f : 1f;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // 🔥 Y축 이동도 multiplier 적용!
        pos.y -= baseSpeed * speedMultiplier * Time.deltaTime;

        // 🔥 X축 이동도 동일하게 multiplier 적용
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(ScoreManager.Instance.scoreItemAmount);
            Destroy(gameObject);
        }
    }
}
