using System.Collections.Generic;
using UnityEngine;

namespace PrototypeOne
{
    public class FactionManager : MonoBehaviour
    {
        public static FactionType playerFaction = FactionType.Grey;

        // Global infection tracking
        public static int totalOccupants { get; private set; } = 0;
        public static int infectedCount { get; private set; } = 0;
        public static float InfectionRatio => totalOccupants == 0 ? 0f : (float)infectedCount / totalOccupants;

        // Faction data maps
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

        public static readonly Dictionary<FactionType, int> factionLayers = new()
        {
            { FactionType.Yellow, 11 },
            { FactionType.Cyan, 9 },
            { FactionType.Magenta, 10 },
            { FactionType.Grey, 3 },
        };

        // --- Infection management ---
        public static void RegisterOccupant() => totalOccupants++;
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

        // --- Faction visuals ---
        public static Color GetColor(FactionType faction) =>
            factionColors.TryGetValue(faction, out var color) ? color : Color.white;

        public static ShapeType GetShape(FactionType faction) =>
            factionShapes.TryGetValue(faction, out var shape) ? shape : ShapeType.Square;

        public static int GetLayer(FactionType faction) =>
            factionLayers.TryGetValue(faction, out var layer) ? layer : 0;

#if UNITY_EDITOR
        // Debug helper: count outposts per faction layer
        public void DebugCheckOutpostsOnLayers()
        {
            Dictionary<FactionType, int> outpostCounts = new();
            foreach (FactionType faction in System.Enum.GetValues(typeof(FactionType)))
                outpostCounts[faction] = 0;

            GameObject[] outposts = GameObject.FindGameObjectsWithTag("Outpost");
            foreach (GameObject outpost in outposts)
            {
                int layer = outpost.layer;
                foreach (var kvp in factionLayers)
                {
                    if (kvp.Value == layer)
                    {
                        outpostCounts[kvp.Key]++;
                        break;
                    }
                }
            }

            foreach (var kvp in outpostCounts)
                Debug.Log($"Faction {kvp.Key} has {kvp.Value} outposts on layer {GetLayer(kvp.Key)}");
        }
#endif
    }
}
