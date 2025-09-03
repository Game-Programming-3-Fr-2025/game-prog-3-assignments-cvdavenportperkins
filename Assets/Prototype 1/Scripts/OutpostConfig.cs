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


    public float infectionPulseRate;
    public int levelIndex;
    public int maxOccupants;
    public int minOccupants;

    public OutpostConfig(int occupantCount, FactionType faction, float colliderRadius, ShapeType shape, Color color)
    {
        this.occupantCount = occupantCount;
        this.faction = faction;
        this.colliderRadius = Mathf.Sqrt((2f * occupantCount) / Mathf.PI);
        this.shape = shape;
        this.color = color;
    }
}
