using UnityEngine;

[System.Serializable]
public class OutpostConfig
{
    public int occupantCount;
    public FactionType faction;
    public float colliderRadius;
    public ShapeType shape;
    public Color color;
    public float inputChallengeRadius;

    public float infectionPulseRate = 0.5f;
    public int levelIndex;
    public int maxOccupants = 10;
    public int minOccupants = 3;

    public OutpostConfig(int occupantCount, FactionType faction, float spawnBuffer, ShapeType shape, Color color)
    {
        this.occupantCount = Mathf.Max(1, occupantCount);
        this.faction = faction;
        this.shape = shape;
        this.color = color;

        // Radius scales with count; add a small buffer for spacing
        float baseRadius = Mathf.Sqrt((2f * this.occupantCount) / Mathf.PI);
        this.colliderRadius = baseRadius + Mathf.Max(0f, spawnBuffer);

        // Challenge radius slightly larger than interaction space
        this.inputChallengeRadius = this.colliderRadius + 1.0f;
    }
}