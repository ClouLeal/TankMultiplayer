using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;

    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;
    private Action<HealthController> OnDie;


    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        CurrentHealth.Value = MaxHealth;
    }


    public void TakeDamage(int damageValue)
    {
        ModifyHealthValue(-damageValue);
    }

    public void RestoreHealth(int healthValue)
    {
        ModifyHealthValue(healthValue);
    }


    private void ModifyHealthValue(int value)
    {
        if(isDead) return;

        var newHealth = CurrentHealth.Value + value;
        CurrentHealth.Value = Math.Clamp(newHealth, 0, MaxHealth);

        if(CurrentHealth.Value == 0)
        {
            isDead = true;
            OnDie?.Invoke(this);
        }
    }

}
