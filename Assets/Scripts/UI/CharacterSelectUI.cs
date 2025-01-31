using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    private void Start()
    {
        Unity.Services.Lobbies.Models.Lobby lobby = KitchenGameLobby.Instance.GetLobby();
        lobbyNameText.text = $"Lobby name: {lobby.Name}";
        lobbyCodeText.text = $"Lobby code: {lobby.LobbyCode}";
    }

    private void OnReadyButtonClicked()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }

    private void OnMainMenuButtonClicked()
    {
        KitchenGameLobby.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
