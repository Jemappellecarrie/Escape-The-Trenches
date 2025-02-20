using UnityEngine;
using EscapeTheTrenches.Player;

namespace EscapeTheTrenches.PowerUps
{
    public class CoinPowerUp : PowerUp
    {
        // 每次拾取金币增加的数量
        public int coinAmount = 1;

        public override void Activate()
        {
            Debug.Log("CoinPowerUp activated: adding " + coinAmount + " coin(s).");
            // 查找场景中的玩家
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                // 为玩家增加金币
                player.coinCount += coinAmount;
            }
            // 激活后销毁金币预制体
            Destroy(gameObject);
        }
    }
}
