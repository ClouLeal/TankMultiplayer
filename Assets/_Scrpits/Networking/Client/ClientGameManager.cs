using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using Unity.Services.Authentication;

public class ClientGameManager : IDisposable
{

    private const string MenuSceneName = "Menu";
    private JoinAllocation allocation;
    private NetworkClient networkClient;

    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        var authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthStates.Authenticated)
        {
            return true;
        }

        return false;
    }

    internal void GotoMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
           allocation =  await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        var userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        var payloadBite = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBite;

        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }
}
