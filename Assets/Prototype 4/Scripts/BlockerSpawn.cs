using UnityEngine;

namespace PrototypeFour
{
    public class BlockerSpawner : MonoBehaviour
    {
        public GameObject blockerPrefab;
        public float spawnInterval = 2f;
        private LevelWorld levelWorld;

        void Start()
        {
            levelWorld = FindFirstObjectByType<LevelWorld>();
            InvokeRepeating("SpawnBlocker", 1f, spawnInterval);
        }

        void SpawnBlocker()
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(levelWorld.spawnAreaMin.x, levelWorld.spawnAreaMax.x),
                Random.Range(levelWorld.spawnAreaMin.y, levelWorld.spawnAreaMax.y)
            );
            Instantiate(blockerPrefab, spawnPos, Quaternion.identity);
        }
    }
}