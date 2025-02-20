using UnityEngine;
using System.Collections;

namespace EscapeTheTrenches.Obstacles
{
    public class ObstacleManager : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs; // 障碍物预制体数组
        public Transform spawnPoint;           // 基准生成位置

        public float baseSpawnRate = 2f;       // 初始生成频率（秒）
        public float minSpawnRate = 0.5f;        // 最快生成频率（秒）
        public float horizontalRange = 1f;     // 水平随机范围
        public float verticalOffset = 2f;      // 垂直偏移量，用于调整生成位置

        // 每个单位高度减少生成间隔的秒数
        public float difficultyFactor = 0.1f;

        private void Start()
        {
            StartCoroutine(SpawnObstaclesRoutine());
        }

        private IEnumerator SpawnObstaclesRoutine()
        {
            while (true)
            {
                // 仅当游戏处于 Playing 状态时生成障碍物
                if (EscapeTheTrenches.Core.GameManager.Instance.CurrentState == EscapeTheTrenches.Core.GameManager.GameState.Playing)
                {
                    SpawnObstacle();

                    // 获取玩家当前高度，动态调整生成间隔
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    float playerHeight = player != null ? player.transform.position.y : 0f;
                    float adjustedSpawnRate = Mathf.Max(minSpawnRate, baseSpawnRate - (playerHeight * difficultyFactor));

                    yield return new WaitForSeconds(adjustedSpawnRate);
                }
                else
                {
                    yield return null;
                }
            }
        }

        private void SpawnObstacle()
        {
            if (obstaclePrefabs.Length == 0 || spawnPoint == null)
                return;

            int index = Random.Range(0, obstaclePrefabs.Length);
            // 在 spawnPoint 的基础上增加水平随机偏移和垂直偏移
            Vector3 spawnPosition = spawnPoint.position + new Vector3(Random.Range(-horizontalRange, horizontalRange), verticalOffset, 0);
            Instantiate(obstaclePrefabs[index], spawnPosition, Quaternion.identity);
        }
    }
}
