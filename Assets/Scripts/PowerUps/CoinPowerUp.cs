using UnityEngine;
using EscapeTheTrenches.Player;

namespace EscapeTheTrenches.PowerUps
{
    public class CoinPowerUp : PowerUp
    {
        // 每次拾取金币增加的数量
        public int coinAmount = 1;
        // 额外控制金币重生的位置偏移（比如距离玩家上方的距离）
        public float coinRespawnDistanceAbovePlayer = 500f;

        private bool activated = false; 

        public override void Activate()
        {
            if (activated)
                return;
            activated = true;

            Debug.Log("CoinPowerUp activated: adding " + coinAmount + " coin(s).");

            // 查找场景中的玩家并增加金币
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.AddCoins(coinAmount);
            }

            // 重生金币采用自己的逻辑
            RespawnCoin();
        }

        /// <summary>
        /// 重生金币：让金币出现在玩家正上方 coinRespawnDistanceAbovePlayer 的位置
        /// </summary>
        private void RespawnCoin()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                float spawnX = player.transform.position.x; // X 坐标和玩家一致
                float spawnY = player.transform.position.y + coinRespawnDistanceAbovePlayer;
                transform.position = new Vector3(spawnX, spawnY, transform.position.z);
                Debug.Log("Coin respawned at (relative to player): " + transform.position);
            }
            else
            {
                Respawn();
            }
            activated = false;
        }
    }
}
