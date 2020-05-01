using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MyScriptable/Create ItemDataBase/Item")]
public class ItemDataBase : ScriptableObject
{

    [SerializeField]
    private List<ItemData> itemLists = new List<ItemData>();

    //　アイテムリストを返す
    public List<ItemData> GetItemDataList(){return itemLists;}
}