using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private LobbyListEntry lobbyTemplate;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);
        joinCodeButton.onClick.AddListener(OnJoinCodeButtonClicked);
    }

    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener(OnPlayerNameInputFieldValueChanged);

        KitchenGameLobby.Instance.OnLobbyListChanged += OnLobbyListChanged;
        UpdateLobbyList(new List<Unity.Services.Lobbies.Models.Lobby>());
    }

    private void OnLobbyListChanged(object sender, KitchenGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.Lobbies);
    }

    private void OnPlayerNameInputFieldValueChanged(string newPlayerName)
    {
        KitchenGameMultiplayer.Instance.SetPlayerName(newPlayerName);
    }

    private void OnJoinCodeButtonClicked()
    {
        KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
    }

    private void OnQuickJoinButtonClicked()
    {
        KitchenGameLobby.Instance.QuickJoin();
    }

    private void OnCreateLobbyButtonClicked()
    {
        lobbyCreateUI.Show();
    }

    private void OnMainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void UpdateLobbyList(List<Unity.Services.Lobbies.Models.Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        foreach(Unity.Services.Lobbies.Models.Lobby lobby in lobbyList)
        {
            LobbyListEntry lobbyEntry = Instantiate<LobbyListEntry>(lobbyTemplate, lobbyContainer);
            lobbyEntry.gameObject.SetActive(true);
            lobbyEntry.SetLobby(lobby);
        }
    }
}
