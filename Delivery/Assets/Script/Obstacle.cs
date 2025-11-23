using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Car Setting")]
    public float moveSpeed = 4f;
    public float destroyY = -13f;
    private float originalSpeed;

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    public void PauseSpeed()
    {
        originalSpeed = moveSpeed;
        moveSpeed = 0f;
    }

    public void ResumeSpeed()
    {
        moveSpeed = originalSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 부딪혔을 때만 처리
        if (other.CompareTag("Player"))
        {
            HeartSystem hs = FindObjectOfType<HeartSystem>();

            if (hs != null)
            {
                // HeartSystem이 적을 제거하고 하트 처리까지 한다
                hs.TakeDamage(gameObject);
            }
        }
    }
}