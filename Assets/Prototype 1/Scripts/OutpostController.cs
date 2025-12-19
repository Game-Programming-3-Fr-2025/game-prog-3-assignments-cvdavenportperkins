using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PrototypeOne
{
    public class OutpostController : MonoBehaviour
    {
        [Header("Visuals & FX")]
        [SerializeField] private Transform visualsRoot;
        [SerializeField] private Light2D light2D;
        [SerializeField] private GameObject dischargeEffectPrefab;

        [Header("Combat")]
        [SerializeField] private float dischargeDamage = 1f;

        [Header("Colliders")]
        [SerializeField] private CircleCollider2D challengeCollider;

        // NEW: Serialized references to prefab colliders in ShapeBounds
        [Header("Shape Bounds Colliders")]
        [SerializeField] private CapsuleCollider2D capsuleCollider;
        [SerializeField] private CircleCollider2D circleCollider;
        [SerializeField] private PolygonCollider2D polygonCollider;

        // Runtime state
        public FactionType faction;
        private bool nodeCaptured = false;

        // Shape bounds root and physics collider used for occupant confinement
        private GameObject shapeBounds;
        private Collider boundsCollider;

        // Occupant tracking
        private readonly List<OccupantController> occupants = new();

        // Optional occupant constraints (fallback if provided in config)
        public int minOccupants;
        public int maxOccupants;

        private void Awake()
        {
            if (light2D == null)
                light2D = GetComponentInChildren<Light2D>();
        }

        private void Start()
        {

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var c = sr.color;
                c.a = sr.color.a;
                sr.color = c;
            }
        }

        public void Initialize(OutpostConfig config)
        {
            if (config.faction == FactionType.Grey)
            {
                Debug.LogWarning("Grey faction is player-only. Outpost will be destroyed.");
                Destroy(gameObject);
                return;
            }

            faction = config.faction;

            if (light2D != null)
                light2D.color = config.color;

            ApplyFactionVisuals(config.shape, config.color);

            // NEW: Use prefab colliders instead of runtime AddComponent
            SetupCollider(config);

            SpawnOutpost(config.faction, transform.position, config.levelIndex, config);
        }

        public void ApplyFactionVisuals(ShapeType shape, Color color)
        {
            var visualController = GetComponentInChildren<ShapeVisualController>();
            if (visualController != null)
            {
                visualController.SetShape(shape);
                visualController.SetColor(color);
            }

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                color.a = sr.color.a;
                sr.color = color;
            }
        }

        /// <summary>
        /// Sets up challenge collider and enables the correct shape bounds collider.
        /// </summary>
        private void SetupCollider(OutpostConfig config)
        {
            // Challenge collider setup
            if (challengeCollider == null)
            {
                Transform challengeTransform = transform.Find("ChallengeCollider");
                if (challengeTransform == null)
                {
                    Debug.LogError("ChallengeCollider child not found on Outpost prefab.");
                    return;
                }
                challengeCollider = challengeTransform.GetComponent<CircleCollider2D>();
            }

            if (challengeCollider != null)
            {
                challengeCollider.isTrigger = true;
                challengeCollider.radius = config.inputChallengeRadius;
            }

            // Disable all shape colliders first
            if (capsuleCollider) capsuleCollider.enabled = false;
            if (circleCollider) circleCollider.enabled = false;
            if (polygonCollider) polygonCollider.enabled = false;

            // Enable only the collider that matches the shape
            switch (config.shape)
            {
                case ShapeType.Circle:
                    if (circleCollider) { circleCollider.enabled = true; boundsCollider = circleCollider; }
                    break;

                case ShapeType.Capsule:
                    if (capsuleCollider) { capsuleCollider.enabled = true; boundsCollider = capsuleCollider; }
                    break;

                case ShapeType.Triangle:
                case ShapeType.Square:
                    if (polygonCollider) { polygonCollider.enabled = true; boundsCollider = polygonCollider; }
                    break;

                default:
                    if (circleCollider) { circleCollider.enabled = true; boundsCollider = circleCollider; }
                    break;
            }

            // Scale visuals to match collider radius
            if (visualsRoot != null)
            {
                float diameter = config.colliderRadius * 2f;
                visualsRoot.localScale = new Vector3(diameter, diameter, 1f);
            }
        }


        private void EnsureShapeBounds()
        {
            // Reuse existing child if present
            var existing = transform.Find("ShapeBounds");
            if (existing != null)
            {
                shapeBounds = existing.gameObject;
                return;
            }

            shapeBounds = new GameObject("ShapeBounds");
            shapeBounds.transform.SetParent(transform);
            shapeBounds.transform.localPosition = Vector3.zero;
        }

        private CircleCollider2D CreateCircleCollider(GameObject parent, float radius)
        {
            var circle = parent.AddComponent<CircleCollider2D>();
            circle.radius = radius;
            circle.isTrigger = false;
            return circle;
        }

        private PolygonCollider2D CreatePolygonCollider(GameObject parent, Vector2[] points)
        {
            var poly = parent.AddComponent<PolygonCollider2D>();
            poly.points = points;
            poly.isTrigger = false;
            return poly;
        }

        private Vector2[] GetSquarePoints(float radius)
        {
            return new Vector2[]
            {
                new Vector2(-radius, -radius),
                new Vector2(radius, -radius),
                new Vector2(radius, radius),
                new Vector2(-radius, radius)
            };
        }

        private Vector2[] GetTrianglePoints(float radius)
        {
            float height = Mathf.Sqrt(3f) * radius;
            return new Vector2[]
            {
                new Vector2(-radius, -height / 3f),
                new Vector2(radius, -height / 3f),
                new Vector2(0f, 2f * height / 3f)
            };
        }

        /// <summary>
        /// Spawns occupant entities within the outpost bounds and registers them.
        /// </summary>
        public void SpawnOutpost(FactionType factionType, Vector3 location, int levelIndex, OutpostConfig config)
        {
            // Determine occupant count. Use config.occupantCount primarily; clamp if config has limits.
            int occupantCount = config.occupantCount;
            if (config.minOccupants > 0 && config.maxOccupants > 0 && config.maxOccupants >= config.minOccupants)
            {
                occupantCount = Mathf.Clamp(occupantCount, config.minOccupants, config.maxOccupants);
            }
            else if (maxOccupants > 0 && minOccupants > 0 && maxOccupants >= minOccupants)
            {
                // Fallback to controller fields if config doesn't provide min/max
                occupantCount = Mathf.Clamp(occupantCount, minOccupants, maxOccupants);
            }

            // Cache prefab from Resources to avoid repeated lookups
            GameObject occupantPrefab = Resources.Load<GameObject>("OccupantPrefab");
            if (occupantPrefab == null)
            {
                Debug.LogError("Resources.Load<OccupantPrefab> failed. Ensure the prefab exists at Resources/OccupantPrefab.");
                return;
            }

            // Spawn within collider radius
            for (int i = 0; i < occupantCount; i++)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float distance = Random.Range(0.5f, config.colliderRadius * 0.9f);
                Vector3 spawnPosition = location + new Vector3(randomDirection.x, randomDirection.y, 0f) * distance;

                GameObject occupant = Instantiate(occupantPrefab, spawnPosition, Quaternion.identity);
                var occupantController = occupant.GetComponent<OccupantController>();
                if (occupantController == null)
                {
                    Debug.LogError("Occupant prefab missing OccupantController component.");
                    continue;
                }

                occupantController.outpostCenter = this.transform;
                occupantController.roamRadius = Mathf.Max(3f, config.colliderRadius * 0.8f);
                occupantController.roamSpeed = Random.Range(1f, 3f);

                // Infection parameters (could be moved to config if desired)
                occupantController.infectionRadius1 = 2f;
                occupantController.infectionRadius2 = 4f;
                occupantController.infectionChance1 = 0.33f;
                occupantController.infectionChance2 = 0.50f;

                occupantController.animator = occupant.GetComponent<Animator>();
                occupantController.faction = factionType;
                occupantController.currentColor = FactionManager.GetColor(factionType);

                var sr = occupant.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = FactionManager.GetColor(factionType);

                occupants.Add(occupantController);
            }
        }

        /// <summary>
        /// Starts the infection pulse loop when access is granted.
        /// </summary>
        public void AccessGranted()
        {
            StopAllCoroutines();
            StartCoroutine(InfectionPulse(0.5f));
        }

        private IEnumerator InfectionPulse(float pulseInterval)
        {
            while (!nodeCaptured)
            {
                foreach (var occupant in occupants)
                {
                    if (!occupant || occupant.isInfected) continue;

                    bool infected = false;
                    float distance = Vector3.Distance(transform.position, occupant.transform.position);

                    if (distance <= occupant.infectionRadius1 && Random.value < occupant.infectionChance1) infected = true;
                    if (distance <= occupant.infectionRadius2 && Random.value < occupant.infectionChance2) infected = true;

                    if (infected)
                    {
                        occupant.Infect();
                        occupant.FlashTick(Color.red);
                        GameManager.Instance.AddScore(25);
                    }
                    else
                    {
                        occupant.FlashTick(Color.white);
                    }
                }

                yield return new WaitForSeconds(pulseInterval);

                // If all occupants are infected or destroyed, capture node
                if (occupants.Count > 0 && occupants.TrueForAll(o => o == null || o.isInfected))
                {
                    nodeCaptured = true;
                    UpdateNodeUI();
                    GameManager.Instance?.OnOutpostCaptured();
                    Debug.Log("Outpost Captured!");
                    GameManager.Instance.AddScore(500);
                }
            }
        }

        /// <summary>
        /// Updates visuals on capture.
        /// </summary>
        public void UpdateNodeUI()
        {
            var outpostLight = light2D != null ? light2D : GetComponentInChildren<Light2D>();
            if (outpostLight != null)
            {
                outpostLight.color = Color.red;
                outpostLight.intensity = 2f;
            }

            foreach (var occupant in occupants)
            {
                if (occupant == null) continue;
                var sr = occupant.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = Color.red;
                occupant.animator?.SetTrigger("Captured");
            }
        }

        private void Update()
        {
            if (boundsCollider == null) return;

            // Keep occupants within solid bounds
            foreach (var occ in occupants)
            {
                if (occ == null) continue;
                occ.EnforceBounds(boundsCollider);
            }
        }

        /// <summary>
        /// Triggers a discharge effect and damages player if within range.
        /// </summary>
        public void TriggerDischarge(Vector3 targetPosition)
        {
            if (dischargeEffectPrefab != null)
            {
                Instantiate(dischargeEffectPrefab, targetPosition, Quaternion.identity);
            }

            var hit = Physics2D.OverlapCircle((Vector2)targetPosition, 0.5f, LayerMask.GetMask("Player"));
            if (hit != null && hit.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(dischargeDamage);
            }
        }
    }
}
