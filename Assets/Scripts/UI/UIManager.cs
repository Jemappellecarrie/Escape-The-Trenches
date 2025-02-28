using UnityEngine;
using UnityEngine.UI;
using EscapeTheTrenches.Core;

namespace EscapeTheTrenches.UI
{
    public class UIManager : MonoBehaviour
    {
        // 主菜单、游戏结束和游戏内 HUD 面板
        public GameObject mainMenuPanel;
        public GameObject gameOverPanel;
        public GameObject playingPanel; // 游戏进行时显示 HUD
        // 货币化选项面板（GameOver 时显示）
        public GameObject monetizationPanel;

        [SerializeField] private GameManager _manager;

        // 分数、金币和尝试次数的 UI 文本
        public Text scoreText;
        public Text coinText;
        public Text attemptsText;

        private int score;
        private int coinCount;

        private void OnEnable()
        {
            _manager.OnGameStateChanged += HandleGameStateChanged;
            _manager.OnAttemptsChanged += UpdateAttemptsUI;
            _manager.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            _manager.OnGameStateChanged -= HandleGameStateChanged;
            _manager.OnAttemptsChanged -= UpdateAttemptsUI;
            _manager.OnGameOver -= HandleGameOver;
        }

        /// <summary>
        /// 根据游戏状态切换不同的 UI 面板
        /// </summary>
        /// <param name="newState">当前游戏状态</param>
        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            switch (newState)
            {
                case GameManager.GameState.MainMenu:
                    mainMenuPanel.SetActive(true);
                    gameOverPanel.SetActive(false);
                    playingPanel.SetActive(false);
                    if (monetizationPanel != null)
                        monetizationPanel.SetActive(false);
                    break;
                case GameManager.GameState.Playing:
                    mainMenuPanel.SetActive(false);
                    gameOverPanel.SetActive(false);
                    playingPanel.SetActive(true);
                    if (monetizationPanel != null)
                        monetizationPanel.SetActive(false);
                    break;
                case GameManager.GameState.GameOver:
                    mainMenuPanel.SetActive(false);
                    gameOverPanel.SetActive(true);
                    playingPanel.SetActive(false);
                    // monetizationPanel 的显示由 OnGameOver 事件控制
                    break;
                case GameManager.GameState.Paused:
                    // 暂停状态下可以显示额外的暂停界面
                    break;
                case GameManager.GameState.AwaitingRecharge:
                    mainMenuPanel.SetActive(false);
                    gameOverPanel.SetActive(true);
                    playingPanel.SetActive(false);
                    if (monetizationPanel != null)
                        monetizationPanel.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// 处理游戏结束事件，显示货币化选项面板
        /// </summary>
        private void HandleGameOver()
        {
            if (monetizationPanel != null)
            {
                monetizationPanel.SetActive(true);
            }
        }

        /// <summary>
        /// 更新分数显示
        /// </summary>
        /// <param name="increment">分数增量</param>
        public void UpdateScore(int increment)
        {
            score += increment;
            if (scoreText != null)
                scoreText.text = "Score: " + score;
        }

        /// <summary>
        /// 重置分数
        /// </summary>
        public void ResetScore()
        {
            score = 0;
            if (scoreText != null)
                scoreText.text = "Score: 0";
        }

        /// <summary>
        /// 更新金币显示
        /// </summary>
        /// <param name="coins">当前金币数量</param>
        public void UpdateCoinCount(int coins)
        {
            coinCount = coins;
            if (coinText != null)
                coinText.text = "Coins: " + coinCount;
        }

        /// <summary>
        /// 更新尝试次数的显示（通过订阅 GameManager 的 OnAttemptsChanged 事件）
        /// </summary>
        /// <param name="attemptsRemaining">剩余尝试次数</param>
        private void UpdateAttemptsUI(int attemptsRemaining)
        {
            if (attemptsText != null)
                attemptsText.text = "Attempts: " + attemptsRemaining;
        }
    }
}
