using UnityEngine;

namespace PrototypeFour
{
    public class LevelWorld : MonoBehaviour
    {
        [Header("Spawn Area")]
        public Vector2 spawnAreaMin;
        public Vector2 spawnAreaMax;

        [Header("Enemy Settings")]
        public float enemyBaseSpeed = 2f;
        public float enemyMaxSpeed = 6f;
        public float accelerationWindow = 5f;

        [Header("Level Timing")]
        public float levelDuration = 30f;
    }
}

