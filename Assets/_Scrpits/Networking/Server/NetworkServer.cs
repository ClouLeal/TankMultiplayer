using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager networkManager;
    private Dictionary<ulong,string> clienIdToAuthId = new Dictionary<ulong,string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovelCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }


    private void ApprovelCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var payloadString = System.Text.Encoding.UTF8.GetString(request.Payload);
        var userData = JsonUtility.FromJson<UserData>(payloadString);

        clienIdToAuthId[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        Debug.Log(userData.userName);   

        response.Approved = true;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clienIdToAuthId.TryGetValue(clientId, out var authId))
        {
            clienIdToAuthId.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if(networkManager != null)
        {
            networkManager.ConnectionApprovalCallback -= ApprovelCheck;
            networkManager.OnServerStarted -= OnNetworkReady;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

            if(networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }
    }
}
