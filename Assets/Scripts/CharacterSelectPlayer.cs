using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.ClientId);
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnPlayerReadyChanged += CharacterSelectReady_OnPlayerReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        Hide();
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnPlayerReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyText.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));

            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.ColorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
