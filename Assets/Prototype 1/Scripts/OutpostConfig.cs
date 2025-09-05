using UnityEngine;

[System.Serializable]
public class OutpostConfig
{
    public int occupantCount;
    public FactionType faction;
    public ShapeType shape;
    public Color color;

    public float colliderRadius;
    public float inputChallengeRadius;
    public float boundsColliderRadius;

    public float infectionPulseRate = 0.5f;
    public int levelIndex;
    public int maxOccupants = 10;
    public int minOccupants = 3;

    /// <summary>
    /// Creates a pure-data config for an outpost.
    /// </summary>
    public OutpostConfig(
        int occupantCount,
        FactionType faction,
        float boundsColliderRadius,
        ShapeType shape,
        Color color,
        float spawnBuffer)
    {
        // Clamp occupant count to at least 1
        this.occupantCount = Mathf.Max(1, occupantCount);
        this.faction = faction;
        this.boundsColliderRadius = boundsColliderRadius;
        this.shape = shape;
        this.color = color;

        // Radius scales with count; add a buffer for spacing
        float baseRadius = Mathf.Sqrt((2f * this.occupantCount) / Mathf.PI);
        this.colliderRadius = baseRadius + Mathf.Max(0f, spawnBuffer);

        // Challenge radius slightly larger than interaction space
        this.inputChallengeRadius = this.colliderRadius + 1.0f;
    }
}