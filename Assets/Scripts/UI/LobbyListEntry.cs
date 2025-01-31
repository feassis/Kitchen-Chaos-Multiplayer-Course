using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListEntry : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI lobbyNameText;
    [SerializeField] private Button lobbyButton;

    private Lobby lobby;

    private void Awake()
    {
        lobbyButton.onClick.AddListener(OnLobbyButtonClicked);
    }

    private void OnLobbyButtonClicked()
    {
        KitchenGameLobby.Instance.JoinByID(lobby.Id);
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }

}
