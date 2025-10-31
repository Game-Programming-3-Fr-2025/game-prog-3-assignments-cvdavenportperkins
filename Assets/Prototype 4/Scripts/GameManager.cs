using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PrototypeFour
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Prefabs")]
        public GameObject playerPrefab;
       
        public GameObject levelPrefab;

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
        public float gameTime = 180f;
        public float gameClock = 60f;
        public float timeRemaining;

        [Header("UI References")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI youWinText;
        public Slider healthSlider;

        [Header("Level Parameters")]
        public LevelWorld levelWorld;

        private GameObject playerInstance;
       
        private HealthManager healthManager;
        public int currentTier = 1;

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

        void Start()
        {
            InitializePlayerAndCamera();

            if (playerInstance != null)
            {
                healthManager = playerInstance.GetComponent<HealthManager>();
                healthManager.SetTier(currentTier);
            }

            UpdateScoreText();
            UpdateTimerText();
            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) gameOverText.gameObject.SetActive(false);
        }

        void InitializePlayerAndCamera()
        {
            Vector2 spawnPos = (levelWorld.spawnAreaMin + levelWorld.spawnAreaMax) / 2f;

            playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            EnemyAI[] enemies = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                enemy.player = playerInstance.transform;
            }
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

            if (healthManager != null && healthSlider != null)
            {
                healthSlider.maxValue = healthManager.CalculateScaledHealth();
                healthSlider.value = healthManager.currentHealth;
            }

            if (healthManager != null && healthManager.currentHealth <= 0 && !isGameOver)
            {
                isGameOver = true;
                HandleGameOver();
            }
        }

        public void GenerateWorld(int tierLevel)
        {
            float scaleFactor = 1f + (tierLevel * 0.15f);
            GameObject level = Instantiate(levelPrefab);
            level.transform.localScale = Vector3.one * scaleFactor;

            PolygonCollider2D collider = level.GetComponent<PolygonCollider2D>();
            SetWorldBounds(collider.bounds);
        }

        public void SetWorldBounds(Bounds bounds)
        {
            worldBounds = bounds;
        }

        public Vector3 ClampToWorldBounds(Vector3 position, float padding = 0f)
        {
            return new Vector3(
                Mathf.Clamp(position.x, worldBounds.min.x + padding, worldBounds.max.x - padding),
                Mathf.Clamp(position.y, worldBounds.min.y + padding, worldBounds.max.y - padding),
                position.z
            );
        }

        public void UpdateHealthSlider(int value)
        {
            if (healthSlider != null)
                healthSlider.value = value;
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

        public void ExtendGameClock(float extraTime)
        {
            gameClock += extraTime;
            Debug.Log($"Game clock extended by {extraTime} seconds. New time: {gameClock}");
        }

        public void HandleGameOver()
        {
            Debug.Log("Game Over! Time's up.");
            if (gameOverText) gameOverText.gameObject.SetActive(true);
            SceneManager.LoadScene("GameOverScene");
        }

        public void HandleGameWon()
        {
            Debug.Log("Congratulations! You've won the game!");
            if (youWinText) youWinText.gameObject.SetActive(true);
            SceneManager.LoadScene("VictoryScene");
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void ResetGame()
        {
            score = 0;
            timeRemaining = gameTime;
            isGameOver = false;
            isGameWon = false;

            UpdateScoreText();
            UpdateTimerText();

            if (gameOverText) gameOverText.gameObject.SetActive(false);
            if (youWinText) gameOverText.gameObject.SetActive(false);

            Time.timeScale = 1f;
        }

    }

} // namespace PrototypeFour