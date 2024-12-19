using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObjectList", menuName = "KichenObjList")]
public class KitchenObjectListSO : ScriptableObject
{
    public List<KitchenObjectSO> kitchenObjectSOList;
}
