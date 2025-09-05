using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 3f;
    public GameObject damageEffectPrefab;
    public AudioClip damageSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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