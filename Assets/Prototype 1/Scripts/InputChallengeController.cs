using NUnit.Framework;
using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class InputChallengeController : MonoBehaviour
{
    [SerializeField] private ShapeVisualController visualController;
    [SerializeField] private float challengeDuration = 3.5f;

    private PlayerController currentPlayer;
    private OutpostController currentOutpost; 
        
    private ShapeType expectedShape;
    private ShapeType targetShape;
    private float timer;
    private bool challengeActive;

    public event Action OnChallengeSuccess;
    public event Action OnChallengeFailure;

    private int totalInputs;
    private float totalDuration;
    private List<string> promptSequence = new List<string>();
    private List<GameObject> promptUIElements = new List<GameObject>();
    private int currentInputIndex = 0;

    public GameObject promptPrefab;
    public GameObject promptContainer;

    public void DisplayPromptSequence()
    {
        foreach (string prompt in promptSequence)
        {
            GameObject promptUI = Instantiate(promptPrefab, promptContainer.transform);
            promptUI.GetComponent<TextMeshPro>().text = prompt;
            promptUIElements.Add(promptUI);
        }
    }
    
    public void SubmitPromptInput(string input)
    {
        if (!challengeActive || currentInputIndex >= promptSequence.Count) return;

        if (input == promptSequence[currentInputIndex])
        {
            Destroy(promptUIElements[currentInputIndex]);
            currentInputIndex++;

            if (currentInputIndex >= promptSequence.Count)
            { 
                ChallengeSucceeded();
            }
            else
            {
                ChallengeFailed();
            }
        }

        
    }

    public void ConfigureChallenge(int occupantCount, float factionInfectionRatio)
    {
        int extraInputs = occupantCount / 3;
        totalInputs = 3 + extraInputs;
        totalDuration = 3f + (0.5f * extraInputs);

        bool includeNumbers = factionInfectionRatio > 0.33f;

        string[] directions = { "Up", "Down", "Left", "Right" };
        string[] numbers = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        promptSequence.Clear();
        for (int i = 0; i < totalInputs; i++)
        {
            bool useNumber = includeNumbers && UnityEngine.Random.value > 0.5f;
            string prompt = useNumber
                ? numbers[UnityEngine.Random.Range(0, numbers.Length)]
                : directions[UnityEngine.Random.Range(0, directions.Length)];
            promptSequence.Add(prompt);
        }
    }

    public void StartChallenge(FactionType faction, PlayerController player, OutpostController outpost)
    {
        currentPlayer = player;
        currentOutpost = outpost;
        
        var targetShape = FactionManager.GetShape(faction);
        expectedShape = targetShape; 
        visualController.SetShape(targetShape);
        visualController.SetColor(Color.white);
        timer = challengeDuration;
        challengeActive = true;
    }

    public List<string> GetPromptSequence()
    {
        return new List<string> (promptSequence);
    }

    private void Update()
    {
        if (!challengeActive) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChallengeFailed();
            return;
        }
    }

    public void SubmitInput(ShapeType inputShape)
    {
        if (!challengeActive) return;
        if (inputShape == expectedShape)
        {
            ChallengeSucceeded();
        }
        else
        {
            ChallengeFailed();
        }
    }

    private void ChallengeSucceeded()
    {
        challengeActive = false;
        visualController.SetColor(Color.green);
        OnChallengeSuccess?.Invoke();
        
        if (currentPlayer != null && currentOutpost != null)
        {
            HandleChallengeSuccess(currentPlayer, currentOutpost);
        }
        
    }

    private void ChallengeFailed()
    {
        challengeActive = false;
        visualController.SetColor(Color.red);
        OnChallengeFailure?.Invoke();
    }

    public void HandleChallengeSuccess(PlayerController player, OutpostController outpost)
    {
        player.faction = outpost.faction;

        player.UpdateVisuals();

        outpost.AccessGranted(player);
    }

}
