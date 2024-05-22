using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{

    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;

    private LobbyList lobbyList;
    private Lobby lobby;

    public void Initialize(LobbyList lobbyList, Lobby lobby)
    {
        this.lobbyList = lobbyList;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbyList.JoinAsync(lobby);
    }
}
