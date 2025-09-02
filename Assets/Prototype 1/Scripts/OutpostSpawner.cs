using UnityEngine;
using System.Collections.Generic;

public class OutpostSpawner : MonoBehaviour
{
    public GameObject outpostPrefab;
    public int totalOutposts = 12;
    public float spawnBuffer = 0.5f;

    private List<Vector3> placedPositions = new List<Vector3>();

    void Start()
    {
        for (int i = 0; i < totalOutposts; i++)
        {
            OutpostConfig config = GenerateConfig(); // Occupant count, faction, shape
            Vector3 spawnPos = GetValidPosition(config.colliderRadius);
            GameObject outpostGO = Instantiate(outpostPrefab, spawnPos, Quaternion.identity);
            outpostGO.GetComponent<OutpostController>().Initialize(config);
            placedPositions.Add(spawnPos);
        }
    }

    OutpostConfig GenerateConfig()
    {
        int occupantCount = Random.Range(3, 10);
        FactionType faction = (FactionType)Random.Range(0, System.Enum.GetValues(typeof(FactionType)).Length);
        float colliderRadius = Mathf.Sqrt((2f * occupantCount) / Mathf.PI) + spawnBuffer;
        ShapeType shape = GetShapeForFaction(faction);
        Color color = GetColorForFaction(faction);
        return new OutpostConfig(occupantCount, faction, colliderRadius, shape, color);
    }

    private Vector3 GetValidPosition(float radius)
    {
        Vector3 candidate;
        int attempts = 0;
        do
        {
            candidate = GetRandomPositionInHelixField();
            attempts++;
        } 
        while (IsValidSpawnCandidate(candidate, radius) && attempts < 100);
        
        if (attempts >= 100) // Prevent infinite loop
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

    private Vector3 GetRandomPositionInHelixField()
    {
        
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 0f, z);
    }

    private ShapeType GetShapeForFaction(FactionType faction)
    {
        switch (faction)
        {
            case FactionType.Yellow: return ShapeType.Triangle;
            case FactionType.Cyan: return ShapeType.Circle;
            case FactionType.Magenta: return ShapeType.Square;
            default: return ShapeType.Square;
        };
    }

    Color GetColorForFaction(FactionType faction)
    {
        switch (faction)
        {
            case FactionType.Yellow: return Color.yellow;
            case FactionType.Cyan: return Color.cyan;
            case FactionType.Magenta: return Color.magenta;
            default: return Color.white;
        }
    }

}
