using System.Collections;
using UnityEngine;
using EscapeTheTrenches.Player;

namespace EscapeTheTrenches.PowerUps
{
    public class SpeedBoostPowerUp : PowerUp
    {
        // 加速倍率
        public float speedMultiplier = 1.5f;

        public override void Activate()
        {
            Debug.Log("Speed Boost PowerUp activated, duration: " + duration + " seconds.");
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 利用玩家对象启动协程来应用加速效果
                player.StartCoroutine(ApplySpeedBoost(player));
            }
            Destroy(gameObject);
        }

        private IEnumerator ApplySpeedBoost(PlayerController player)
        {
            // 记录原始跳跃力度
            float originalJumpForce = player.jumpForce;
            // 增加跳跃力度以体现加速效果
            player.jumpForce *= speedMultiplier;
            yield return new WaitForSeconds(duration);
            // 恢复原始数值
            player.jumpForce = originalJumpForce;
        }
    }
}
