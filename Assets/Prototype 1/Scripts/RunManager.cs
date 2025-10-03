using JetBrains.Annotations;
using UnityEngine;

namespace PrototypeOne
{

    public class RunManager : MonoBehaviour
    {
        public static RunManager runInstance;
        public static FactionType GetPlayerFaction() => FactionType.Grey;

        public int inputChallengeAttempts;
        public FactionType playerFaction;
        public GameObject player;


        void Awake()
        {
            if (runInstance != null && runInstance != this)
            {
                Destroy(gameObject);
                return;
            }

            runInstance = this;
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            player = GameObject.FindWithTag("Player");
            InitializePlayer();
        }


        public void InitializePlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            var health = player.GetComponent<PlayerHealth>();
            var faction = player.GetComponent<FactionManager>();

            float startingHealth = Mathf.Min(3 + inputChallengeAttempts, 8);
            health.SetHealth(startingHealth);

            var factionManager = Object.FindFirstObjectByType<FactionManager>(); 
            if (factionManager != null)
                faction.SetPlayerFaction(FactionType.Grey);

            GameManager.Instance.currentHealth = startingHealth;
            GameManager.Instance.UpdateHealthUI(startingHealth);


        }

        public void RegisterFailedChallenge()
        {
            inputChallengeAttempts++;
        }

        public void ResetRun()
        { 
            inputChallengeAttempts = 0;
            playerFaction = FactionType.Grey;        
        }

        void Update()
        {

        }



    }
}
        
