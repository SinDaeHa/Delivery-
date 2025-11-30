using UnityEngine;

public class ManholeObstacle : Obstacle
{
    protected override void OnPlayerHit(PlayerController player, HeartSystem hs)
    {
        if (player != null)
        {
            // 플레이어 렌더러 꺼짐 (빠진 연출)
            player.GetComponent<SpriteRenderer>().enabled = false;
        }

        // 하트 전부 삭제 + 게임오버
        if (hs != null)
        {
            while (hs.GetCurrentHearts() > 0)
            {
                hs.ForceRemoveHeart();
            }
        }

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.GameOver();

    }
}
