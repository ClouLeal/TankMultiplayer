using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damage = 5;

    private ulong _ownerClientId; 

    public void SetOwner(ulong ownerClientId)
    {
        _ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody == null) { return; }
    
        if(collision.attachedRigidbody.TryGetComponent<NetworkObject>(out var netObj))
        {
            if(_ownerClientId == netObj.OwnerClientId){ return; }
        }

        if(collision.attachedRigidbody.TryGetComponent<HealthController>(out var healthController))
        {
            healthController.TakeDamage(_damage);
        }
    }
}
