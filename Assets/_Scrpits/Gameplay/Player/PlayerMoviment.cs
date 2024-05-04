using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoviment : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rigibody;
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float tourningRate = 360f;

    private Vector2 previousMovimentInput;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent += MovimentHandler;
    }

    private void MovimentHandler(Vector2 movimentInput)
    {
        previousMovimentInput = movimentInput;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        inputReader.MoveEvent -= MovimentHandler;
    }

    //Update is called once per frame
    private void Update()
    {
        if(IsOwner)
        {
            float zRotation = previousMovimentInput.x * -tourningRate * Time.deltaTime;
            bodyTransform.Rotate(0f,0f,zRotation);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        rigibody.velocity = (Vector2)bodyTransform.up * previousMovimentInput.y * movementSpeed;
    }
}
