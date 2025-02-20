using System.Collections.Generic;
using UnityEngine;
using EscapeTheTrenches.Core;
using EscapeTheTrenches.PowerUps;

namespace EscapeTheTrenches.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float jumpForce = 10f;         // 跳跃力度
        private Rigidbody2D rb;

        // 金币计数
        public int coinCount = 0;

        // 存储道具（最多 3 个）
        private List<PowerUp> storedPowerUps = new List<PowerUp>();
        public int maxStoredPowerUps = 3;

        // 是否拥有武器升级，解锁攻击敌人功能
        public bool hasWeaponUpgrade = false;

        public bool isInvincible = false;
        public float coinMultiplier = 1f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // 点击屏幕触发跳跃（或游泳）
            if (Input.GetMouseButtonDown(0))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 遇到障碍物直接结束游戏
            if (collision.CompareTag("Obstacle"))
            {
                GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
            }
            // 遇到敌人时，根据武器升级判断：攻击或结束游戏
            else if (collision.CompareTag("Enemy"))
            {
                if (hasWeaponUpgrade)
                {
                    // 攻击敌人：销毁敌人并触发攻击效果
                    Destroy(collision.gameObject);
                }
                else
                {
                    GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
                }
            }
            // 收集金币
            else if (collision.CompareTag("Coin"))
            {
                // 根据 coinMultiplier 计算实际增加的金币数量
                int addedCoins = Mathf.RoundToInt(1 * coinMultiplier);
                coinCount += addedCoins;
                // 此处可添加 UI 更新调用
                Destroy(collision.gameObject);
            }
            // 收集道具：存储到道具列表，若已满则直接激活道具效果
            else if (collision.CompareTag("PowerUp"))
            {
                PowerUp powerUp = collision.GetComponent<PowerUp>();
                if (powerUp != null)
                {
                    if (storedPowerUps.Count < maxStoredPowerUps)
                    {
                        storedPowerUps.Add(powerUp);
                        // 隐藏道具物体，等待玩家在 UI 中点击激活
                        powerUp.gameObject.SetActive(false);
                    }
                    else
                    {
                        // 若道具槽已满，选择直接激活
                        powerUp.Activate();
                        Destroy(collision.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// 供 UI 调用：激活存储槽中的道具，并释放槽位
        /// </summary>
        /// <param name="index">存储槽索引</param>
        public void ActivateStoredPowerUp(int index)
        {
            if (index >= 0 && index < storedPowerUps.Count)
            {
                storedPowerUps[index].Activate();
                storedPowerUps.RemoveAt(index);
            }
        }
    }
}
