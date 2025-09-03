using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutpostController : MonoBehaviour
{
    [SerializeField] private GameObject dischargeEffectPrefab;
    [SerializeField] private float dischargeDamage = 1f;

    public FactionType faction;
    private bool nodeCaptured = false;

    private GameObject shapeBounds;
    private Collider2D boundsCollider;

    [SerializeField] private CircleCollider2D challengeCollider;

    private readonly List<OccupantController> occupants = new();

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
                if (GameManager.Instance != null) GameManager.Instance.OnOutpostCaptured();
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
            if (occupant.animator != null) occupant.animator.SetTrigger("Captured");
        }
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

        // Interaction (challenge) radius on the outpost root
        challengeCollider = GetComponent<CircleCollider2D>();
        if (challengeCollider == null) challengeCollider = gameObject.AddComponent<CircleCollider2D>();
        challengeCollider.isTrigger = true;
        challengeCollider.radius = config.inputChallengeRadius;

        float radius = config.colliderRadius;
        ShapeType shape = config.shape;

        switch (shape)
        {
            case ShapeType.Circle:
                {
                    var circle = shapeBounds.AddComponent<CircleCollider2D>();
                    circle.radius = radius;
                    circle.isTrigger = false;
                    boundsCollider = circle;
                    break;
                }
            case ShapeType.Square:
                {
                    var box = shapeBounds.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(radius * 2f, radius * 2f);
                    box.isTrigger = false;
                    boundsCollider = box;
                    break;
                }
            case ShapeType.Triangle:
                {
                    var poly = shapeBounds.AddComponent<PolygonCollider2D>();
                    float height = Mathf.Sqrt(3f) * radius;
                    Vector2[] points = new Vector2[3];
                    points[0] = new Vector2(-radius, -height / 3f);
                    points[1] = new Vector2(radius, -height / 3f);
                    points[2] = new Vector2(0f, 2f * height / 3f);
                    poly.points = points;
                    poly.isTrigger = false;
                    boundsCollider = poly;
                    break;
                }
        }
    }

    public void Initialize(OutpostConfig config)
    {
        faction = config.faction;
        SetupCollider(config);
        ApplyFactionVisuals(config.shape, config.color);
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
    }

    private void Update()
    {
        // Keep occupants within bounds (soft correction)
        if (boundsCollider == null) return;
        for (int i = 0; i < occupants.Count; i++)
        {
            var occ = occupants[i];
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