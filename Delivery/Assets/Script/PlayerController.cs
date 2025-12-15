using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Moving Setting")]
    public float moveSpeed = 20f;
    public float minX = -4.75f;
    public float maxX = 4.75f;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite moveSprite;

    [Header("Invincible Time Setting")]
    public float invincibleTime = 3f;

    [Header("Hit Slowdown Setting")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

    [Header("Booster Invincibility Color")]
    public Color boosterInvincibleColor = new Color(0f, 1f, 0.5f, 1f); // 부스터 무적 색상

    private SpriteRenderer sr;
    private Collider2D col;

    private bool isInvincible = false;
    private bool isSlowed = false;

    private float baseMoveSpeed;
    private bool isStageBlinking = false;

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

        float currentSpeed = moveSpeed;
        Vector3 pos = transform.position;
        pos.x += input * currentSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;

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

    // ================================================================
    // 🔥 일반 무적 (교통신호/피격 등) → 투명 깜빡임
    // ================================================================
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

    // ================================================================
    // 🔥 슬로우 효과
    // ================================================================
    public void TriggerSlowdown()
    {
        if (!isSlowed)
            StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        isSlowed = true;
        moveSpeed = baseMoveSpeed * slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        moveSpeed = baseMoveSpeed;
        isSlowed = false;
    }

    public void TriggerCustomSlowdown(float multiplier, float duration)
    {
        StartCoroutine(CustomSlowdownRoutine(multiplier, duration));
    }

    private IEnumerator CustomSlowdownRoutine(float m, float d)
    {
        float original = moveSpeed;
        moveSpeed = baseMoveSpeed * m;

        yield return new WaitForSeconds(d);
        moveSpeed = original;
    }

    // ================================================================
    // 🔥 스테이지 종료용 충돌 OFF + 깜빡임
    // ================================================================
    public void DisableCollisionForStageEnd()
    {
        if (col != null)
            col.enabled = false;
    }

    public void StartBlinking(float duration)
    {
        if (!isStageBlinking)
            StartCoroutine(BlinkRoutine(duration));
    }

    private IEnumerator BlinkRoutine(float duration)
    {
        isStageBlinking = true;

        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;
            sr.enabled = visible;

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        sr.enabled = true;
        isStageBlinking = false;
    }

    // ================================================================
    // 🔥 Booster 무적 - 색상 깜빡임
    // ================================================================
    public void TriggerInvincibleCustom(float duration)
    {
        StartCoroutine(InvincibleColorRoutine(duration));
    }

    private IEnumerator InvincibleColorRoutine(float duration)
    {
        isInvincible = true;
        col.enabled = false;

        float timer = 0f;
        bool useAltColor = false;

        Color originalColor = sr.color;

        while (timer < duration)
        {
            useAltColor = !useAltColor;

            sr.color = useAltColor ? boosterInvincibleColor : originalColor;

            yield return new WaitForSeconds(0.15f);
            timer += 0.15f;
        }

        sr.color = originalColor;
        col.enabled = true;
        isInvincible = false;
    }

    // ================================================================
    // 🔥 Booster 이동속도 증가
    // ================================================================
    public void TriggerCustomSpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(CustomSpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator CustomSpeedBoostRoutine(float multiplier, float duration)
    {
        float original = moveSpeed;
        moveSpeed = baseMoveSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = original;
    }
}
