using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemContainer;
    [SerializeField] private LobbyItem LobbyItemPrefab;

    private bool isJoining;
    private bool isRefresing;

    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefresing) return;
        isRefresing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"),
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            foreach (Transform child in lobbyItemContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                var lobbyItem = Instantiate(LobbyItemPrefab, lobbyItemContainer);
                lobbyItem.Initialize(this,lobby);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        } 
        

       

        isRefresing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;

        try
        {
           Lobby joiningLobby =  await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            var joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);

        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }

        isJoining = false;
    }
}
