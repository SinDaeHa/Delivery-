using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float destroyY = -13f;

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어와 충돌! 게임 오버 처리");

            // 간단한 게임 오버 예시
            Time.timeScale = 0f; // 게임 멈추기

            // 나중에 GameManager를 만들면 이렇게 바꿀 수 있음:
            // FindObjectOfType<GameManager>().GameOver();
        }
    }
}