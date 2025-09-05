using System.Collections.Generic;
using UnityEngine;

public class OutpostSpawner : MonoBehaviour
{
    [SerializeField] private float playerColliderRadius = 1.5f;
    
    [Header("Spawner Settings")]
    public GameObject outpostPrefab;
    public int totalOutposts = 12;
    public float spawnBuffer;

    // Track placed outposts to avoid overlap
    private readonly List<(Vector3 pos, float radius)> placedOutposts = new();


    private void Awake()
    {
        spawnBuffer = playerColliderRadius;
    }
    private void Start()
    {
        for (int i = 0; i < totalOutposts; i++)
        {
            OutpostConfig config = GenerateConfig();
            Vector3 spawnPos = GetValidPosition(config.colliderRadius);

            GameObject outpostGO = Instantiate(outpostPrefab, spawnPos, Quaternion.identity);
            var controller = outpostGO.GetComponent<OutpostController>();
            controller.Initialize(config);

            placedOutposts.Add((spawnPos, config.colliderRadius));

            GameManager.Instance?.RegisterOutpost();
        }
    }

       // Generates a randomized OutpostConfig for spawning.
    private OutpostConfig GenerateConfig()
    {
        int occupantCount = Random.Range(3, 10);
        FactionType faction = (FactionType)Random.Range(0, System.Enum.GetValues(typeof(FactionType)).Length);
        ShapeType shape = FactionManager.GetShape(faction);
        Color color = FactionManager.GetColor(faction);

        // Example: collider radius could be tied to shape or faction, here randomized for variety
        float colliderRadius = Random.Range(1.5f, 3f);
        float inputChallengeRadius = colliderRadius + 0.5f;

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
            candidate = GetRandomPositionInField();
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
        foreach (var (pos, otherChallengeRadius) in placedOutposts)
        {
            float minDistance = newChallengeRadius + otherChallengeRadius + (playerColliderRadius * 2f);
            if (Vector3.Distance(candidate, pos) < minDistance)
                return false;
        }
        return true;
    }

    private Vector3 GetRandomPositionInField()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-10f, 10f);
        return new Vector3(x, y, 0f);
    }
}