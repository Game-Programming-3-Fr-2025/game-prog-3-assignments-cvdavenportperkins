using PrototypeOne;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PrototypeOne
{

    public class UniversalPrefabValidator : EditorWindow
    {
        [MenuItem("Tools/Validate Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<UniversalPrefabValidator>("Prefab Validator");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Validate All Prefabs"))
            {
                ValidateOutpostPrefab();
                ValidateOccupantPrefab();
                ValidatePlayerPrefab();
                ValidateInputChallengePrefab();
            }
        }

        private void ValidateOutpostPrefab()
        {
            GameObject prefab = Resources.Load<GameObject>("OutpostPrefab");
            if (prefab == null)
            {
                Debug.LogError("OutpostPrefab not found in Resources.");
                return;
            }

            Debug.Log("Validating OutpostPrefab...");

            string[] requiredChildren = { "ChallengeCollider", "ShapeBounds" };
            foreach (string childName in requiredChildren)
            {
                Transform child = prefab.transform.Find(childName);
                if (child == null)
                    Debug.LogError($"OutpostPrefab missing child: {childName}");
            }

            if (prefab.GetComponentInChildren<ShapeVisualController>() == null)
                Debug.LogError("OutpostPrefab missing ShapeVisualController.");

            if (prefab.GetComponentInChildren<Light2D>() == null)
                Debug.LogWarning("OutpostPrefab missing Light2D (optional).");

            Debug.Log("OutpostPrefab validation complete.");
        }

        private void ValidateOccupantPrefab()
        {
            GameObject prefab = Resources.Load<GameObject>("OccupantPrefab");
            if (prefab == null)
            {
                Debug.LogError("OccupantPrefab not found in Resources.");
                return;
            }

            Debug.Log("Validating OccupantPrefab...");

            if (prefab.GetComponent<SpriteRenderer>() == null)
                Debug.LogError("OccupantPrefab missing SpriteRenderer.");

            if (prefab.GetComponent<OccupantController>() == null)
                Debug.LogError("OccupantPrefab missing OccupantController.");

            if (prefab.GetComponent<Animator>() == null)
                Debug.LogWarning("OccupantPrefab missing Animator (optional).");

            Debug.Log("OccupantPrefab validation complete.");
        }

        private void ValidatePlayerPrefab()
        {
            GameObject prefab = Resources.Load<GameObject>("PlayerPrefab");
            if (prefab == null)
            {
                Debug.LogError("PlayerPrefab not found in Resources.");
                return;
            }

            Debug.Log("Validating PlayerPrefab...");

            if (prefab.GetComponent<PlayerHealth>() == null)
                Debug.LogError("PlayerPrefab missing PlayerHealth.");

            if (prefab.GetComponent<Rigidbody2D>() == null)
                Debug.LogError("PlayerPrefab missing Rigidbody2D.");

            if (prefab.GetComponent<Collider2D>() == null)
                Debug.LogError("PlayerPrefab missing Collider2D.");

            Debug.Log("PlayerPrefab validation complete.");
        }

        private void ValidateInputChallengePrefab()
        {
            GameObject prefab = Resources.Load<GameObject>("InputChallenge");
            if (prefab == null)
            {
                Debug.LogError("InputChallenge prefab not found in Resources.");
                return;
            }

            Debug.Log("Validating InputChallenge...");

            if (prefab.GetComponent<InputChallengeController>() == null)
                Debug.LogError("InputChallenge prefab missing InputChallenge script.");

            if (prefab.GetComponent<Collider2D>() == null)
                Debug.LogWarning("InputChallenge prefab missing Collider2D (optional).");

            Debug.Log("InputChallenge validation complete.");
        }
    }
}