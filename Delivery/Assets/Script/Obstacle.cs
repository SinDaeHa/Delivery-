using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Car Setting")]
    public float moveSpeed = 4f;
    public float destroyY = -17f;

    private float originalSpeed;

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
            PlayerController player = other.GetComponent<PlayerController>();
            HeartSystem hs = FindObjectOfType<HeartSystem>();

            OnPlayerHit(player, hs);
        }
    }

    // 🔥 각 장애물이 override하도록 설계
    protected virtual void OnPlayerHit(PlayerController player, HeartSystem hs)
    {
        // 기본 장애물(자동차)은 하트 감소
        if (hs != null)
            hs.TakeDamage(gameObject);
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
}
