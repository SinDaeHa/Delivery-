using UnityEngine;
using TMPro;

public class TMPUIFloatingTitle : MonoBehaviour
{
    public float amplitude = 10f;   // 픽셀 단위
    public float speed = 2f;

    private RectTransform rt;
    private Vector2 startPos;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
    }

    void Update()
    {
        Vector2 pos = startPos;
        pos.y += Mathf.Sin(Time.time * speed) * amplitude;
        rt.anchoredPosition = pos;
    }
}
