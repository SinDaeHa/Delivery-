using UnityEngine;

public class SpeedBumpObstacle : Obstacle
{
    [Header("Speed Bump Slow Settings")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 1.5f;

    protected override void OnPlayerHit(PlayerController player, HeartSystem hs)
    {
        // 하트 감소 없음
        // 무적 없음
        // 이동속도만 감소
        if (player != null)
            player.TriggerCustomSlowdown(slowMultiplier, slowDuration);

    }
}
