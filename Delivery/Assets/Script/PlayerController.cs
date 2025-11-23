using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Moving Setting")]
    public float moveSpeed = 10f;
    public float minX = -4.75f;
    public float maxX = 4.75f;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite moveSprite;

    [Header("Invincible Time Setting")]
    public float invincibleTime = 3f;

    [Header("Hit Slowdown Setting")]
    public float slowMultiplier = 0.5f;   // 기본 이동속도 50%로 감소
    public float slowDuration = 2f;       // 기본 감속 지속시간 2초

    private SpriteRenderer sr;
    private Collider2D col;

    private bool isInvincible = false;
    private bool isSlowed = false;

    private float baseMoveSpeed; // 원래 속도 저장

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        baseMoveSpeed = moveSpeed;
        sr.sprite = idleSprite;
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");

        // 이동
        float currentSpeed = moveSpeed;
        Vector3 pos = transform.position;
        pos.x += input * currentSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

        // 스프라이트 변경
        UpdateSprite(input);
    }

    void UpdateSprite(float input)
    {
        if (input == 0)
        {
            sr.sprite = idleSprite;
            sr.flipX = false;
        }
        else if (input < 0)
        {
            sr.sprite = moveSprite;
            sr.flipX = false;
        }
        else
        {
            sr.sprite = moveSprite;
            sr.flipX = true;
        }
    }

    // --------------------------------------------------------
    // 🔥 플레이어 무적 + 깜빡임
    // --------------------------------------------------------
    public void TriggerInvincible()
    {
        if (!isInvincible)
            StartCoroutine(InvincibleRoutine(invincibleTime));
    }

    private IEnumerator InvincibleRoutine(float duration)
    {
        isInvincible = true;
        col.enabled = false;

        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;
            sr.enabled = visible;

            float interval = 0.15f;
            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        sr.enabled = true;
        col.enabled = true;
        isInvincible = false;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    // --------------------------------------------------------
    //  🔥 피격 시 이동속도 감소 효과
    // --------------------------------------------------------
    public void TriggerSlowdown()
    {
        if (!isSlowed)
            StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        isSlowed = true;

        // 이동속도 감소: 예) 50%면 0.5배
        moveSpeed = baseMoveSpeed * slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        // 이동속도 복구
        moveSpeed = baseMoveSpeed;

        isSlowed = false;
    }
}
