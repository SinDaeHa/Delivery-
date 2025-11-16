using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 9f;    // 이동 속도

    [Header("화면 X 이동 제한")]
    public float minX = -4.5f;        // 왼쪽 끝 X
    public float maxX = 4.5f;         // 오른쪽 끝 X

    void Update()
    {
        // 좌우 입력: A/D, 방향키 ←/→
        float inputX = Input.GetAxisRaw("Horizontal"); // -1, 0, 1

        // 이동
        Vector3 pos = transform.position;
        pos.x += inputX * moveSpeed * Time.deltaTime;

        // 화면 안으로 클램프
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}
