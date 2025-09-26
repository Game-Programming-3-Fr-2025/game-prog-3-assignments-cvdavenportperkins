using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

namespace PrototypeOne
{

    public class FactionManager : MonoBehaviour
    {

        public FactionType playerFaction = FactionType.Grey;
        public static int totalOccupants = 0;
        public static int infectedCount = 0;
        public static float InfectionRatio => totalOccupants == 0 ? 0f : (float)infectedCount / totalOccupants;

        public void SetPlayerFaction(FactionType faction)
        {
            playerFaction = faction;
            Debug.Log($"PlayerFaction set to {faction} with {GetColor(faction)} and {GetShape(faction)}");
        }
        
        public static void RegisterOccupant()
        {
            totalOccupants++;
        }

        public static void UnregisterOccupant(bool wasInfected)
        {
            totalOccupants = Mathf.Max(0, totalOccupants - 1);
            if (wasInfected) infectedCount = Mathf.Max(0, infectedCount - 1);
        }

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
        { FactionType.Magenta, Color.magenta },
        { FactionType.Grey, Color.grey }
    };

        public static readonly Dictionary<FactionType, ShapeType> factionShapes = new()
    {
        { FactionType.Yellow, ShapeType.Triangle },
        { FactionType.Cyan, ShapeType.Circle },
        { FactionType.Magenta, ShapeType.Square },
        { FactionType.Grey, ShapeType.Capsule }
    };

        public static Color GetColor(FactionType faction)
        {
            return factionColors.TryGetValue(faction, out var color) ? color : Color.white;
        }

        public static ShapeType GetShape(FactionType faction)
        {
            return factionShapes.TryGetValue(faction, out var shape) ? shape : ShapeType.Square;
        }
    }
}