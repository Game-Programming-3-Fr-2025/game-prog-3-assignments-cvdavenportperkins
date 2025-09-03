using System.Collections.Generic;
using UnityEngine;

public class OutpostSpawner : MonoBehaviour
{
    public GameObject outpostPrefab;
    public int totalOutposts = 12;
    public float spawnBuffer = 0.5f;

    private readonly List<Vector3> placedPositions = new();

    void Start()
    {
        for (int i = 0; i < totalOutposts; i++)
        {
            OutpostConfig config = GenerateConfig();
            Vector3 spawnPos = GetValidPosition(config.colliderRadius);
            GameObject outpostGO = Instantiate(outpostPrefab, spawnPos, Quaternion.identity);
            var controller = outpostGO.GetComponent<OutpostController>();
            controller.Initialize(config);
            placedPositions.Add(spawnPos);

            if (GameManager.Instance != null) GameManager.Instance.RegisterOutpost();
        }
    }

    OutpostConfig GenerateConfig()
    {
        int occupantCount = Random.Range(3, 10);
        FactionType faction = (FactionType)Random.Range(0, System.Enum.GetValues(typeof(FactionType)).Length);
        ShapeType shape = FactionManager.GetShape(faction);
        Color color = FactionManager.GetColor(faction);
        return new OutpostConfig(occupantCount, faction, spawnBuffer, shape, color);
    }

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

    bool IsValidSpawnCandidate(Vector3 candidate, float radius)
    {
        foreach (var pos in placedPositions)
        {
            float minDistance = radius + spawnBuffer;
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