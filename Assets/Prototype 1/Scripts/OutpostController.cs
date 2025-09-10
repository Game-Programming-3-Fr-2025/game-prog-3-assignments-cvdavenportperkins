using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PrototypeOne
{

    public class OutpostController : MonoBehaviour
    {
        [SerializeField] private GameObject dischargeEffectPrefab;
        [SerializeField] private float dischargeDamage = 1f;
        [SerializeField] private Transform visualsRoot;

        public FactionType faction;
        private bool nodeCaptured = false;

        private GameObject shapeBounds;
        private Collider2D boundsCollider;

        [SerializeField] private CircleCollider2D challengeCollider;

        private readonly List<OccupantController> occupants = new();

        public void ApplyFactionVisuals(ShapeType shape, Color color)
        {
            var visualController = GetComponentInChildren<ShapeVisualController>();
            if (visualController != null)
            {
                visualController.SetShape(shape);
                visualController.SetColor(color);
            }
        }
        public void Initialize(OutpostConfig config)
        {
            faction = config.faction;

            SetupCollider(config);
            ApplyFactionVisuals(config.shape, config.color);
            SpawnOutpost(config.faction, transform.position, config.levelIndex, config);
        }

        private void CreateShapeBounds()
        {
            shapeBounds = new GameObject("ShapeBounds");
            shapeBounds.transform.SetParent(transform);
            shapeBounds.transform.localPosition = Vector3.zero;
        }

        private void SetupCollider(OutpostConfig config)
        {
            CreateShapeBounds();

            // Challenge radius on root
            challengeCollider = GetComponent<CircleCollider2D>() ?? gameObject.AddComponent<CircleCollider2D>();
            challengeCollider.isTrigger = true;
            challengeCollider.radius = config.inputChallengeRadius;

            float radius = config.colliderRadius;

            switch (config.shape)
            {
                case ShapeType.Circle:
                    boundsCollider = CreateCircleCollider(shapeBounds, radius);
                    break;

                case ShapeType.Square:
                    boundsCollider = CreatePolygonCollider(shapeBounds, GetSquarePoints(radius));
                    break;

                case ShapeType.Triangle:
                    boundsCollider = CreatePolygonCollider(shapeBounds, GetTrianglePoints(radius));
                    break;
            }

            // Scale visuals to match collider
            float diameter = radius * 2f;
            visualsRoot.localScale = new Vector3(diameter, diameter, 1f);
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

        public void SpawnOutpost(FactionType factionType, Vector3 location, int levelIndex, OutpostConfig config)
        {
            int occupantCount = Mathf.Clamp(config.occupantCount, config.minOccupants, config.maxOccupants);

            for (int i = 0; i < occupantCount; i++)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                float distance = Random.Range(0.5f, config.colliderRadius * 0.9f);
                Vector3 spawnPosition = location + new Vector3(randomDirection.x, randomDirection.y, 0f) * distance;

                GameObject occupant = Instantiate(Resources.Load<GameObject>("OccupantPrefab"), spawnPosition, Quaternion.identity);
                var occupantController = occupant.GetComponent<OccupantController>();

                occupantController.outpostCenter = this.transform;
                occupantController.roamRadius = 3f;
                occupantController.roamSpeed = Random.Range(1f, 3f);
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
                    }
                    else
                    {
                        occupant.FlashTick(Color.white);
                    }
                }

                yield return new WaitForSeconds(pulseInterval);

                if (occupants.Count > 0 && occupants.TrueForAll(o => o == null || o.isInfected))
                {
                    nodeCaptured = true;
                    UpdateNodeUI();
                    GameManager.Instance?.OnOutpostCaptured();
                    Debug.Log("Outpost Captured!");
                }
            }
        }

        public void AccessGranted()
        {
            StopAllCoroutines();
            StartCoroutine(InfectionPulse(0.5f));
        }

        public void UpdateNodeUI()
        {
            Light2D outpostLight = GetComponentInChildren<Light2D>();
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
            foreach (var occ in occupants)
            {
                if (occ == null) continue;
                occ.EnforceBounds(boundsCollider);
            }
        }



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