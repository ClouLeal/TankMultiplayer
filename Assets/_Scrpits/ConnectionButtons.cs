using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public void HostStart()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ClientStart()
    {
        NetworkManager.Singleton.StartClient();
    }

}
