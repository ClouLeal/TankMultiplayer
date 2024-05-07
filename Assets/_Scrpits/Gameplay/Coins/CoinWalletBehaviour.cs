using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWalletBehaviour : NetworkBehaviour 
{
    public NetworkVariable<int> TotalConins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<CoinBehaviour>(out var coin))
        {
            var coinCollectedValue = coin.Collect();
            
            if (!IsServer) return;

            TotalConins.Value += coinCollectedValue;
        }
    }

    public bool TrySpendCoins(int coins)
    {
        if(TotalConins.Value < coins) return false;

        TotalConins.Value -= coins;
        return true;
    }
}
