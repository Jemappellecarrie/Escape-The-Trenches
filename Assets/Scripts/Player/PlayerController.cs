using System.Collections.Generic;
using UnityEngine;
using EscapeTheTrenches.Core;
using EscapeTheTrenches.PowerUps;
using System;


namespace EscapeTheTrenches.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float jumpForce = 8f;         // 跳跃力度（纯水平）
        public float jumpDistance = 2.5f;    // 跳跃时左右移动的距离
        public bool isOnLeftWall = true;     // 是否在左侧墙壁
        private bool canJump = true;         // 防止空中重复跳跃

        private Rigidbody2D rb;
        // 我们可以使用左右墙的位置来校正玩家位置，但不做自动向上攀爬
        private Vector2 leftWallPosition;
        private Vector2 rightWallPosition;

        // 金币计数与道具
        public int coinCount = 0;
        private List<PowerUp> storedPowerUps = new List<PowerUp>();
        public int maxStoredPowerUps = 3;

        public bool hasWeaponUpgrade = false;
        public bool isInvincible = false;
        public float coinMultiplier = 1f;

        public event Action<int> OnCoinCountChanged;

        // 判断是否与墙体碰撞（防止空中跳跃）
        private bool isOnWall = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            // 计算左右墙的位置（基于玩家初始位置）
            leftWallPosition = new Vector2(transform.position.x - jumpDistance, transform.position.y);
            rightWallPosition = new Vector2(transform.position.x + jumpDistance, transform.position.y);
        }

        private void Update()
        {
            // 不再自动向上爬，因此无需修改 rb.velocity.y
            // 仅在玩家处于墙上时允许跳跃
            if (Input.GetMouseButtonDown(0) && canJump && isOnWall)
            {
                JumpToOtherWall();
            }
        }

        private void JumpToOtherWall()
        {
            canJump = false; // 防止重复跳跃

            // 如果在左墙，则向右跳；否则向左跳
            float jumpDirection = isOnLeftWall ? 1f : -1f;
            // 只施加水平跳跃力，垂直部分为0
            Vector2 jumpVelocity = new Vector2(jumpForce * jumpDirection, 0);

            rb.velocity = jumpVelocity;

            // 切换墙壁状态
            isOnLeftWall = !isOnLeftWall;

            // 延时恢复跳跃
            Invoke(nameof(EnableJump), 0.2f);
        }

        private void EnableJump()
        {
            canJump = true;
        }

        // 当与墙碰撞时，允许跳跃
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                isOnWall = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                isOnWall = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Obstacle"))
            {
                GameManager.Instance.SetGameState(GameManager.GameState.GameOver);
            }
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
            else if (collision.CompareTag("Coin"))
            {
                // 查找金币组件并调用 Activate()，金币自身会增加 coinCount 并重生
                CoinPowerUp coin = collision.GetComponent<CoinPowerUp>();
                if (coin != null)
                {
                    coin.Activate();
                }
            }

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

        public void AddCoins(int amount)
        {
            coinCount += amount;
            Debug.Log("玩家获得金币：" + amount + "，当前金币：" + coinCount);
            OnCoinCountChanged?.Invoke(coinCount);
        }


    }
}
