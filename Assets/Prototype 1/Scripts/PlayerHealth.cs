using UnityEngine;

namespace PrototypeOne
{

    public class PlayerHealth : MonoBehaviour
    {
        public float maxHealth = 8f;
        public float currentHealth = 3f;
        public bool isAlive => currentHealth > 0f;
        public GameObject damageEffectPrefab;
        public AudioClip damageSound;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }


        public void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0f, maxHealth);
            GameManager.Instance.currentHealth = currentHealth;
            GameManager.Instance.UpdateHealthUI(Mathf.FloorToInt(currentHealth));
        }
        public void TakeDamage(float damageAmount)
        {
            if (damageEffectPrefab != null)
            {
                Instantiate(damageEffectPrefab, transform.position, Quaternion.identity);
            }

            if (audioSource != null && damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.isGameOver = true;
            GameManager.Instance.HandleGameOver();
            Debug.Log("Player has died.");
        }
    }
}