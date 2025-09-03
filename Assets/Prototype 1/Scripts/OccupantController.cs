using System.Collections;
using UnityEngine;

public class OccupantController : MonoBehaviour
{
    [Header("Roaming Settings")]
    public Transform outpostCenter;
    public float roamRadius = 5f;
    public float roamSpeed = 2f;

    [Header("Infection Settings")]
    [SerializeField] public float infectionRadius1 = 2f;
    [SerializeField] public float infectionRadius2 = 4f;
    [SerializeField] public float infectionChance1 = 0.50f; // inner radius chance
    [SerializeField] public float infectionChance2 = 0.33f; // outer radius chance
    [SerializeField] public float infectionRate = 0.5f; // seconds between spread attempts
    private float infectionTimer = 0f;
    public bool isInfected;

    [Header("Factions and Visuals")]
    public Color infectionColor = Color.green;
    public Color currentColor;
    public FactionType faction;
    public Animator animator;

    public Vector3 roamTarget;
    private SpriteRenderer rend;

    void Start()
    {
        SetNewRoamTarget();
        rend = GetComponent<SpriteRenderer>();
        if (rend != null && currentColor != default) rend.color = currentColor;
        FactionManager.RegisterOccupant();
    }

    void OnDestroy()
    {
        FactionManager.UnregisterOccupant(isInfected);
    }

    void Update()
    {
        if (isInfected)
        {
            infectionTimer += Time.deltaTime;
            if (infectionTimer >= infectionRate)
            {
                infectionTimer = 0f;
                TryInfectNearby();
            }

            var target = FindNearestUninfected(transform.position);
            if (target != null)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                transform.position += direction * roamSpeed * Time.deltaTime;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, roamTarget, roamSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, roamTarget) < 0.5f)
            {
                SetNewRoamTarget();
            }
        }
    }

    void SetNewRoamTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = outpostCenter.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * Random.Range(0, roamRadius);
        roamTarget = targetPosition;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Infected") && !isInfected)
        {
            Infect();
        }
    }

    public void Infect()
    {
        if (isInfected) return;

        isInfected = true;
        gameObject.tag = "Infected";
        if (rend != null) rend.color = infectionColor;
        if (animator != null) animator.SetTrigger("Infect");
        FactionManager.ReportInfection();
        SoundManager.instance?.PlayInfectionSound(transform.position);
        Debug.Log($"{gameObject.name} has been assimilated!");
    }

    public void FlashTick(Color tickColor)
    {
        StartCoroutine(TickCoroutine(tickColor));
    }

    IEnumerator TickCoroutine(Color tickColor)
    {
        if (rend == null) yield break;
        Color originalColor = rend.color;
        rend.color = tickColor;
        yield return new WaitForSeconds(0.2f);
        rend.color = originalColor;
    }

    private void TryInfectNearby()
    {
        OccupantController[] allOccupants = FindObjectsByType<OccupantController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var occupant in allOccupants)
        {
            if (!occupant.isInfected)
            {
                float dist = Vector3.Distance(transform.position, occupant.transform.position);

                if (dist < infectionRadius1 && Random.value < infectionChance1 + infectionChance2)
                {
                    occupant.Infect();
                }
                else if (dist < infectionRadius2 && Random.value < infectionChance2)
                {
                    occupant.Infect();
                }
            }
        }
    }

    public void EnforceBounds(Collider2D bounds)
    {
        if (!bounds.OverlapPoint(transform.position))
        {
            Vector3 direction = (outpostCenter.position - transform.position).normalized;
            transform.position += direction * Time.deltaTime * roamSpeed;
        }
    }

    public static OccupantController FindNearestUninfected(Vector3 origin)
    {
        OccupantController[] allOccupants = FindObjectsByType<OccupantController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        OccupantController nearest = null;
        float minDistance = float.MaxValue;

        foreach (var occupant in allOccupants)
        {
            if (occupant.isInfected) continue;

            float distance = Vector3.Distance(origin, occupant.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = occupant;
            }
        }
        return nearest;
    }
}