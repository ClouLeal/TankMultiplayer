using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController _healthController;
    [SerializeField] private Image _healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        _healthController.CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, _healthController.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsClient) return;

        _healthController.CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    public void HandleHealthChange(int oldValue, int newValue)
    {
        _healthBarImage.fillAmount = (float) newValue / _healthController.MaxHealth;
    }
}
