using System.Collections;
using UnityEngine;
using EscapeTheTrenches.Player;

namespace EscapeTheTrenches.PowerUps
{
    public class ShieldPowerUp : PowerUp
    {
        public override void Activate()
        {
            Debug.Log("Shield PowerUp activated, duration: " + duration + " seconds.");
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 利用玩家对象启动协程来应用护盾效果
                player.StartCoroutine(ApplyShield(player));
            }
            Destroy(gameObject);
        }

        private IEnumerator ApplyShield(PlayerController player)
        {
            // 设置玩家为无敌状态
            player.isInvincible = true;
            yield return new WaitForSeconds(duration);
            // 取消无敌状态
            player.isInvincible = false;
        }
    }
}
