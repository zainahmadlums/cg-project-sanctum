using UnityEngine;

public class HealPickup : PickupBase
{
    public int healAmount = 1;

    protected override void OnPickup(GameObject player)
    {
        PlayerHealth hp = player.GetComponent<PlayerHealth>();

        if (hp != null)
        {
            hp.Heal(healAmount);
        }
    }
}
