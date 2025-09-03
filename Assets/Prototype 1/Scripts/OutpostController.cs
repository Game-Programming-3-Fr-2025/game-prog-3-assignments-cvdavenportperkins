using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutpostController : MonoBehaviour
{
    [SerializeField] private GameObject dishargeEffectPrefab;
    [SerializeField] private float dischargeDamage = 1f;

    public FactionManager faction;
    private bool nodeCaptured = false;
    private GameObject shapeBounds;

    [SerializeField] private CircleCollider2D challengeCollider;

    List<OccupantController> occupants = new List<OccupantController>();

    public void SpawnOutpost(FactionType faction, Vector3 location, int levelIndex, OutpostConfig config)
    {
        int occupantCount = Random.Range(3, 10); // Random number of occupants between 3 and 10
        for (int i = 0; i < occupantCount; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = location + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(0.5f, config.colliderRadius * 0.9f);
            GameObject occupant = Instantiate(Resources.Load<GameObject>("OccupantPrefab"), spawnPosition, Quaternion.identity);
            OccupantController occupantController = occupant.GetComponent<OccupantController>();
            occupantController.outpostCenter = this.transform;
            occupantController.roamRadius = 3f;
            occupantController.roamSpeed = Random.Range(1f, 3f);
            occupantController.infectionRadius1 = 2f;
            occupantController.infectionRadius2 = 4f;
            occupantController.infectionChance1 = 0.33f;
            occupantController.infectionChance2 = 0.50f;
            occupantController.animator = occupant.GetComponent<Animator>();
            occupantController.faction = faction; // Assign faction
            occupantController.currentColor = FactionManager.GetColor(faction); // Set faction color
            occupant.GetComponent<Renderer>().material.color = FactionManager.GetColor(faction); // Apply faction color
            occupants.Add(occupantController);
        }
    }

    private IEnumerator InfectionPulse()
    {
        while (!nodeCaptured)
        {
            foreach (OccupantController occupant in occupants)
            {
                if (!occupant.isInfected)
                {
                    bool infected = false;

                    float distance = Vector3.Distance(transform.position, occupant.transform.position);

                    if (distance <= occupant.infectionRadius1 && Random.value < occupant.infectionChance1)
                        infected = true;

                    if (distance <= occupant.infectionRadius2 && Random.value < occupant.infectionChance2)
                        infected = true;

                    if (infected)
                    {
                        occupant.Infect();
                        occupant.FlashTick(Color.red); // Flash red on infection
                    }
                    else
                    {
                        occupant.FlashTick(Color.white); // Flash white if not infected
                    }

                }


            }

            yield return new WaitForSeconds(0.5f); // Pulse every .5 seconds
            if (occupants.TrueForAll(o => o.isInfected))
            {
                Debug.Log("All occupants infected!");
                UpdateNodeUI();
                nodeCaptured = true;
                Debug.Log("Outpost Captured!");
            }
        }
    }
    public void AccessGranted()
    {
        StartCoroutine(InfectionPulse());
    }

    public void UpdateNodeUI()
    {
        // Update the outpost's visual representation to indicate capture
        Light2D outpostLight = GetComponentInChildren<Light2D>();
        if (outpostLight != null)
        {
            outpostLight.color = Color.red; // Change light color to red
            outpostLight.intensity = 2f; // Increase intensity
        }

        // Change color of all occupants to indicate capture
        foreach (OccupantController occupant in occupants)
        {
            occupant.GetComponent<Renderer>().material.color = Color.red;
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

        challengeCollider = GetComponent<CircleCollider2D>();
        if (challengeCollider == null) challengeCollider = gameObject.AddComponent<CircleCollider2D>();
        challengeCollider.isTrigger = true;
        challengeCollider.radius = config.inputChallengeRadius;

        float radius = config.colliderRadius;
        ShapeType shape = config.shape;

        switch (shape)
        {
            case ShapeType.Circle:
                BoxCollider2D circleBox = GetComponent<BoxCollider2D>();
                if (circleBox == null) circleBox = shapeBounds.AddComponent<BoxCollider2D>();
                circleBox.size = new Vector2(radius * 2, radius * 2);
                circleBox.isTrigger = false;
                break;

            case ShapeType.Square:
                BoxCollider2D box = GetComponent<BoxCollider2D>();
                if (box == null) box = shapeBounds.AddComponent<BoxCollider2D>();
                box.size = new Vector2(radius * 2, radius * 2);
                box.isTrigger = false;
                break;

            case ShapeType.Triangle:
                PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
                if (poly == null) poly = shapeBounds.AddComponent<PolygonCollider2D>();
                
                float height = Mathf.Sqrt(3) * radius; // Height of the triangle
                Vector2[] points = new Vector2[3];
                points[0] = new Vector2(-radius, -height / 3); // Bottom left
                points[1] = new Vector2(radius, -height / 3);  // Bottom right
                points[2] = new Vector2(0, 2 * height / 3);    // Top
                poly.points = points;
                poly.isTrigger = true;
                break;
                
        }
    }

    public void Initialize(OutpostConfig config)
    {
        SetupCollider(config);
        ApplyFactionVisuals(config.shape, config.color);
        SpawnOutpost(config.faction, transform.position, config.levelIndex, config);
    }

    public void ApplyFactionVisuals(ShapeType shape, Color color)
    {
        ShapeVisualController visualController = GetComponentInChildren<ShapeVisualController>();
        if (visualController != null)
        {
            visualController.SetShape(shape);
            visualController.SetColor(color);
        }


    }

    public void TriggerDischarge(Vector3 targetPosition)
    {
        // Instantiate discharge effect
        if (dishargeEffectPrefab != null)
        {
            Instantiate(dishargeEffectPrefab, targetPosition, Quaternion.identity);
        }

        // Apply damage to Player prefab in radius
        Collider2D hit = Physics2D.OverlapCircle(targetPosition, 0.5f, LayerMask.GetMask("Player"));
        if (hit != null && hit.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(dischargeDamage);
        }
        
            
        
    }
}
