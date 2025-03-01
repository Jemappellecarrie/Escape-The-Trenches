using System;
using UnityEngine;
using EscapeTheTrenches.Data;
using EscapeTheTrenches.Ads;
using EscapeTheTrenches.Core;



namespace EscapeTheTrenches.Monetization
{
    public class Monetization : MonoBehaviour
    {
        public int maxAdContinueAttempts = 1;
        public int maxAdDoubleRewards = 1;
        private int adContinueCount = 0;
        private int adDoubleCount = 0;
        public int premiumCurrencyCostForContinue = 10;
        public int premiumCurrencyCostForDouble = 5;
        public AdManager adManager;
        private GameData gameData;
        public Action OnContinueAttemptSuccessful;
        public Action OnDoubleRewardSuccessful;

        private void Start()
        {
            gameData = SaveSystem.LoadData();
            if (adManager == null)
            {
                adManager = FindObjectOfType<AdManager>();
            }
        }

        public void ContinueAttemptViaAd()
        {
            if (adContinueCount < maxAdContinueAttempts)
            {
                adContinueCount++;
                if (adManager != null)
                {
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
                        // 将本局金币翻倍累加到持久数据中
                        GameManager.Instance.AddSessionCoinsToPersistent(true);
                        OnDoubleRewardSuccessful?.Invoke();
                    };
                    adManager.ShowRewardedAd();
                }
                else
                {
                    Debug.LogWarning("AdManager 不可用，直接翻倍奖励。");
                    GameManager.Instance.AddSessionCoinsToPersistent(true);
                    OnDoubleRewardSuccessful?.Invoke();
                }
            }
            else
            {
                Debug.Log("广告机会已用完，无法通过广告翻倍奖励！");
            }
        }

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

        public void DoubleRewardViaPremiumCurrency()
        {
            if (gameData.premiumCurrency >= premiumCurrencyCostForDouble)
            {
                gameData.premiumCurrency -= premiumCurrencyCostForDouble;
                SaveSystem.SaveData(gameData);
                Debug.Log("使用高级货币奖励翻倍成功！");
                GameManager.Instance.AddSessionCoinsToPersistent(true);
                OnDoubleRewardSuccessful?.Invoke();
            }
            else
            {
                Debug.Log("高级货币不足，无法通过高级货币翻倍奖励！");
            }
        }

        public void ResetAdCounts()
        {
            adContinueCount = 0;
            adDoubleCount = 0;
        }
    }
}
