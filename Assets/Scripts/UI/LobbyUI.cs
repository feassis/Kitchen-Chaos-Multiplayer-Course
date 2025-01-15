using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        createLobbyButton.onClick.AddListener(OnCreateLobbyButtonClicked);
        quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);
    }

    private void OnQuickJoinButtonClicked()
    {
        KitchenGameLobby.Instance.QuickJoin();
    }

    private void OnCreateLobbyButtonClicked()
    {
        KitchenGameLobby.Instance.CreateLobby("My Lobby", false);
    }

    private void OnMainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
