using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Settings")]
    public int damageAmount = 1;
    public float knockbackForce = 3f;
    public float stunDuration = 0.5f;
    public float damageCooldown = 1.0f; // Debounce time

    private float lastDamageTime;

    void OnTriggerEnter(Collider other)
    {
        // 1. Debounce Check: Are we on cooldown?
        if (Time.time < lastDamageTime + damageCooldown) return;

        // 2. Check for Player Components
        // We look for the controller first since that's what we move
        if (other.TryGetComponent<PlayerController>(out PlayerController playerMove))
        {
            // Calculate knockback direction
            // Direction: From trap center TO player, plus a little Up so they hop
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.y = 0.5f; // Ensure upward lift
            direction.Normalize();

            // Apply the physics shove
            playerMove.ApplyKnockback(direction * knockbackForce, stunDuration);
            
            // 3. Deal Damage
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth health))
            {
                health.TakeDamage(damageAmount);
            }

            // Reset cooldown
            lastDamageTime = Time.time;
        }
    }
}