using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeFive
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public float loopDuration = 4f;
        //private float loopTimer;
        private int loopChances = 3;

        public float survivalGoal = 60f;
        private float survivalTimer;
       
        public Transform player;
        private Vector3 startPosition;

        public float difficultyInterval = 10f;
        private float nextDifficultyTime = 10f;
        public float playerSpeedMultiplier = 1.1f;
        public float enemySpeedMultiplier = 1.15f;

        
        void Awake()
        {
            Debug.Log("wtf");
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (player != null)
                startPosition = player.position;

            //loopTimer = 0f;
            survivalTimer = 0f;
        }

        void Update()
        {
            // Track survival time
            survivalTimer += Time.deltaTime;

            // Win condition
            if (survivalTimer >= survivalGoal)
            {
                Debug.Log("You survived 60 seconds! Victory!");
                // TODO: trigger win screen
            }

            // Difficulty scaling
            if (survivalTimer >= nextDifficultyTime)
            {
                IncreaseDifficulty();
                nextDifficultyTime += difficultyInterval;
            }
        }

        public void OnPlayerHit()
        {
            loopChances--;

            if (loopChances <= 0)
            {
                Debug.Log("Game Over! Out of loops.");
                // TODO: trigger game over screen
                return;
            }

            RestartLoop();
        }

        private void RestartLoop()
        {
            //loopTimer = 0f;

            if (player != null)
                player.position = startPosition;

            ResetWorld();

            Debug.Log("Loop restarted! Remaining chances: " + loopChances);
        }

        private void ResetWorld()
        {
            // TODO: Reset enemies, pickups, etc.
        }

        private void IncreaseDifficulty()
        {
            Debug.Log("Difficulty increased!");

            // Example: scale player + enemy speeds
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.moveSpeed *= playerSpeedMultiplier;

            foreach (EnemyController e in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
            {
                e.moveSpeed *= enemySpeedMultiplier;
            }
        }

        private List<EnemyController> enemies = new List<EnemyController>();
        public void RegisterEnemy(EnemyController enemy) => enemies.Add(enemy);
        public void UnregisterEnemy(EnemyController enemy) => enemies.Remove(enemy);

       
    }
}
