using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Car Setting")]
    public float moveSpeed = 16f;
    public float destroyY = -17f;

    [Header("Hit Sound")]
    public AudioClip hitSound;      // 🔥 충돌 사운드
    public float hitSoundVolume = 1f;

    private float originalSpeed;
    private bool hasHitPlayer = false;   // 🔥 중복 재생 방지

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
        if (hasHitPlayer)
            return;

        if (other.CompareTag("Player"))
        {
            hasHitPlayer = true;

            // 🔊 충돌 사운드 재생
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    hitSound,
                    transform.position,
                    hitSoundVolume
                );
            }

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

    // -------------------------------------------------
    // 신호등 정지 / 재개
    // -------------------------------------------------
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
