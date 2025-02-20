using UnityEngine;
using System;

namespace EscapeTheTrenches.Data
{
    [System.Serializable]
    public class GameData
    {
        // 最高分
        public int highScore;
        // 游戏中收集的金币数量
        public int currency;
        // 内购或奖励获得的高级货币，用于继续游戏或双倍奖励
        public int premiumCurrency;
        // 玩家购买的武器升级等级
        public int weaponUpgradeLevel;
        // 剩余的游戏尝试次数
        public int attemptsRemaining;
        // 上一次尝试结束的时间
        public string lastAttemptTime;

        // 构造函数中设置初始值
        public GameData()
        {
            highScore = 0;
            currency = 0;
            premiumCurrency = 0;
            weaponUpgradeLevel = 0;
            attemptsRemaining = 3;
            lastAttemptTime = DateTime.Now.ToString();
        }
    }

    public static class SaveSystem
    {
        private const string GameDataKey = "GameData";

        public static void SaveData(GameData data)
        {
            string jsonData = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(GameDataKey, jsonData);
            PlayerPrefs.Save();
            Debug.Log("游戏数据已保存: " + jsonData);
        }

        public static GameData LoadData()
        {
            if (PlayerPrefs.HasKey(GameDataKey))
            {
                string jsonData = PlayerPrefs.GetString(GameDataKey);
                return JsonUtility.FromJson<GameData>(jsonData);
            }
            return new GameData();
        }
    }
}
