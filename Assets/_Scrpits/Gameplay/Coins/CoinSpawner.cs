using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CoinRespwanBehaviour _coinPrefab;

    [Header("Settings")]
    [SerializeField] private int _maxCoinCount = 50;
    [SerializeField] private int _coinValue = 10;
    [SerializeField] private Vector2 _xSpawnRange;
    [SerializeField] private Vector2 _ySpawnRange;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] _coinBuffer = new Collider2D[1]; 
    private float _coinRadius;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoinCount; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        var coinInstance = Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(_coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandlerCoinCollected;
    }

    private void HandlerCoinCollected(CoinRespwanBehaviour coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x=0, y=0;

        while(true)
        {
            x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
            y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);

            Vector2 spawnPoint = new Vector2(x, y);

            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _coinRadius, _coinBuffer, layerMask);

            if (numColliders == 0) return spawnPoint;
        }
    }

}
