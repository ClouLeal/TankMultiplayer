using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpawnTransform;
    [SerializeField] private GameObject _projectileClientPrefab;
    [SerializeField] private GameObject _projectileServerPrefab;

    [SerializeField] private GameObject _muzzleFlashSprite;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _projectileFireRate;
    [SerializeField] private float _muzzleFlashDuration;

    private bool _shouldFire;
    private float _previousFiteTime;
    private float _muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader.PrimaryFireEvent += HandlerPrimaryFire; 
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) return;

        _inputReader.PrimaryFireEvent -= HandlerPrimaryFire;
    }

    private void HandlerPrimaryFire(bool shouldFire)
    {
        _shouldFire = shouldFire;
    }

    void Update()
    {
        if(_muzzleFlashTimer > 0f)
        {
            _muzzleFlashTimer -= Time.deltaTime;

            if(_muzzleFlashTimer <= 0f ) 
            {
                _muzzleFlashSprite.SetActive(false);
                _muzzleFlashTimer = 0f;
            }
        }

        if(!IsOwner) return;
        if(!_shouldFire) return;

        if (Time.time < (1 / _projectileFireRate) + _previousFiteTime) return;

        SpawnDummyProjectile(_projectileSpawnTransform.position, _projectileSpawnTransform.up);
        PrimaryFireServerRpc(_projectileSpawnTransform.position, _projectileSpawnTransform.up);

        _previousFiteTime = Time.time;
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        var projectileInstance = Instantiate(_projectileServerPrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if (projectileInstance.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
        {
            rigidbody2D.velocity = projectileInstance.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        if(IsOwner) return;
        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        _muzzleFlashSprite.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;

        var projectileInstance = Instantiate(_projectileClientPrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out var rigidbody2D))
        {
            rigidbody2D.velocity = projectileInstance.transform.up * _projectileSpeed;
        }
    }



}
