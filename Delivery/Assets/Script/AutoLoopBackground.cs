using UnityEngine;
using System.Collections.Generic;

public class AutoLoopBackground : MonoBehaviour
{
    [Header("Main Background (only 1 object)")]
    public Transform sourceBg;

    [Header("Foreground Layers (multiple allowed)")]
    public List<Transform> foregroundSources = new List<Transform>();

    [Header("Foreground Parallax Multipliers (match order)")]
    public List<float> parallaxMultipliers = new List<float>();

    [Header("Scroll Settings")]
    public float scrollSpeed = 4f;

    [Header("Background Height (Y units)")]
    public float bgHeight = 28f;

    private Transform bg1, bg2;

    // 각 Foreground 마다 2장씩 저장
    private List<Transform> fg1List = new List<Transform>();
    private List<Transform> fg2List = new List<Transform>();

    private bool isRed = false;

    void Start()
    {
        // -------------------------------
        // Main BG 2장 생성
        // -------------------------------
        bg1 = sourceBg;

        bg2 = Instantiate(sourceBg.gameObject, sourceBg.parent).transform;
        bg2.position = bg1.position + new Vector3(0f, bgHeight, 0f);

        // -------------------------------
        // Foreground BG 여러 장 설정
        // -------------------------------
        for (int i = 0; i < foregroundSources.Count; i++)
        {
            Transform fg1 = foregroundSources[i];
            Transform fg2 = Instantiate(fg1.gameObject, fg1.parent).transform;

            // fg1 위로 28만큼 배치
            fg2.position = fg1.position + new Vector3(0f, bgHeight, 0f);

            fg1List.Add(fg1);
            fg2List.Add(fg2);

            // 패럴랙스 multiplier 개수 안 맞을 경우 자동 보정
            if (parallaxMultipliers.Count <= i)
                parallaxMultipliers.Add(1.2f); // 기본값
        }
    }

    void Update()
    {
        if (isRed) return;

        float move = scrollSpeed * Time.deltaTime;

        // ---- Main BG ----
        ScrollAndLoop(bg1, move);
        ScrollAndLoop(bg2, move);

        // ---- Foregrounds ----
        for (int i = 0; i < fg1List.Count; i++)
        {
            float pmove = move * parallaxMultipliers[i];
            ScrollAndLoop(fg1List[i], pmove);
            ScrollAndLoop(fg2List[i], pmove);
        }
    }

    void ScrollAndLoop(Transform bg, float move)
    {
        bg.Translate(Vector3.down * move);

        if (bg.position.y <= -bgHeight)
        {
            bg.position += new Vector3(0f, bgHeight * 2f, 0f);
        }
    }

    public void SetRedLight(bool red)
    {
        isRed = red;
    }
}
