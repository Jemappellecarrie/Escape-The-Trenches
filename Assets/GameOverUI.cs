using UnityEngine;
using EscapeTheTrenches.Core;
using EscapeTheTrenches.Monetization;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Monetization monetization; // 请在 Inspector 中拖拽 Monetization 组件

    private void Start()
    {
        if (monetization != null)
        {
            monetization.OnContinueAttemptSuccessful += HandleContinueAttemptSuccessful;
            monetization.OnDoubleRewardSuccessful += HandleDoubleRewardSuccessful;
        }
        else
        {
            Debug.LogWarning("Monetization 引用未设置！");
        }
    }

    private void OnDisable()
    {
        if (monetization != null)
        {
            monetization.OnContinueAttemptSuccessful -= HandleContinueAttemptSuccessful;
            monetization.OnDoubleRewardSuccessful -= HandleDoubleRewardSuccessful;
        }
    }

    // 继续游戏广告播放完毕后立即继续游戏
    private void HandleContinueAttemptSuccessful()
    {
        GameManager.Instance.ContinueGame();
    }

    // 翻倍奖励广告播放完毕后仅应用奖励，不自动继续游戏
    private void HandleDoubleRewardSuccessful()
    {
        Debug.Log("广告翻倍奖励播放完毕，奖励已应用，请点击“继续游戏”按钮重新开始！");
        // 此处不调用 GameManager.Instance.ContinueGame()，这样 Game Over UI 保持显示，
        // 玩家需要再次点击“继续游戏”按钮才能开始新游戏。
    }
}
