using UnityEngine;
using System;
using EscapeTheTrenches.Data;
using EscapeTheTrenches.Ads;

namespace EscapeTheTrenches.Monetization
{
    public class Monetization : MonoBehaviour
    {
        // 每局游戏允许通过广告继续游戏和翻倍奖励的最大次数
        public int maxAdContinueAttempts = 1;
        public int maxAdDoubleRewards = 1;

        // 内部计数
        private int adContinueCount = 0;
        private int adDoubleCount = 0;

        // 使用高级货币的花费（单位：premiumCurrency）
        public int premiumCurrencyCostForContinue = 10;
        public int premiumCurrencyCostForDouble = 5;

        // 广告模块引用（请在 Inspector 中赋值或自动查找）
        public AdManager adManager;

        // 当前游戏数据
        private GameData gameData;

        // 广告操作完成后的回调（例如继续游戏或翻倍奖励）
        public Action OnContinueAttemptSuccessful;
        public Action OnDoubleRewardSuccessful;

        private void Start()
        {
            // 加载数据
            gameData = SaveSystem.LoadData();

            // 尝试自动查找广告管理器
            if (adManager == null)
            {
                adManager = FindObjectOfType<AdManager>();
            }
        }

        /// <summary>
        /// 通过广告继续游戏尝试
        /// </summary>
        public void ContinueAttemptViaAd()
        {
            if (adContinueCount < maxAdContinueAttempts)
            {
                adContinueCount++;
                if (adManager != null)
                {
                    // 设置广告结束回调，继续游戏尝试
                    adManager.OnAdFinished = () =>
                    {
                        Debug.Log("广告播放完毕，继续游戏尝试成功！");
                        OnContinueAttemptSuccessful?.Invoke();
                    };
                    adManager.ShowRewardedAd();
                }
                else
                {
                    Debug.LogWarning("AdManager 不可用，直接继续游戏尝试。");
                    OnContinueAttemptSuccessful?.Invoke();
                }
            }
            else
            {
                Debug.Log("广告机会已用完，无法通过广告继续游戏尝试！");
            }
        }

        /// <summary>
        /// 通过广告翻倍奖励
        /// </summary>
        public void DoubleRewardViaAd()
        {
            if (adDoubleCount < maxAdDoubleRewards)
            {
                adDoubleCount++;
                if (adManager != null)
                {
                    adManager.OnAdFinished = () =>
                    {
                        Debug.Log("广告播放完毕，奖励翻倍成功！");
                        OnDoubleRewardSuccessful?.Invoke();
                    };
                    adManager.ShowRewardedAd();
                }
                else
                {
                    Debug.LogWarning("AdManager 不可用，直接翻倍奖励。");
                    OnDoubleRewardSuccessful?.Invoke();
                }
            }
            else
            {
                Debug.Log("广告机会已用完，无法通过广告翻倍奖励！");
            }
        }

        /// <summary>
        /// 通过高级货币继续游戏尝试
        /// </summary>
        public void ContinueAttemptViaPremiumCurrency()
        {
            if (gameData.premiumCurrency >= premiumCurrencyCostForContinue)
            {
                gameData.premiumCurrency -= premiumCurrencyCostForContinue;
                SaveSystem.SaveData(gameData);
                Debug.Log("使用高级货币继续游戏尝试成功！");
                OnContinueAttemptSuccessful?.Invoke();
            }
            else
            {
                Debug.Log("高级货币不足，无法通过高级货币继续游戏尝试！");
            }
        }

        /// <summary>
        /// 通过高级货币翻倍奖励
        /// </summary>
        public void DoubleRewardViaPremiumCurrency()
        {
            if (gameData.premiumCurrency >= premiumCurrencyCostForDouble)
            {
                gameData.premiumCurrency -= premiumCurrencyCostForDouble;
                SaveSystem.SaveData(gameData);
                Debug.Log("使用高级货币奖励翻倍成功！");
                OnDoubleRewardSuccessful?.Invoke();
            }
            else
            {
                Debug.Log("高级货币不足，无法通过高级货币翻倍奖励！");
            }
        }

        /// <summary>
        /// 重置当前局中广告次数（例如在新游戏尝试开始时调用）
        /// </summary>
        public void ResetAdCounts()
        {
            adContinueCount = 0;
            adDoubleCount = 0;
        }
    }
}
