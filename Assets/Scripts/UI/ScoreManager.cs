using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EscapeTheTrenches.Core;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; private set; }         // 当前分数
    public Text scoreText;                         // UI 文本引用（在 Inspector 中绑定）

    private Coroutine scoreCoroutine;              // 计分协程引用

    private void Start()
    {
        ResetScore();
        // 订阅游戏状态改变事件，以控制计分
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        // 当进入 Playing 状态时，开始计分
        if (newState == GameManager.GameState.Playing)
        {
            if (scoreCoroutine == null)
            {
                scoreCoroutine = StartCoroutine(ScoreCoroutine());
            }
        }
        else
        {
            // 停止计分协程
            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = null;
            }

            // 如果切换到 MainMenu（或其他状态，表示游戏彻底结束），重置分数
            if (newState == GameManager.GameState.MainMenu)
            {
                ResetScore();
            }
        }
    }

    private IEnumerator ScoreCoroutine()
    {
        // 每秒加 1 分
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Score++;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
    }

    public void ResetScore()
    {
        Score = 0;
        UpdateScoreUI();
    }
}
