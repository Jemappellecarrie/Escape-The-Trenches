using System.Collections;
using UnityEngine;
using EscapeTheTrenches.Player;

namespace EscapeTheTrenches.PowerUps
{
    public class DoubleCoinsPowerUp : PowerUp
    {
        public override void Activate()
        {
            Debug.Log("Double Coins PowerUp activated, duration: " + duration + " seconds.");
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 利用玩家对象启动协程来实现金币翻倍效果
                player.StartCoroutine(ApplyDoubleCoins(player));
            }
            Destroy(gameObject);
        }

        private IEnumerator ApplyDoubleCoins(PlayerController player)
        {
            // 记录原始金币倍数
            float originalMultiplier = player.coinMultiplier;
            // 设置金币倍数为 2 倍
            player.coinMultiplier = 2f;
            yield return new WaitForSeconds(duration);
            // 恢复原始金币倍数
            player.coinMultiplier = originalMultiplier;
        }
    }
}
