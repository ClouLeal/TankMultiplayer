using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRespwanBehaviour : CoinBehaviour
{
    public event Action<CoinRespwanBehaviour> OnCollected;

    private Vector3 _previousPosition;
    public override int Collect()
    {
        if (!IsServer)
        {
            Show(false);
            return 0;
        }

        if(_alreadyCollected) return 0;

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        return _coinValue;
    }

    private void Update()
    {
        if(IsClient)
        {
            if(transform.position != _previousPosition)
            {
                Show(true);
                _previousPosition = transform.position;
            }
        }
    }

    internal void Reset()
    {
        _alreadyCollected = false;
    }
}
