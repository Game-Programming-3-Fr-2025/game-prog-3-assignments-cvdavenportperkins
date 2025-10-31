using UnityEngine;

namespace PrototypeFour
{
    public class EnemyAI : MonoBehaviour
    {
        public Transform player;
        private LevelWorld levelWorld;
        private float currentSpeed;
        private float timer;
        private bool accelerating;

        void Start()
        {
            levelWorld = FindFirstObjectByType<LevelWorld>();
            currentSpeed = levelWorld.enemyBaseSpeed;
            timer = 0f;
            accelerating = false;
        }

        void Update()
        {
            if (player == null) return;

            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            float timeRemaining = levelWorld.levelDuration - Time.timeSinceLevelLoad;

            if (!accelerating && timeRemaining <= levelWorld.accelerationWindow)
            {
                accelerating = true;
            }

            if (accelerating && currentSpeed < levelWorld.enemyMaxSpeed)
            {
                currentSpeed += (levelWorld.enemyMaxSpeed - levelWorld.enemyBaseSpeed) * (Time.deltaTime / levelWorld.accelerationWindow);
            }
        }
    }
}

