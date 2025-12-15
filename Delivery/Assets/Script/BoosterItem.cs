using UnityEngine;

public class BoosterItem : MonoBehaviour
{

    [Header("Item Move Setting")]
    public float speedMultiplier = 0.25f;  
    public float minX = -5.5f;
    public float maxX = 5.5f;

    private float baseSpeed;
    private float horizontalDir;   // -1 또는 +1

    [Header("Boost Settings")]
    public float boostMultiplier = 2f;      // 이동속도 증가 비율
    public float boostDuration = 3f;        // 이동속도 증가 지속시간

    [Header("Invincibility Settings")]
    public bool giveInvincibility = false;  // 무적 활성화 여부
    public float invincibleDuration = 5f;   // 무적 지속시간

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
            PlayerController pc = collision.GetComponent<PlayerController>();
            if (pc != null)
            {
                // 이동속도 증가
                pc.TriggerCustomSpeedBoost(boostMultiplier, boostDuration);

                // 무적 활성화
                if (giveInvincibility)
                    pc.TriggerInvincibleCustom(invincibleDuration);
            }

            Destroy(gameObject);
        }
    }
}
