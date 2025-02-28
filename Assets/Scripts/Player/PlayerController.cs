using System.Collections.Generic;
using UnityEngine;
using EscapeTheTrenches.Core;
using EscapeTheTrenches.PowerUps;

namespace EscapeTheTrenches.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float climbSpeed = 3f;        // 自动攀爬速度
        public float jumpForce = 8f;         // 跳跃力度
        public float jumpDistance = 2.5f;    // 跳跃时左右移动的距离
        public bool isOnLeftWall = true;     // 是否在左侧墙壁
        private bool canJump = true;         // 确保跳跃后不会无限跳跃

        private Rigidbody2D rb;
        private Vector2 leftWallPosition;
        private Vector2 rightWallPosition;

        // 金币计数
        public int coinCount = 0;
        // 存储道具（最多 3 个）
        private List<PowerUp> storedPowerUps = new List<PowerUp>();
        public int maxStoredPowerUps = 3;

        public bool hasWeaponUpgrade = false;
        public bool isInvincible = false;
        public float coinMultiplier = 1f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // 计算左右墙壁的位置（基于玩家初始位置）
            leftWallPosition = new Vector2(transform.position.x - jumpDistance, transform.position.y);
            rightWallPosition = new Vector2(transform.position.x + jumpDistance, transform.position.y);
        }

        private void Update()
        {
            // 角色自动向上攀爬
            rb.velocity = new Vector2(rb.velocity.x, climbSpeed);

            // 监听点击事件进行跳跃
            if (Input.GetMouseButtonDown(0) && canJump)
            {
                JumpToOtherWall();
            }
        }

        private void JumpToOtherWall()
        {
            canJump = false; // 防止重复跳跃

            // 计算跳跃方向
            float jumpDirection = isOnLeftWall ? 1f : -1f;
            Vector2 jumpVelocity = new Vector2(jumpForce * jumpDirection, jumpForce);

            // 施加跳跃力
            rb.velocity = jumpVelocity;

            // 切换墙壁状态
            isOnLeftWall = !isOnLeftWall;

            // 设置新位置（确保跳跃后不会偏离墙面）
            if (isOnLeftWall)
                transform.position = new Vector2(leftWallPosition.x, transform.position.y);
            else
                transform.position = new Vector2(rightWallPosition.x, transform.position.y);

            // 延迟一段时间后允许再次跳跃（防止空中连跳）
            Invoke(nameof(EnableJump), 0.2f);
        }

        private void EnableJump()
        {
            canJump = true;
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
                int addedCoins = Mathf.RoundToInt(1 * coinMultiplier);
                coinCount += addedCoins;
                Destroy(collision.gameObject);
            }
            // 收集道具
            else if (collision.CompareTag("PowerUp"))
            {
                PowerUp powerUp = collision.GetComponent<PowerUp>();
                if (powerUp != null)
                {
                    if (storedPowerUps.Count < maxStoredPowerUps)
                    {
                        storedPowerUps.Add(powerUp);
                        powerUp.gameObject.SetActive(false);
                    }
                    else
                    {
                        powerUp.Activate();
                        Destroy(collision.gameObject);
                    }
                }
            }
        }

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
