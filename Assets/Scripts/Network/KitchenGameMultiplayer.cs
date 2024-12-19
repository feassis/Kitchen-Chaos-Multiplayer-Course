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
        SpawnKitchenObjectServerRpc(GetKitchenObject(kitchenObjectSO), kitchenObjectParent.GetNetWorkObject());
    }

    private int GetKitchenObject(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectList.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjectSO GetKitchenObjectSO(int index)
    {
        return kitchenObjectList.kitchenObjectSOList[index];
    }
}
