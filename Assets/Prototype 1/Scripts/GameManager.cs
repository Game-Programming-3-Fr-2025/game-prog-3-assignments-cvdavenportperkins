using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PrototypeOne
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game State")]
        public bool isGameOver = false;
        public bool isGameWon = false;
        public bool isPaused = false;

        [Header("World Bounds")]
        [SerializeField] private PolygonCollider2D baseLevelCollider;
        private Bounds worldBounds;
        
        [Header("Score & Time")]
        public int score = 0;
        public float gameTime = 300f; // Total game time in seconds
        public float timeRemaining;

        [Header("Objectives")]
        public int outpostsRemaining;
        public int totalOutposts;

        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI youWinText;

        public Image HealthImage1;
        public Image HealthImage2;
        public Image HealthImage3;

        public int currentHealth = 3;


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
            if (baseLevelCollider != null)
            {
                worldBounds = baseLevelCollider.bounds;
            }
        }

        public Vector3 ClampToWorldBounds(Vector3 position, float padding = 0f)
        {
            return new Vector3(
                Mathf.Clamp(position.x, worldBounds.min.x, worldBounds.max.x - padding),
                Mathf.Clamp(position.y, worldBounds.min.y, worldBounds.max.y - padding),
                position.z
            );
        }
        void Start()
        {
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

        public void UpdateHealthUI(int currenthealth)
        {
            HealthImage1.color = currenthealth >= 1 ? Color.white : Color.red;
            HealthImage1.color = currenthealth >= 2 ? Color.white : Color.red;
            HealthImage1.color = currenthealth >= 3 ? Color.white : Color.red;
        }

        // --- Objective Tracking ---
        public void RegisterOutpost()
        {
            totalOutposts++;
            outpostsRemaining++;
        }

        public void OnOutpostCaptured()
        {
            outpostsRemaining = Mathf.Max(0, outpostsRemaining - 1);
            if (outpostsRemaining <= 0 && !isGameWon)
            {
                isGameWon = true;
                HandleGameWon();
            }
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

        // --- Access Feedback ---
        public void GrantAccess()
        {
            SoundManager.instance?.PlayAccessGrantedSound(Camera.main.transform.position);
        }

        public void DenyAccess()
        {
            SoundManager.instance?.PlayAccessDeniedSound(Camera.main.transform.position);
        }

        // --- Game End States ---
        public void HandleGameOver()
        {
            SoundManager.instance?.PlayGameOverSound();
            Debug.Log("Game Over! Time's up.");
            if (gameOverText) gameOverText.gameObject.SetActive(true);
            SceneManager.LoadScene("GameOverScene");
            FactionManager.ResetInfection();
        }

        public void HandleGameWon()
        {
            SoundManager.instance?.PlayVictorySound();
            Debug.Log("Congratulations! You've won the game!");
            if (youWinText) youWinText.gameObject.SetActive(true);
            SceneManager.LoadScene("VictoryScene");
            FactionManager.ResetInfection();
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
            outpostsRemaining = totalOutposts;

            UpdateScoreText();
            UpdateTimerText();

            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) youWinText.gameObject.SetActive(false);

            Time.timeScale = 1f;
        }
    }
}