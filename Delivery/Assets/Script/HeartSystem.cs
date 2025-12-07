using UnityEngine;
using System.Collections;

public class HeartSystem : MonoBehaviour
{
    [Header("Heart Setting")]
    public GameObject heartPrefab;
    public int maxHearts = 3;

    [Header("UI Locate")]
    public Vector2 lastHeartPos = new Vector2(-5.3f, -10.1f);
    public float heartSpacing = 0.8f;

    private GameObject[] hearts;
    private int currentHearts;

    void Start()
    {
        currentHearts = maxHearts;
        hearts = new GameObject[maxHearts];

        // hearts[0] = 가장 아래 → 마지막에 사라지는 하트
        for (int i = 0; i < maxHearts; i++)
        {
            float yPos = lastHeartPos.y + heartSpacing * i;
            Vector3 pos = new Vector3(lastHeartPos.x, yPos, 0);

            hearts[i] = Instantiate(heartPrefab, pos, Quaternion.identity, transform);
        }
    }

    public void TakeDamage(GameObject enemyObj)
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null && player.IsInvincible())
            return;

        // 하트 감소
        currentHearts--;

        if (currentHearts >= 0)
        {
            Destroy(hearts[currentHearts]);
            hearts[currentHearts] = null;
        }

        // 충돌한 car 제거
        if (enemyObj != null)
            Destroy(enemyObj);

        // 하트가 0이면 즉시 게임오버
        if (currentHearts <= 0)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.GameOver();
            return;
        }

        // 🔥 플레이어 무적 + 하트 깜빡임 (플레이어 설정 기반)
        player.TriggerSlowdown();
        player.TriggerInvincible();
        StartCoroutine(HeartsBlinkRoutine(player.invincibleTime));
    }
    public void AddHeart()
    {
        // 이미 최대체력이면 회복 안 됨
        if (currentHearts >= maxHearts)
            return;

        // 1) 하트 오브젝트 생성
        float yPos = lastHeartPos.y + heartSpacing * currentHearts;
        Vector3 pos = new Vector3(lastHeartPos.x, yPos, 0);

        hearts[currentHearts] = Instantiate(heartPrefab, pos, Quaternion.identity, transform);

        // 2) 체력 증가
        currentHearts++;
    }

    public int GetCurrentHearts()
    {
        return currentHearts;
    }

    public void ForceRemoveHeart()
    {
        if (currentHearts <= 0) return;

        currentHearts--;
        Destroy(hearts[currentHearts]);
        hearts[currentHearts] = null;
    }

    public bool IsFullHeart()
    {
        return currentHearts >= maxHearts;
    }


    private IEnumerator HeartsBlinkRoutine(float duration)
    {
        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;

            // 남아 있는 하트만 깜빡임
            for (int i = 0; i < maxHearts; i++)
            {
                if (hearts[i] != null)
                {
                    var sr = hearts[i].GetComponent<SpriteRenderer>();
                    if (sr != null)
                        sr.enabled = visible;
                }
            }

            float interval = 0.15f;
            yield return new WaitForSeconds(interval);
            timer += interval;
        }

        // 모든 하트 다시 보이게
        for (int i = 0; i < maxHearts; i++)
        {
            if (hearts[i] != null)
            {
                var sr = hearts[i].GetComponent<SpriteRenderer>();
                sr.enabled = true;
            }
        }
    }
}


