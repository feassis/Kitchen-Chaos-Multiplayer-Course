
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        createPublicButton.onClick.AddListener(CreatePublicLobby);
        createPrivateButton.onClick.AddListener(CreatePrivateLobby);
    }

    private void CreatePrivateLobby()
    {
        KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
    }

    private void CreatePublicLobby()
    {
        KitchenGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
    }

    private void Close()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide() 
    {         
        gameObject.SetActive(false);
    }
}
