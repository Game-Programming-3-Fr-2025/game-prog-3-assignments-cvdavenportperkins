using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.AppUI.Core;

namespace PrototypeSix
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Generate Level")]
        [SerializeField] private GameObject playerPrefab;
        
      



        [Header("Game State")]
        public bool isGameOver = false;
        public bool isGameWon = false;
        public bool isPaused = false;

        [Header("World Bounds")]
        [SerializeField] private PolygonCollider2D baseLevelCollider;
        private Bounds worldBounds;
        public Bounds GetWorldBounds() => worldBounds;

        [Header("Score & Time")]
        public int score = 0;
        public float gameTime = 180f; // Total game time in seconds
        public float gameClock = 60f;
        public float timeRemaining;

        [Header("Objectives")]
        

        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI youWinText;

        public Image HealthImage1;
        public Image HealthImage2;
        public Image HealthImage3;

        public float currentHealth = 3f;


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            timeRemaining = gameTime;
           
        }

        void Start()
        {
           
            SpawnPlayer();
           

            UpdateScoreText();
            UpdateTimerText();
            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) youWinText.gameObject.SetActive(false);
        }

        void Update()
        {
            if (isGameOver || isGameWon || isPaused) return;

            timeRemaining -= Time.deltaTime;
            UpdateTimerText();

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isGameOver = true;
                HandleGameOver();
            }
        }
                
        // --- Health Handling ---

        public void UpdateHealthUI(float currenthealth)
        {
            HealthImage1.color = currenthealth >= 1 ? Color.white : Color.red;
            HealthImage1.color = currenthealth >= 2 ? Color.white : Color.red;
            HealthImage1.color = currenthealth >= 3 ? Color.white : Color.red;
        }

        // --- world Generaion ---

        public void SetWorldBounds(Bounds bounds)
        {
            worldBounds = bounds;
        }

        public void SpawnPlayer()
        {
            Vector3 safePos = GameManager.Instance.ClampToWorldBounds(Vector3.zero, 2f);
            Instantiate(playerPrefab, safePos, Quaternion.identity);
        }

        public Vector3 ClampToWorldBounds(Vector3 position, float padding = 0f)
        {
            return new Vector3(
                Mathf.Clamp(position.x, worldBounds.min.x + padding, worldBounds.max.x - padding),
                Mathf.Clamp(position.y, worldBounds.min.y + padding, worldBounds.max.y - padding),
                position.z
            );
        }

        // --- Score Handling ---
        public void AddScore(int points)
        {
            score += points;
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            if (scoreText) scoreText.text = $"Score: {score}";
        }

        private void UpdateTimerText()
        {
            if (timerText) timerText.text = $"Time: {Mathf.Max(0, timeRemaining):F2}";
        }

        public void ExtendGameClock(float extraTime)
        {
            gameClock += extraTime;
            Debug.Log($"Game clock extended by {extraTime} seconds. New time: {gameClock}");
        }

        // --- Game End States ---
        public void HandleGameOver()
        {
            SoundManager.instance?.PlayGameOverSound();
            Debug.Log("Game Over! Time's up.");
            if (gameOverText) gameOverText.gameObject.SetActive(true);
            SceneManager.LoadScene("GameOverScene");
           
        }

        public void HandleGameWon()
        {
            SoundManager.instance?.PlayVictorySound();
            Debug.Log("Congratulations! You've won the game!");
            if (youWinText) youWinText.gameObject.SetActive(true);
            SceneManager.LoadScene("VictoryScene");
         
        }

        // --- Pause/Resume ---
        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }

        // --- Optional Reset for Replay ---
        public void ResetGame()
        {
            score = 0;
            timeRemaining = gameTime;
            isGameOver = false;
            isGameWon = false;
           

            UpdateScoreText();
            UpdateTimerText();

            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) youWinText.gameObject.SetActive(false);

            Time.timeScale = 1f;
        }
    }
}