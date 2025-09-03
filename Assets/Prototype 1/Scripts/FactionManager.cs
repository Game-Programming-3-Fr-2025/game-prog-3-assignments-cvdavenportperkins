using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static int totalOccupants = 0;
    public static int infectedCount = 0;
    public static float InfectionRatio => totalOccupants ==0 ? 0f : (float)infectedCount / totalOccupants;

    public static void ReportInfection()
    {
        infectedCount++;
        Debug.Log($"Infection reported. Current ratio: {InfectionRatio:P0}"); 
    }

    public static void ResetInfection()
    {
        infectedCount = 0;
        totalOccupants = 0;
        Debug.Log("Infection state reset");
    }

   public static readonly Dictionary<FactionType, Color> factionColors = new()
    {
        { FactionType.Yellow, Color.yellow },
        { FactionType.Cyan, Color.cyan },
        { FactionType.Magenta, Color.magenta }
    };

    public static readonly Dictionary<FactionType, ShapeType> factionShapes = new()
    {
        { FactionType.Yellow, ShapeType.Triangle },
        { FactionType.Cyan, ShapeType.Circle },
        { FactionType.Magenta, ShapeType.Square }
    };

    public static Color GetColor(FactionType faction)
    {
        return factionColors.TryGetValue(faction, out var color) ? color : default(Color);
    }

    public static ShapeType GetShape(FactionType faction)
    {
        return factionShapes.TryGetValue(faction, out var shape) ? shape : default(ShapeType);
    }
     
}
