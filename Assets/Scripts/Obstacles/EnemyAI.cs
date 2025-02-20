using UnityEngine;

namespace EscapeTheTrenches.Obstacles.Enemies
{
    public class EnemyAI : MonoBehaviour
    {
        public float chaseSpeed = 2f;         // 追击玩家时的速度
        public float patrolSpeed = 1f;        // 巡逻时的速度
        public float detectionRange = 5f;     // 检测玩家的范围
        public float patrolRadius = 3f;       // 巡逻区域半径（基于生成点）

        private Transform player;
        private Vector3 spawnOrigin;          // 敌人初始生成位置
        private Vector3 patrolTarget;         // 当前巡逻目标
        private bool hasPatrolTarget = false; // 是否已经分配巡逻目标

        private enum EnemyState { Patrol, Chase }
        private EnemyState currentState = EnemyState.Patrol;

        private void Start()
        {
            // 获取玩家引用
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;

            // 记录敌人的初始位置，用于限定巡逻范围
            spawnOrigin = transform.position;
        }

        private void Update()
        {
            if (player == null)
                return;

            // 判断与玩家的距离，决定状态切换
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                currentState = EnemyState.Chase;
            }
            else
            {
                currentState = EnemyState.Patrol;
            }

            // 根据状态执行相应的行为
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

        /// <summary>
        /// 追击玩家：向玩家当前位置移动
        /// </summary>
        private void ChasePlayer()
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 巡逻逻辑：在生成点附近随机选择一个目标点并向其移动
        /// </summary>
        private void Patrol()
        {
            if (!hasPatrolTarget || Vector2.Distance(transform.position, patrolTarget) < 0.1f)
            {
                // 生成新的巡逻目标
                patrolTarget = spawnOrigin + new Vector3(Random.Range(-patrolRadius, patrolRadius), Random.Range(-patrolRadius, patrolRadius), 0);
                hasPatrolTarget = true;
            }
            // 向目标位置移动
            transform.position = Vector2.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("敌人撞击玩家！");
                // 撞击玩家后调用 GameManager 切换到 GameOver 状态
                EscapeTheTrenches.Core.GameManager.Instance.SetGameState(EscapeTheTrenches.Core.GameManager.GameState.GameOver);
            }
        }
    }
}
