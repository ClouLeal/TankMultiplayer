using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float lifeTimeInSec;

    private void Start()
    {
        Destroy(gameObject, lifeTimeInSec);
    }
}
