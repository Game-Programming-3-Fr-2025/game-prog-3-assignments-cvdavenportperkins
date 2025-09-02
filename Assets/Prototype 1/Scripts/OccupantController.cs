using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupantController : MonoBehaviour
{
    public Transform outpostCenter;
    public float roamRadius = 5f;
    public float roamSpeed = 2f;
    public float infectionRadius1 = 2f;
    public float infectionRadius2 = 4f;
    public float infectionChance1 = 0.33f; // 33% chance to infect
    public float infectionChance2 = 0.50f; // 50% chance to infect
    public float infectionRate;
    public Vector3 roamTarget;

    public bool isInfected = false;
    public Color infectionColor = Color.green;
    public Color currentColor;

    public OutpostController.FactionType faction;
    public Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        SetNewRoamTarget();
    }

    // Update is called once per frame
    void Update()
    {            
        transform.position = Vector3.MoveTowards(transform.position, roamTarget, roamSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, roamTarget) < 0.5f)
        {
            SetNewRoamTarget();
        }
    }

    void SetNewRoamTarget()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 targetPosition = outpostCenter.position + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(0, roamRadius);
        roamTarget = targetPosition;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Infected") && !isInfected) Infect();    
    }

    public void Infect()
    {
        isInfected = true;
        gameObject.tag = "Infected";
        // Change color to indicate infection
        GetComponent<Renderer>().material.color = infectionColor;
        if (animator != null) animator.SetTrigger("Infect");
        Debug.Log($"{gameObject.name} has been assimilated!");
    }
    public void FlashTick(Color tickColor)
    {
        StartCoroutine(TickCoroutine(tickColor));
    }

    IEnumerator TickCoroutine(Color tickColor)
    {
        Renderer rend = GetComponent<Renderer>();
                Color originalColor = rend.material.color;
        rend.material.color = tickColor;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }
}
