using UnityEngine;

namespace EscapeTheTrenches.PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public float duration = 5f;

        // 当玩家拾取道具时调用此方法
        public virtual void Activate()
        {
            Debug.Log("道具激活，持续时间：" + duration + " 秒");
         
            Destroy(gameObject);  // 激活后移除道具预制体
        }
    }


}
