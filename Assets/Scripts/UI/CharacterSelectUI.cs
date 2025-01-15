using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    private void OnReadyButtonClicked()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
    }

    private void OnMainMenuButtonClicked()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}
