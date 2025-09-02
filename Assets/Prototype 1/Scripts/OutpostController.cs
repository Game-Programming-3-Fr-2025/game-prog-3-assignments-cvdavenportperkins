using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutpostController : MonoBehaviour
{
    public enum FactionType { Yellow, Cyan, Magenta }
    private bool nodeCaptured = false;

    List<OccupantController> occupants = new List<OccupantController>();

    public static Dictionary<FactionType, Color> factionColors = new Dictionary<FactionType, Color>
    {
        { FactionType.Yellow, Color.yellow },
        { FactionType.Cyan, Color.cyan },
        { FactionType.Magenta, Color.magenta }
    };

    public void SpawnOutpost(FactionType faction, Vector3 location, int levelIndex)
    {
        int occupantCount = Random.Range(3, 10); // Random number of occupants between 3 and 10
        for (int i = 0; i < occupantCount; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnPosition = location + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(1f, 3f);
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
            occupantController.currentColor = OutpostController.factionColors[faction]; // Set faction color
            occupant.GetComponent<Renderer>().material.color = OutpostController.factionColors[faction]; // Apply faction color
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
    void AccessGranted()
    {
        StartCoroutine(InfectionPulse()); 
    }

    void UpdateNodeUI()
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



}
