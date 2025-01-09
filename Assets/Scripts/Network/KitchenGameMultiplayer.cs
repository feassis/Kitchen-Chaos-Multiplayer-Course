using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectList;

    private void Awake()
    {
        Instance = this;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConectionApprovalCallBack;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (KitchenGameManager.Instance.IsWaitingToStart())
        {
            response.Approved = true;
            response.CreatePlayerObject = true;
        }
        else
        {
            response.Approved = false;
        }
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
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
}
