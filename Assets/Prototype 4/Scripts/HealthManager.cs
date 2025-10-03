using UnityEngine;
namespace PrototypeFour
{
    public class HealthManager : MonoBehaviour
    {
        public int baseHealth = 100;
        public int currentHealth;

        public int tierLevel = 1;
        public int deathCount = 0;
        public int streakBonus = 0;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            currentHealth = CalculateScaledHealth();
        }

        public int CalculateScaledHealth()
        {
            int tierBoost = tierLevel * 10;
            int deathRecovery = deathCount * 5;
            int streakBoost = streakBonus * 15;

            return baseHealth + tierBoost + deathRecovery + streakBoost;  
        }

        public void RegisterDeath()
        {
            deathCount++;
            currentHealth = CalculateScaledHealth();
        }

        public void RegisterStreak(int streak)
        {
            streakBonus = streak;
            currentHealth = CalculateScaledHealth();
            
            if (currentHealth <= 0)
            {
                GameManager.Instance?.HandleGameOver();
            }

        }

        public void SetTier(int tier)
        {
            tierLevel = tier;
            currentHealth = CalculateScaledHealth();
        }
        
        // Update is called once per frame
        void Update()
        {

        }
    }


}
