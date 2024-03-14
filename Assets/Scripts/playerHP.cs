using UnityEngine;
using UnityEngine.UI;

public class playerHP : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Text healthText;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthUI();
    }

    void Die()
    {
        Debug.Log("Player has died");
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth.ToString();
        }
    }
}