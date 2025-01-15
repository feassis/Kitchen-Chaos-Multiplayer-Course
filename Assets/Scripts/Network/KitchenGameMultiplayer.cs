using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
 using Scene = Loader.Scene;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectList;
    [SerializeField] private List<Color> playerColorList;

    public const int MAX_PLAYERS = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConectionApprovalCallBack;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.ClientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientID)
    {
        playerDataNetworkList.Add(new PlayerData { 
            ClientId = clientID,
            ColorId = GetFirstUnusedColorId()
        });
    }

    public void StartHostAndLoadScene(Scene desiredScene)
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConectionApprovalCallBack;
        StartCoroutine(StartHostAfterASingleFrameAndLoadScene(desiredScene));
    }

    private IEnumerator StartHostAfterASingleFrameAndLoadScene(Scene scene)
    {
        yield return null;
        NetworkManager.Singleton.StartHost();
        yield return null;
        Loader.LoadNetwork(scene);
    }

    private void NetworkManager_ConectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game already in progress";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYERS)
        {
            response.Approved = false;
            response.Reason = "Game is full";
            return;
        }

        response.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientID)
    {
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectIndex, NetworkObjectReference kitchenNetworkObject)
    {
        Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSO(kitchenObjectIndex).prefab);

        kitchenObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        kitchenNetworkObject.TryGet(out NetworkObject kichenParaentNetworkObject);

        IKitchenObjectParent kitchenParent = kichenParaentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenParent);
    }


    public  void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectIndex(kitchenObjectSO), kitchenObjectParent.GetNetWorkObject());
    }

    public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSO(int index)
    {
        return kitchenObjectList.kitchenObjectSOList[index];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObject)
    {
        kitchenObjectNetworkObject.TryGet(out NetworkObject kitchenObjectNetwork);

        KitchenObject kitchenObject = kitchenObjectNetwork.GetComponent<KitchenObject>();
        ClearKitchenObjectOnParentClientRPC(kitchenObjectNetworkObject);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRPC(NetworkObjectReference kitchenObjectNetworkObject)
    {
        kitchenObjectNetworkObject.TryGet(out NetworkObject kitchenObjectNetwork);

        KitchenObject kitchenObject = kitchenObjectNetwork.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // Color not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.ColorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.ClientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].ClientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.ColorId == colorId)
            {
                // Already in use
                return false;
            }
        }
        return true;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
