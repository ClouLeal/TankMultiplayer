using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private const int MaxConnections = 20;
    private Allocation allocation;
    private string joinCode;
    private string lobbyId;
    private NetworkServer networkServer;


    public const string GameSceneName = "Game";

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        try
        {
            //Creating the lobby 
            var lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;
            lobbyOptions.Data = new Dictionary<string, DataObject>
            {
                {
                    "JoinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                        )
                }
            };

            var playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name");
            var lobby =  await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", MaxConnections, lobbyOptions);
            lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
            return;
        }

        networkServer = new NetworkServer(NetworkManager.Singleton);

        var userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        var payloadBite = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBite;

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }


    //This is needed to avoid the Lobby to me shoutdown , so by documentation it need a ping avery 15 seconds to be alive 
    private IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        var waitTime  = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return waitTime;
        }

    }

    public async void Dispose()
    {
        HostSingleton.Instance.StopCoroutine(nameof(HeartBeatLobby));
        if (!string.IsNullOrEmpty(lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch(LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }

            lobbyId = string.Empty;
        }

        networkServer?.Dispose();
    }
}
