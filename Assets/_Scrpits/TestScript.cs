using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    private void Start()
    {
        inputReader.MoveEvent += HandlerMove;
    }

    private void OnDestroy()
    {
        inputReader.MoveEvent -= HandlerMove;
    }

    private void  HandlerMove(Vector2 movement)
    {
        Debug.Log($"Move {movement}");
    }
}
