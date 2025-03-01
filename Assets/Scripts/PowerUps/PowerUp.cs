using UnityEngine;

namespace EscapeTheTrenches.PowerUps
{
    public abstract class PowerUp : MonoBehaviour
    {
        public float duration = 5f;
        public float fallSpeed = 2f;             // 下落速度
        public float destroyY = -500f;           // 当 Y 小于该值时自动重生

        // 通用重生设置（可以被子类使用或覆盖）
        public float respawnY = 2000f;           // 重生时的 Y 坐标
        public float respawnMinX = 400f;         // 重生时 X 坐标下限
        public float respawnMaxX = 800f;         // 重生时 X 坐标上限

        // 通用下落逻辑
        protected virtual void Update()
        {
            // 使 PowerUp 持续下落
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
            // 如果超出屏幕下限，则重生
            if (transform.position.y < destroyY)
            {
                Respawn();
            }
        }

        /// <summary>
        /// 当玩家拾取道具时调用（默认行为：打印日志并重生）
        /// 子类可重写以实现自己的逻辑
        /// </summary>
        public virtual void Activate()
        {
            Debug.Log("PowerUp activated, duration: " + duration + " 秒");
            Respawn();
        }

        /// <summary>
        /// 重生逻辑：将道具重新定位到指定的重生位置
        /// </summary>
        protected virtual void Respawn()
        {
            float spawnX = Random.Range(respawnMinX, respawnMaxX);
            Vector3 newPos = new Vector3(spawnX, respawnY, transform.position.z);
            transform.position = newPos;
            Debug.Log("PowerUp respawned at: " + newPos);
        }
    }
}
