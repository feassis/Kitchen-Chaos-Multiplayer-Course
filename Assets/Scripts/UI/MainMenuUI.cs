using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {


    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button quitButton;


    private void Awake() {
        playMultiplayerButton.onClick.AddListener(() => {
            KitchenGameMultiplayer.PlayMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        singlePlayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.PlayMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        Time.timeScale = 1f;
    }

}