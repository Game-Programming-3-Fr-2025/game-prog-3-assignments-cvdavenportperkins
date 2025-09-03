using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isGameOver = false;
    public bool isGameWon = false;
    public int score = 0;
    public bool isPaused = false;
    public float gameTime = 300f; // Total game time in seconds
    public float timeRemaining;
    public int outpostsRemaining;
    public int totalOutposts;

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

    public void GrantAccess()
    {
        SoundManager.instance?.PlayAccessGrantedSound(Camera.main.transform.position);
    }

    public void DenyAccess()
    {
        SoundManager.instance?.PlayAccessDeniedSound(Camera.main.transform.position);
    }

    void Update()
    {
        if (isGameOver || isGameWon || isPaused) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            isGameOver = true;
            HandleGameOver();
        }
    }

    public void HandleGameOver()
    {
        SoundManager.instance?.PlayGameOverSound();
        Debug.Log("Game Over! Time's up.");
        SceneManager.LoadScene("GameOverScene");
        FactionManager.ResetInfection();
    }

    public void HandleGameWon()
    {
        SoundManager.instance?.PlayVictorySound();
        Debug.Log("Congratulations! You've won the game!");
        SceneManager.LoadScene("VictoryScene");
        FactionManager.ResetInfection();
    }
}