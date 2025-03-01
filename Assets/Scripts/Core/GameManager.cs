using UnityEngine;
using System;

using EscapeTheTrenches.Player;
using EscapeTheTrenches.Data;


namespace EscapeTheTrenches.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { MainMenu, Playing, GameOver, Paused, AwaitingRecharge }
        public GameState CurrentState { get; private set; }

        // 状态变更通知事件
        public event Action<GameState> OnGameStateChanged;
        // 游戏结束事件
        public event Action OnGameOver;

        // 尝试次数管理
        public int MaxAttempts = 3;
        public int AttemptsRemaining { get; private set; }

        // 重置等待时间（单位：小时）
        public float RechargeHours = 12f;
        private DateTime lastAttemptTime;

        // 尝试次数变化事件，供 UI 更新显示
        public event Action<int> OnAttemptsChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                AttemptsRemaining = MaxAttempts;

                SetGameState(GameState.MainMenu);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetGameState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
            Debug.Log("游戏状态切换为：" + newState);

            if (newState == GameState.GameOver)
            {
                // 当游戏进入 GameOver 状态时，触发游戏结束事件
                OnGameOver?.Invoke();
            }
        }

        /// <summary>
        /// 尝试开始游戏。如果还有剩余尝试，则扣除一次并进入 Playing 状态；否则检查是否满足重置条件。
        /// </summary>
        public void TryStartGame()
        {
            if (AttemptsRemaining > 0)
            {
                AttemptsRemaining--;
                OnAttemptsChanged?.Invoke(AttemptsRemaining);
                SetGameState(GameState.Playing);
            }
            else
            {
                if (IsRechargeAvailable())
                {
                    ResetAttempts();
                    AttemptsRemaining--;
                    OnAttemptsChanged?.Invoke(AttemptsRemaining);
                    SetGameState(GameState.Playing);
                }
                else
                {
                    // 当前尝试次数已用完且未达到重置条件
                    SetGameState(GameState.AwaitingRecharge);
                    Debug.Log("游戏尝试次数已用完，请等待重置。");
                }
            }
        }

        /// <summary>
        /// 在游戏结束时调用，记录当前时间以便后续判断重置条件。
        /// </summary>
        public void RecordAttemptEnd()
        {
            lastAttemptTime = DateTime.Now;
        }

        /// <summary>
        /// 判断是否达到重置条件（即等待时间是否达到 RechargeHours）。
        /// </summary>
        private bool IsRechargeAvailable()
        {
            TimeSpan elapsed = DateTime.Now - lastAttemptTime;
            return elapsed.TotalHours >= RechargeHours;
        }

        /// <summary>
        /// 重置尝试次数为最大值，并通知 UI 更新。
        /// </summary>
        public void ResetAttempts()
        {
            AttemptsRemaining = MaxAttempts;
            OnAttemptsChanged?.Invoke(AttemptsRemaining );
            Debug.Log("尝试次数已重置。");
        }

        /// <summary>
        /// 继续当前游戏尝试，通常在玩家通过广告或内购选择继续游戏时调用，不消耗新的尝试次数。
        /// </summary>
        public void ContinueGame()
        {
            SetGameState(GameState.Playing);
            Debug.Log("继续当前游戏尝试。");
        }

        public void ReturnToMainMenu()
        {
            // 如果需要在返回主菜单时重置尝试次数或其他数据，可以在这里调用 ResetAttempts() 等方法
            ResetAttempts();
            ClearAllEnemies();
            GameManager.Instance.EndSession(false);
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.coinCount = 0;
            }

            SetGameState(GameState.MainMenu);
            Debug.Log("返回主菜单");
        }
        public void ClearAllEnemies()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }
        }

        public void AddSessionCoinsToPersistent(bool doubleReward)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                GameData data = SaveSystem.LoadData();
                int coinsToAdd = player.coinCount;
                if (doubleReward)
                {
                    coinsToAdd *= 2;
                }
                data.currency += coinsToAdd;
                SaveSystem.SaveData(data);
                Debug.Log("累加本局金币：" + coinsToAdd + "，持久金币总数：" + data.currency);
            }
        }

        public void EndSession(bool doubleReward)
        {
            // 找到玩家，获取本局金币
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                int coinsToAdd = player.coinCount;
                if (doubleReward)
                {
                    coinsToAdd *= 2;
                }
                // 加载当前持久数据
                GameData data = SaveSystem.LoadData();
                data.currency += coinsToAdd;
                SaveSystem.SaveData(data);
                Debug.Log("本局金币 " + coinsToAdd + " 已加入总金币，当前总金币：" + data.currency);
            }
        }



    }
}
