using System.Collections;
using UnityEngine;

namespace EscapeTheTrenches.Obstacles.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [Header("Enemy Spawn Settings")]
        public GameObject enemyPrefab;          // 敌人预制体
        public Transform player;                // 玩家引用（在 Inspector 中拖拽玩家对象）
        public float spawnDistanceAbovePlayer = 500f; // 敌人生成到玩家上方的距离
        public float spawnInterval = 3f;        // 敌人生成间隔（秒）

        private void Start()
        {
            StartCoroutine(SpawnEnemyRoutine());
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            while (true)
            {
                // 如果场景中没有敌人，则生成一个
                int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
                if (currentEnemies == 0)
                {
                    SpawnEnemy();
                }

                // 等待一段时间后再尝试生成
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            // 在玩家上方固定距离生成
            float spawnY = player.position.y + spawnDistanceAbovePlayer;

            // 敌人X坐标和玩家一致，或者你可以改成随机范围
            float spawnX = player.position.x;

            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("生成敌人，位置：" + spawnPosition);
        }
    }
}
