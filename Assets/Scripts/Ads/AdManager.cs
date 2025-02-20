using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EscapeTheTrenches.Ads
{
    public class AdManager : MonoBehaviour
    {
        // 广告面板引用
        public GameObject adPanel;
        // 广告面板上的文本控件，用于显示广告内容
        public Text adText;
        // 广告展示持续时间
        public float adDuration = 3f;

        // 广告结束后的回调
        public System.Action OnAdFinished;

        /// <summary>
        /// 模拟展示激励广告
        /// </summary>
        public void ShowRewardedAd()
        {
            if (adPanel != null)
            {
                // 设置广告文本
                if (adText != null)
                {
                    adText.text = "BTW, This is A piece of Ad!";
                }
                // 激活广告面板
                adPanel.SetActive(true);
                // 开启协程模拟广告播放
                StartCoroutine(AdRoutine());
            }
            else
            {
                Debug.Log("广告面板未设置，请检查 AdManager 组件。");
            }
        }

        private IEnumerator AdRoutine()
        {
            // 等待广告展示持续时间
            yield return new WaitForSeconds(adDuration);
            // 关闭广告面板
            adPanel.SetActive(false);
            Debug.Log("广告展示结束。");
            // 调用广告结束回调，给予奖励等处理
            OnAdFinished?.Invoke();
        }
    }
}
