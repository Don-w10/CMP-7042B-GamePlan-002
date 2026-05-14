using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider healthSlider;
    public PlayerHealth playerHealth;

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void Start()
    {
        if (playerHealth != null)
            UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthSlider != null)
            healthSlider.value = current / max;
    }
}
