using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class CoinBehaviour : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    protected int _coinValue = 10;
    protected bool _alreadyCollected;

    public abstract int Collect();

    public void SetValue(int coinValue)
    {
        _coinValue = coinValue;
    }

    protected void Show(bool show)
    {
        _spriteRenderer.enabled = show;
    }

}
