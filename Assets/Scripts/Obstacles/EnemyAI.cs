using UnityEngine;

namespace EscapeTheTrenches.Obstacles.Enemies
{
    public class EnemyAI : MonoBehaviour
    {
        public float chaseSpeed = 2f;         // 追击玩家时的速度
        public float patrolSpeed = 1f;        // 巡逻时的速度
        public float detectionRange = 5f;     // 检测玩家的范围
        public float patrolRadius = 3f;       // 巡逻区域半径
        public float destroyDistanceBelowPlayer = 500f; // 当敌人低于玩家多少时触发重生

        [Header("Respawn Settings")]
        public float respawnDistanceAbovePlayer = 500f;  // 重生时敌人距离玩家上方的距离
        public float horizontalRespawnRange = 200f;        // 重生时 X 轴随机偏移范围

        private Transform player;
        private Vector3 spawnOrigin;          // 记录敌人初始生成位置，用于巡逻参考
        private Vector3 patrolTarget;         // 当前巡逻目标
        private bool hasPatrolTarget = false; // 是否已分配巡逻目标

        private enum EnemyState { Patrol, Chase }
        private EnemyState currentState = EnemyState.Patrol;

        private void Start()
        {
            // 获取玩家引用
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            // 记录初始生成位置
            spawnOrigin = transform.position;
        }

        private void Update()
        {
            if (player == null)
                return;

            // 当敌人低于玩家一定距离时，重新生成（重置到玩家上方）
            if (transform.position.y < player.position.y - destroyDistanceBelowPlayer)
            {
                Respawn();
                return;
            }

            // 判断与玩家的距离，决定状态切换
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            currentState = (distanceToPlayer <= detectionRange) ? EnemyState.Chase : EnemyState.Patrol;

            switch (currentState)
            {
                case EnemyState.Chase:
                    ChasePlayer();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
            }
        }

        private void Respawn()
        {
            // 保证敌人重生时 X 坐标与玩家一致
            float spawnX = player.position.x;
            // 在玩家上方 respawnDistanceAbovePlayer 的位置重生
            float spawnY = player.position.y + respawnDistanceAbovePlayer;
            transform.position = new Vector3(spawnX, spawnY, transform.position.z);
            // 重置巡逻目标
            hasPatrolTarget = false;
            Debug.Log("敌人重生于: " + transform.position);
        }


        private void ChasePlayer()
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }

        private void Patrol()
        {
            if (!hasPatrolTarget || Vector2.Distance(transform.position, patrolTarget) < 0.1f)
            {
                patrolTarget = spawnOrigin + new Vector3(Random.Range(-patrolRadius, patrolRadius),
                                                         Random.Range(-patrolRadius, patrolRadius), 0);
                hasPatrolTarget = true;
            }
            transform.position = Vector2.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("敌人撞击玩家！");
                EscapeTheTrenches.Core.GameManager.Instance.SetGameState(EscapeTheTrenches.Core.GameManager.GameState.GameOver);
            }
        }
    }
}
