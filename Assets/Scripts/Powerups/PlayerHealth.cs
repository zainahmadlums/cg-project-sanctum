using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public HealthBarUI healthBarUI;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBarUI.RefreshUI(currentHealth);
    }

    private void Update()
    {
        // TEST KEYS
        if (Keyboard.current.yKey.wasPressedThisFrame)
            TakeDamage(1);

        if (Keyboard.current.hKey.wasPressedThisFrame)
            Heal(1);
    }

    public void Heal(int amount)
    {
        if (currentHealth >= maxHealth) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        // UI update
        healthBarUI.RefreshUI(currentHealth);
        healthBarUI.PlayHealFX(currentHealth - 1);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        // UI update
        healthBarUI.RefreshUI(currentHealth);
        healthBarUI.PlayDamageFX();

        if (currentHealth == 0)
            OnDeath();
    }
    
    private bool isDead = false;

    private void OnDeath()
    {
        if (isDead) return;     // prevent multiple reloads
        isDead = true;

        // optional delay so UI animations finish
        float reloadDelay = 0.4f; 

        Invoke(nameof(ReloadScene), reloadDelay);
    }

    private void ReloadScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
}
