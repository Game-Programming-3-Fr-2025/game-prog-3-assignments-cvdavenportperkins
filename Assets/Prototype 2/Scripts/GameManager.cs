using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeTwo
{

    public class PrototyeTwoGameManager : MonoBehaviour
    {
        public PrototyeTwoGameManager Instance;
        public SoundManager SoundManager;

        [Header("Game State")]
        public bool isGameOver = false;
        public bool isGameWon = false;
        public bool isPaused = false;

        [Header("Score & Time")]
        public bool useTimer = false;    
        public int score = 0;
        public float gameTime = 300f; // Total game time in seconds
        public float timeRemaining;
        public Transform player;
        private float startY;

        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI youWinText;

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
            UpdateScoreText();
            UpdateTimerText();
            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) youWinText.gameObject.SetActive(false);
            
            if (player != null)
                startY = player.position.y;
        }

        void Update()
        {
            if (isGameOver || isGameWon || isPaused) return;

            if (useTimer)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerText();

                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    isGameOver = true;
                    HandleGameOver();
                }
            }
            else
            {
                if (player != null)
                {
                    int distance = Mathf.FloorToInt(startY - player.position.y);
                    if (distance != score)
                    {
                        score = distance;
                        UpdateScoreText();
                    }
                }
            }
        
        }
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isGameOver = true;
                HandleGameOver();
            }
        }

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