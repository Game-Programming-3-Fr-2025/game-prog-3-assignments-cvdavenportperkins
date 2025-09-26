using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace PrototypeOne
{
    public class OutpostSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject outpostPrefab; 
        [SerializeField] private OutpostConfig outpostConfig; 
        [SerializeField] private float playerColliderRadius = 1.66f;

        [Header("Spawner Settings")]
        public float spawnBuffer;
        [SerializeField] private float baseWorldWidth = 100f;
        [SerializeField] private float baseWorldHeight = 100f;

        public static int currentTier = 0;
        private int clearedTiers = 0;
        private int outpostsToSpawn = 3;

        private float worldHeight;
        private float worldWidth;

        // Track placed outposts to avoid overlap
        private readonly List<(Vector3 pos, float radius)> placedOutposts = new();

        private void Awake()
        {
            spawnBuffer = playerColliderRadius * 5f;

        }

        private void Start()
        {
            SpawnTier();

            Bounds bounds = GameManager.Instance.GetWorldBounds();
            worldWidth = bounds.size.x;
            worldHeight = bounds.size.y;
        }

        // Generates a randomized OutpostConfig for spawning.
        private OutpostConfig GenerateConfig()
        {
            int occupantCount = Random.Range(3, 8);
     
            // Example: collider radius could be tied to shape or faction, here randomized for variety
            float colliderRadius = Random.Range(1.5f, 3f);
            float inputChallengeRadius = colliderRadius * 1.2f;

            List<FactionType> validFactions = new()
            {
                FactionType.Cyan,
                FactionType.Magenta,
                FactionType.Yellow,
            };

            FactionType faction = validFactions[Random.Range(0, validFactions.Count)];
            ShapeType shape = FactionManager.GetShape(faction);
            Color color = FactionManager.GetColor(faction);

            return new OutpostConfig(
                occupantCount,
                faction,
                colliderRadius,
                shape,
                color,
                inputChallengeRadius
            );
        }

        // Finds a valid spawn position that doesn't overlap existing outposts.
        private Vector3 GetValidPosition(float radius)
        {
            Vector3 candidate;
            int attempts = 0;

            do
            {
                candidate = GetRandomPositionInField(radius);
                attempts++;
            }
            while (!IsValidSpawnCandidate(candidate, radius) && attempts < 100);

            if (attempts >= 100)
                Debug.LogWarning("Failed to place outpost without overlap");

            return candidate;
        }

        // Checks if a candidate position is far enough from all placed outposts.
        private bool IsValidSpawnCandidate(Vector3 candidate, float newChallengeRadius)
        {
            // Check world bounds
            if (Mathf.Abs(candidate.x) + newChallengeRadius > worldWidth / 2f ||
                Mathf.Abs(candidate.y) + newChallengeRadius > worldHeight / 2f)
                return false;

            foreach (var (pos, otherChallengeRadius) in placedOutposts)
            {
                float minDistance = newChallengeRadius + otherChallengeRadius + (playerColliderRadius * 2f);
                if (Vector3.Distance(candidate, pos) < minDistance)
                    return false;
            }

            return true;
        }


        private Vector3 GetRandomPositionInField(float radius)
        {
            float x = Random.Range(-worldWidth / 2f + radius, worldWidth / 2f - radius);
            float y = Random.Range(-worldHeight / 2f + radius, worldHeight / 2f - radius);
            return new Vector3(x, y, 0f);
        }

        void SpawnOutpost(Vector3 position, OutpostConfig config)
        {
            GameObject outpostGO = Instantiate(outpostPrefab, position, Quaternion.identity);
            OutpostController controller = outpostGO.GetComponent<OutpostController>();
            if (controller != null)
            {
                controller.Initialize(outpostConfig);
            }
        }

        public void OnTierCleared(bool withinTimeLimit)
        {
            if (withinTimeLimit)
            {
                clearedTiers++;
                currentTier++;

                //Increase outpost count
                outpostsToSpawn = Mathf.Min(3 + (2 * currentTier), 15);

                //Expand level size
                if (clearedTiers % 3 == 0)
                {
                    worldWidth *= 1.15f;
                    worldHeight *= 1.15f;
                    GameManager.Instance.ExtendGameClock(30f);
                }
            }
            else
            {
                //Reset on fail
                currentTier = 0;
                clearedTiers = 0;
                outpostsToSpawn = 0;
                worldWidth = baseWorldWidth;
                worldHeight = baseWorldHeight;  
            }

            placedOutposts.Clear();
            SpawnTier();
        }

        public void SpawnTier()
        {
            for (int i = 0; i < outpostsToSpawn; i++)
            {
                OutpostConfig config = GenerateConfig();
                Vector3 spawnPos = GetValidPosition(config.inputChallengeRadius);
                
                SpawnOutpost(spawnPos, config);

                GameObject outpostGO = Instantiate(outpostPrefab, spawnPos, Quaternion.identity);
                var controller = outpostGO.GetComponent<OutpostController>();
                controller.Initialize(config);

                placedOutposts.Add((spawnPos, config.inputChallengeRadius));
                GameManager.Instance?.RegisterOutpost();
            }      
        }

        private void LateUpdate()
        {
           
        }
    }

}          