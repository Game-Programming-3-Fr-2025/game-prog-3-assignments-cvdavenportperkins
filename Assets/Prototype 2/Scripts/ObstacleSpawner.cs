using Unity.VisualScripting;
using UnityEngine;

namespace PrototypeTwo
{

    public class ObstacleSpawner : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;
        public Transform player;
        public float spawnInterval = 2f;
        public float minX = -2.5f;
        public float maxX = 2.5f;
        public float spawnYOffset = -10f;

        private float nextSpawnTime;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time > nextSpawnTime)
            {
                SpawnObstacle();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }

        void SpawnObstacle()
        {
            if (obstaclePrefabs.Length == 0 || player == null) return;

            float randomX = Random.Range(minX, maxX);
            Vector3 spawnPos = new Vector3(randomX, player.position.y + spawnYOffset, 0f);

            Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], spawnPos, Quaternion.identity);
        }
    }

}
