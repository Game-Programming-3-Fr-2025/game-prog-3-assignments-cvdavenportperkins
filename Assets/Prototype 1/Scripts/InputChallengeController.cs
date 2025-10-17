using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeOne
{

    public class InputChallengeController : MonoBehaviour
    {
        [SerializeField] private ShapeVisualController visualController; [SerializeField] public GameObject promptPrefab; [SerializeField] public Transform promptContainer;

        [SerializeField] private Sprite[] directionalSprites;
        [SerializeField] private Sprite[] numberSprites;

        [SerializeField] private float challengeDuration = 3.5f;

        private PlayerController currentPlayer;
        private OutpostController currentOutpost;
        private ShapeType expectedShape;

        private float timer;
        private bool challengeActive;

        public event Action OnChallengeSuccess;
        public event Action OnChallengeFailure;

        private int totalInputs;
        private float totalDuration;
        private readonly List<string> promptSequence = new();
        private readonly List<GameObject> promptUIElements = new();
        private int currentInputIndex = 0;

        public void SubmitPromptInput(string input)
        {
            if (!challengeActive || currentInputIndex >= promptSequence.Count) return;

            if (input == promptSequence[currentInputIndex])
            {
                if (currentInputIndex < promptUIElements.Count && promptUIElements[currentInputIndex] != null)
                    Destroy(promptUIElements[currentInputIndex]);

                currentInputIndex++;

                if (currentInputIndex >= promptSequence.Count)
                {
                    ChallengeSucceeded();
                }
            }
            else
            {
                ChallengeFailed();
            }
        }

        public void ConfigureChallenge(int occupantCount, float factionInfectionRatio)
        {
            int extraInputs = Mathf.FloorToInt(occupantCount / 3f);
            totalInputs = Mathf.Clamp(3 + extraInputs, 3, 12);
            totalDuration = Mathf.Clamp(3f + (0.5f * extraInputs), 3f, 10f);

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

        public void DisplayPromptSequence()
        {
            if (promptPrefab == null || promptContainer == null) return;

            foreach (var ui in promptUIElements)
            {
                if (ui != null) Destroy(ui);
            }
            promptUIElements.Clear();

            foreach (string prompt in promptSequence)
            {
                var promptUI = Instantiate(promptPrefab, promptContainer);
                var icon = promptUI.GetComponent<Image>();

                if (icon != null)
                {
                    if (IsDirection(prompt))
                        icon.sprite = GetDirectionalSprite(prompt);
                    else
                        icon.sprite = GetNumberSprite(prompt);
                }

                promptUIElements.Add(promptUI);
            }
        }

        public void StartChallenge(FactionType faction, PlayerController player, OutpostController outpost)
        {
            currentPlayer = player;
            currentOutpost = outpost;

            expectedShape = FactionManager.GetShape(faction);
            if (visualController != null)
            {
                visualController.SetShape(expectedShape);
                visualController.SetColor(Color.white);
            }

            timer = totalDuration > 0f ? totalDuration : challengeDuration;
            currentInputIndex = 0;
            challengeActive = true;
        }

        public List<string> GetPromptSequence()
        {
            return new List<string>(promptSequence);
        }

        private void Update()
        {
            if (!challengeActive) return;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ChallengeFailed();
            }
        }

        private void ChallengeSucceeded()
        {
            challengeActive = false;
            if (visualController != null) visualController.SetColor(Color.green);
            OnChallengeSuccess?.Invoke();

            if (GameManager.Instance != null) GameManager.Instance.GrantAccess();

            if (currentOutpost != null)
            {
                currentOutpost.AccessGranted();
                GameManager.Instance.AddScore(200);
            }

            if (currentPlayer != null)
            {
                currentPlayer.UpdateVisuals();
            }
        }

        private void ChallengeFailed()
        {
            challengeActive = false;
            if (visualController != null) visualController.SetColor(Color.red);
            OnChallengeFailure?.Invoke();

            if (GameManager.Instance != null) GameManager.Instance.DenyAccess();
        }

        private bool IsDirection(string prompt)
        {
            return prompt == "Up" || prompt == "Down" || prompt == "Left" || prompt == "Right";
        }

        private Sprite GetDirectionalSprite(string direction)
        {
            return direction switch
            {
                "Up" => directionalSprites[0],
                "Down" => directionalSprites[1],
                "Left" => directionalSprites[2],
                "Right" => directionalSprites[3],
                _ => null
            };
        }

        private Sprite GetNumberSprite(string number)
        {
            int index = int.Parse(number) - 1;
            return numberSprites[index];
        }

    }

} 