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

    // 🔥 추가: 마지막 하트 경고 오브젝트
    [Header("Last Heart Warning")]
    public GameObject lastHeartWarning;

    private GameObject[] hearts;
    private int currentHearts;

    void Start()
    {
        currentHearts = maxHearts;
        hearts = new GameObject[maxHearts];

        for (int i = 0; i < maxHearts; i++)
        {
            float yPos = lastHeartPos.y + heartSpacing * i;
            Vector3 pos = new Vector3(lastHeartPos.x, yPos, 0);

            hearts[i] = Instantiate(heartPrefab, pos, Quaternion.identity, transform);
        }

        // 🔥 시작 시 경고 끄기
        if (lastHeartWarning != null)
            lastHeartWarning.SetActive(false);
    }

    public void TakeDamage(GameObject enemyObj)
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null && player.IsInvincible())
            return;

        currentHearts--;

        if (currentHearts >= 0)
        {
            Destroy(hearts[currentHearts]);
            hearts[currentHearts] = null;
        }

        if (enemyObj != null)
            Destroy(enemyObj);

        // 🔥 하트 상태 변경 → 경고 업데이트
        UpdateLastHeartWarning();

        if (currentHearts <= 0)
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.GameOver();

            // 🔥 게임오버 시 경고 끄기
            if (lastHeartWarning != null)
                lastHeartWarning.SetActive(false);

            return;
        }

        player.TriggerSlowdown();
        player.TriggerInvincible();
        StartCoroutine(HeartsBlinkRoutine(player.invincibleTime));
    }

    public void AddHeart()
    {
        if (currentHearts >= maxHearts)
            return;

        float yPos = lastHeartPos.y + heartSpacing * currentHearts;
        Vector3 pos = new Vector3(lastHeartPos.x, yPos, 0);

        hearts[currentHearts] = Instantiate(heartPrefab, pos, Quaternion.identity, transform);
        currentHearts++;

        // 🔥 하트 회복 → 경고 업데이트
        UpdateLastHeartWarning();
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

        // 🔥 강제 제거 시도 경고 업데이트
        UpdateLastHeartWarning();
    }

    public bool IsFullHeart()
    {
        return currentHearts >= maxHearts;
    }

    void UpdateLastHeartWarning()
    {
        if (lastHeartWarning == null)
            return;

        if (currentHearts == 1)
            lastHeartWarning.SetActive(true);
        else
            lastHeartWarning.SetActive(false);
    }

    private IEnumerator HeartsBlinkRoutine(float duration)
    {
        float timer = 0f;
        bool visible = true;

        while (timer < duration)
        {
            visible = !visible;

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
